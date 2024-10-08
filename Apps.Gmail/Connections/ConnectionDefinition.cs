﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Gmail.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new ConnectionPropertyGroup
        {
            Name = "OAuth",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>
            {
            }
        },
        //new ConnectionPropertyGroup
        //{
        //    Name = "Service account",
        //    AuthenticationType = ConnectionAuthenticationType.Undefined,
        //    ConnectionUsage = ConnectionUsage.Actions,
        //    ConnectionProperties = new List<ConnectionProperty>()
        //    {
        //        new ConnectionProperty("serviceAccountConfString")
        //    }
        //},
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
    {
        return values
            .Select(x =>
                new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None, x.Key, x .Value));
    }
}