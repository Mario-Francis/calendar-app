# Calendar App

Calendar App is a web API application that integrates with Microsoft Graph to manage users, their availability, and meetings. It provides functionalities to search for users, check their availability, create, update, and cancel meetings.

## Features

- **User Management**: Search for users by name or email.
- **Availability Check**: Get the availability of users for scheduling meetings.
- **Meeting Management**: Create, update, and cancel meetings.

## Technologies Used

- **.NET 8**
- **Microsoft Graph API**
- **C#**

## Setup Instructions

1. **Clone the repository**:

    ```sh
    git clone https://github.com/Mario-Francis/calendar-app.git
    cd calendar-app
    ```

2. **Install dependencies**:

    ```sh
    dotnet restore
    ```

3. **Set up environment variables**:
    Create a `.env` file in the root directory and add the following variables:

    ```env
    AzureAdSettings__Instance=https://login.microsoftonline.com/
    AzureAdSettings__TenantId=your-tenant-id
    AzureAdSettings__ClientId=your-client-id
    AzureAdSettings__ClientSecret=your-client-secret
    ```

4. **Run the application**:

    ```sh
    dotnet run --project CalendarApp.WebApi
    ```

## Usage

### Get Users Starting With

Endpoint: `GET /api/users/search/starts-with`

Request Body:

```json
{
    "searchTerms": ["John", "Doe"]
}
```

### Get Users by Emails

Endpoint: `GET /api/users/search/emails-in`

Request Body:

```json
{
    "emails": ["john.doe@example.com", "jane.doe@example.com"]
}
```

### Get Users Availability

Endpoint: `POST /api/calendar/check-availability`

Request Body:

```json
{
    "initiatingUserEmail": "organizer@example.com",
    "emails": ["attendee1@example.com", "attendee2@example.com"],
    "startTime": "2023-10-01T09:00:00Z",
    "endTime": "2023-10-01T10:00:00Z",
    "timeZone": "Pacific Standard Time",
    "availabilityViewInterval": 30
}
```

### Create Meeting

Endpoint: `POST /api/calendar/events/create`

Request Body:

```json
{
    "initiatingUserEmail": "organizer@example.com",
    "subject": "Team Meeting",
    "body": "Discuss project updates",
    "start": "2023-10-01T09:00:00Z",
    "end": "2023-10-01T10:00:00Z",
    "attendees": [
        {"email": "attendee1@example.com", "name": "Attendee One", "isRequired": true},
        {"email": "attendee2@example.com", "name": "Attendee Two", "isRequired": false}
    ],
    "location": "Conference Room",
    "isOnlineMeeting": true
}
```

### Update Meeting

Endpoint: `PATCH /api/calendar/events/update`

Request Body:

```json
{
    "initiatingUserEmail": "organizer@example.com",
    "subject": "Updated Team Meeting",
    "body": "Updated discussion points",
    "start": "2023-10-01T09:30:00Z",
    "end": "2023-10-01T10:30:00Z",
    "attendees": [
        {"email": "attendee1@example.com", "name": "Attendee One", "isRequired": true},
        {"email": "attendee2@example.com", "name": "Attendee Two", "isRequired": false}
    ],
    "location": "Updated Conference Room",
    "isOnlineMeeting": true
}
```

### Cancel Meeting

Endpoint: `DELETE /api/calendar/events/cancel`

Request Body:

```json
{
    "initiatingUserEmail": "organizer@example.com",
    "comment": "Meeting has been cancelled"
}
```

## License

This project is licensed under the MIT License.
