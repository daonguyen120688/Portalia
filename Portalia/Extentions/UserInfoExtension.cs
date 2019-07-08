using System;
using Auth0.AuthenticationApi.Models;
using Portalia.Auth0.Constants;

namespace Portalia.Extentions
{
    public static class UserInfoExtension
    {
        public static bool IsUsernamePassword(this UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return (string)user.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] == Auth0Constants.Auth0Connection;
        }

        public static bool IsFromAdfs(this UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return (string)user.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] == Auth0Constants.AdfsConnection;
        }

        public static bool IsFromLinkedIn(this UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return (string)user.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] == Auth0Constants.LinkedInConnection;
        }
    }
}