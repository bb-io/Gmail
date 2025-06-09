using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;

namespace Apps.Gmail.Clients;

public class GoogleGmailClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
    : GmailService(GetInitializer(authenticationCredentialsProviders))
{
    private static Initializer GetInitializer(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var accessTokenProvider = authenticationCredentialsProviders.First(p => p.KeyName == "access_token");
        if (accessTokenProvider == null || string.IsNullOrEmpty(accessTokenProvider.Value))
        {
            throw new InvalidOperationException("Access token provider not found in authentication credentials.");
        }

        var accessToken = accessTokenProvider.Value;
        GoogleCredential credentials = GoogleCredential.FromAccessToken(accessToken);
        return new Initializer
        {
            HttpClientInitializer = credentials,
            ApplicationName = "Blackbird"
        };
            
    }
}