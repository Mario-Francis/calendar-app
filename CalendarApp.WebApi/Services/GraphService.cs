using System.Globalization;
using CalendarApp.WebApi.Exceptions;
using CalendarApp.WebApi.Models;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.Calendar.GetSchedule;
using Microsoft.Graph.Users.Item.Events.Item.Cancel;
using GModels = Microsoft.Graph.Models;

namespace CalendarApp.WebApi.Services;

public class GraphService(GraphServiceClient graphServiceClient) : IGraphService
{
    private readonly GraphServiceClient _graphClient = graphServiceClient;

    public async Task<List<User>> GetUsersStartingWith(List<string> searchTerms)
    {
        if (searchTerms == null || searchTerms.Count == 0)
        {
            throw new ValidationException("Search terms cannot be null or empty.");
        }

        var _filterConditions = searchTerms
        .Where(q => !string.IsNullOrWhiteSpace(q))
        .Select(q =>
            $"(startswith(displayName, '{q}') or startswith(mail, '{q}') or startswith(userPrincipalName, '{q}') or startswith(givenName, '{q}') or startswith(surname, '{q}'))")
        .ToList();

        if (_filterConditions.Count == 0)
        {
            throw new ValidationException("Search terms cannot be null or empty.");
        }

        // Join the conditions with 'or'
        var _filter = string.Join(" or ", _filterConditions);

        var _users = await _graphClient.Users.GetAsync(config =>
            {
                config.QueryParameters.Filter = _filter;
                config.QueryParameters.Select = new[] { "id", "displayName", "mail", "userPrincipalName", "givenName", "surname" };
            });

        if (_users?.Value == null || _users.Value.Count == 0)
        {
            return new List<User>();
        }

        return [.. _users.Value.Select(u => new User
        {
            Id = u.Id,
            Email = u.Mail ?? u.UserPrincipalName,
            FirstName = u.GivenName,
            LastName = u.Surname,
            DisplayName = u.DisplayName,
        })];
    }

    public async Task<List<User>> GetUsersByEmails(List<string> emails)
    {
        if (emails == null || emails.Count == 0)
        {
            throw new ValidationException("Emails cannot be null or empty.");
        }

        var _filterConditions = emails
        .Where(e => !string.IsNullOrWhiteSpace(e))
        .Select(e => $"(mail eq '{e}' or userPrincipalName eq '{e}')")
        .ToList();

        if (_filterConditions.Count == 0)
        {
            throw new ValidationException("Emails cannot be null or empty.");
        }

        // Join the conditions with 'or'
        var _filter = string.Join(" or ", _filterConditions);

        var _users = await _graphClient.Users.GetAsync(config =>
            {
                config.QueryParameters.Filter = _filter;
                config.QueryParameters.Select = new[] { "id", "displayName", "mail", "userPrincipalName", "givenName", "surname" };
            });

        if (_users?.Value == null || _users.Value.Count == 0)
        {
            return new List<User>();
        }

        return [.. _users.Value.Select(u => new User
        {
            Id = u.Id,
            Email = u.Mail ?? u.UserPrincipalName,
            FirstName = u.GivenName,
            LastName = u.Surname,
            DisplayName = u.DisplayName,
        })];
    }

    public async Task<List<Schedule>> GetUsersAvailability(AvailabilityRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InitiatingUserEmail))
        {
            throw new ValidationException("Initiating user email cannot be null.");
        }

        if (request == null || request.Emails.Count == 0)
        {
            throw new ValidationException("Emails cannot be null or empty.");
        }

        if (request.StartTime == null || request.EndTime == null)
        {
            throw new ValidationException("Start and end time cannot be null.");
        }

        if (request.StartTime < DateTimeOffset.Now || request.EndTime < DateTimeOffset.Now)
        {
            throw new ValidationException("Start and end time cannot be in the past.");
        }

        if (request.StartTime >= request.EndTime)
        {
            throw new ValidationException("Start time must be before end time.");
        }

        // Build the request body for the GetSchedule endpoint.
        var scheduleRequestBody = new GetSchedulePostRequestBody
        {
            Schedules = request.Emails,
            StartTime = new Microsoft.Graph.Models.DateTimeTimeZone
            {
                DateTime = request.StartTime.Value.ToString("o"), // ISO 8601 format
                TimeZone = request.TimeZone
            },
            EndTime = new Microsoft.Graph.Models.DateTimeTimeZone
            {
                DateTime = request.EndTime.Value.ToString("o"),
                TimeZone = request.TimeZone
            },
            AvailabilityViewInterval = request.AvailabilityViewInterval
        };

        // Call the GetSchedule endpoint.
        GetSchedulePostResponse? scheduleResponse = await _graphClient.Users[request.InitiatingUserEmail]
                                                        .Calendar.GetSchedule.PostAsGetSchedulePostResponseAsync(scheduleRequestBody);
        if (scheduleResponse == null)
        {
            return new List<Schedule>();
        }

        var _schedules = scheduleResponse.Value!.Select(s => new Schedule
        {
            ScheduleId = s.ScheduleId,
            AvailabilityView = s.AvailabilityView,
            Error = s.Error?.Message,
            WorkingHours = new WorkingHours
            {
                DaysOfWeek = s.WorkingHours!.DaysOfWeek!.Select(d => (DayOfWeek)(int)d!.Value).ToArray(),
                StartTime = CreateDateTimeOffset(s.WorkingHours!.StartTime!.Value.DateTime, s.WorkingHours!.TimeZone!.Name!),
                EndTime = CreateDateTimeOffset(s.WorkingHours!.EndTime!.Value.DateTime, s.WorkingHours!.TimeZone!.Name!),
            },
            ScheduleItems = s.ScheduleItems?.Select(si => new ScheduleItem
            {
                Start = CreateDateTimeOffset(DateTime.Parse(si.Start!.DateTime!, CultureInfo.InvariantCulture), si.Start.TimeZone!),
                End = CreateDateTimeOffset(DateTime.Parse(si.End!.DateTime!, CultureInfo.InvariantCulture), si.End.TimeZone!),
                Subject = si.Subject,
                Status = (ScheduleStatus)(int)si.Status!.Value
            }).ToArray()
        }).ToList();

        return _schedules;
    }

    public async Task<MeetingResponse> CreateMeeting(CreateMeetingRequest request)
    {
        ValidateCreateMeetingRequest(request);

        var _filteredAttendees = request.Attendees.Where(a => !string.IsNullOrWhiteSpace(a.Email)).ToList();
        if (_filteredAttendees.Count == 0)
        {
            throw new ValidationException("Attendees cannot be null or empty.");
        }

        // Create the meeting request.
        var _event = new Microsoft.Graph.Models.Event
        {
            Subject = request.Subject,
            Body = new GModels.ItemBody
            {
                ContentType = GModels.BodyType.Text,
                Content = request.Body ?? string.Empty
            },
            Start = new GModels.DateTimeTimeZone
            {
                DateTime = request.Start!.Value.ToString("o"),
                TimeZone = GetTimeZoneId(request.Start.Value.Offset)
            },
            End = new GModels.DateTimeTimeZone
            {
                DateTime = request.End!.Value.ToString("o"),
                TimeZone = GetTimeZoneId(request.End.Value.Offset)
            },
            Attendees = [.. _filteredAttendees.Select(a => new Microsoft.Graph.Models.Attendee
            {
                EmailAddress = new GModels.EmailAddress
                {
                    Address = a.Email,
                    Name = a.Name,
                },
                Type = a.IsRequired ? GModels.AttendeeType.Required : GModels.AttendeeType.Optional,
            })],
            Location = new GModels.Location
            {
                DisplayName = request.Location ?? "Online"
            },
            IsOnlineMeeting = request.IsOnlineMeeting || string.IsNullOrWhiteSpace(request.Location),
        };

        var _createdEvent = await _graphClient.Users[request.InitiatingUserEmail].Events.PostAsync(_event);

        return new MeetingResponse
        {
            Id = _createdEvent!.Id,
            Subject = _createdEvent.Subject,
            MeetingLink = _createdEvent.OnlineMeeting?.JoinUrl,
        };
    }

    private static void ValidateCreateMeetingRequest(CreateMeetingRequest request)
    {
        // validate the request.
        if (string.IsNullOrWhiteSpace(request.InitiatingUserEmail))
        {
            throw new ValidationException("Initiating user email cannot be null.");
        }
        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            throw new ValidationException("Subject cannot be null.");
        }
        if (request.Start == null || request.End == null)
        {
            throw new ValidationException("Start and end time cannot be null.");
        }
        if (request.Start < DateTimeOffset.Now || request.End < DateTimeOffset.Now)
        {
            throw new ValidationException("Start and end time cannot be in the past.");
        }
        if (request.Start >= request.End)
        {
            throw new ValidationException("Start time must be before end time.");
        }
        if (request.Attendees == null || request.Attendees.Length == 0)
        {
            throw new ValidationException("Attendees cannot be null or empty.");
        }
    }

    public async Task<MeetingResponse> UpdateMeeting(UpdateMeetingRequest request)
    {
        // validate the request.
        ValidateUpdateMeetingRequest(request);
        List<Attendee>? _filteredAttendees = null;
        if (request.Attendees != null)
        {
            _filteredAttendees = request.Attendees.Where(a => !string.IsNullOrWhiteSpace(a.Email)).ToList();
        }

        // update the meeting request.
        var _event = new GModels.Event();
        if (request.Subject != null)
        {
            _event.Subject = request.Subject;
        }
        if (request.Body != null)
        {
            _event.Body = new GModels.ItemBody
            {
                ContentType = GModels.BodyType.Text,
                Content = request.Body
            };
        }
        if (request.Start != null && request.End != null)
        {
            _event.Start = new GModels.DateTimeTimeZone
            {
                DateTime = request.Start.Value.ToString("o"),
                TimeZone = GetTimeZoneId(request.Start.Value.Offset)
            };
            _event.End = new GModels.DateTimeTimeZone
            {
                DateTime = request.End.Value.ToString("o"),
                TimeZone = GetTimeZoneId(request.End.Value.Offset)
            };
        }
        if (_filteredAttendees != null && _filteredAttendees.Count > 0)
        {
            _event.Attendees = [.. _filteredAttendees.Select(a => new GModels.Attendee
            {
                EmailAddress = new GModels.EmailAddress
                {
                    Address = a.Email,
                    Name = a.Name,
                },
                Type = a.IsRequired ? GModels.AttendeeType.Required : GModels.AttendeeType.Optional,
            })];
        }
        if (request.Location != null)
        {
            _event.Location = new GModels.Location
            {
                DisplayName = request.Location
            };
        }

        if (request.IsOnlineMeeting != null)
        {
            _event.IsOnlineMeeting = request.IsOnlineMeeting.Value;
        }

        var _updatedEvent = await _graphClient.Users[request.InitiatingUserEmail].Events[request.Id].PatchAsync(_event);

        return new MeetingResponse
        {
            Id = _updatedEvent!.Id,
            Subject = _updatedEvent.Subject,
            MeetingLink = _updatedEvent.OnlineMeeting?.JoinUrl,
        };
    }

    public async Task CancelMeeting(CancelMeetingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ValidationException("Meeting ID cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(request.InitiatingUserEmail))
        {
            throw new ValidationException("Initiating user email cannot be null.");
        }

        var _cancelRequestBody = new CancelPostRequestBody
        {
            // The comment will be sent to attendees as part of the cancellation notice.
            Comment = request.Comment
        };

        await _graphClient.Users[request.InitiatingUserEmail].Events[request.Id].Cancel.PostAsync(_cancelRequestBody);
    }

    private static void ValidateUpdateMeetingRequest(UpdateMeetingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ValidationException("Meeting ID cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(request.InitiatingUserEmail))
        {
            throw new ValidationException("Initiating user email cannot be null.");
        }

        if (request.Start != null || request.End != null)
        {
            if (request.Start == null || request.End == null)
            {
                throw new ValidationException("Start and end time cannot be null. Either omit or set both");
            }

            if (request.Start < DateTimeOffset.Now || request.End < DateTimeOffset.Now)
            {
                throw new ValidationException("Start and end time cannot be in the past.");
            }

            if (request.Start >= request.End)
            {
                throw new ValidationException("Start time must be before end time.");
            }
        }
    }

    public static DateTimeOffset CreateDateTimeOffset(DateTime dateTime, string timeZoneId)
    {
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateTimeInTimeZone = TimeZoneInfo.ConvertTime(dateTime, timeZone);
        return new DateTimeOffset(dateTimeInTimeZone, timeZone.GetUtcOffset(dateTimeInTimeZone));
    }

    public static string GetTimeZoneId(TimeSpan offset)
    {
        var _timeZone = TimeZoneInfo.GetSystemTimeZones().First(tz => tz.BaseUtcOffset == offset);
        return _timeZone.StandardName;
    }
}
