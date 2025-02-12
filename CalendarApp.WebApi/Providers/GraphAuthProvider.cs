using CalendarApp.WebApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace CalendarApp.WebApi.Providers;

public class GraphAuthProvider : IAuthenticationProvider
{
    private readonly IConfidentialClientApplication _clientApp;
    private readonly AzureAdSettings _azureAdSettings;

    public GraphAuthProvider(IOptions<AzureAdSettings> azureAdSettings)
    {
        // Build the confidential client application using settings from configuration.
        _azureAdSettings = azureAdSettings.Value;
        _clientApp = ConfidentialClientApplicationBuilder.Create(_azureAdSettings.ClientId)
            .WithClientSecret(_azureAdSettings.ClientSecret)
            .WithAuthority($"{_azureAdSettings.Instance}{_azureAdSettings.TenantId}")
            .Build();
    }

    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        // Define the scopes. Using .default means "use all permissions granted to the app" in Azure AD.
        string[] scopes = this._azureAdSettings.Scopes;

        // Acquire a token using client credentials.
        var authResult = await _clientApp.AcquireTokenForClient(scopes)
                                         .ExecuteAsync(cancellationToken)
                                         .ConfigureAwait(false);

        // Kiota's RequestInformation holds headers in a dictionary.
        // Ensure the "Authorization" header is set with the Bearer token.
        request.Headers.Add("Authorization", $"Bearer {authResult.AccessToken}");
    }
}
