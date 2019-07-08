using System.Collections.Generic;
using System.Security.Claims;
using Portalia.Core.Entity;
using Portalia.Core.Interface.Service;

namespace Portalia.Service
{
    public class LinkedinUserProfile : ICreateUserProfileBasedOnExternalLogin
    {
        private IUserProfileService _userProfileService;

        public LinkedinUserProfile(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        public UserProfile CreateUserProfile(string userIdentityId, List<Claim> userClaims)
        {
            var firstName = userClaims.Find(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value;
            var lastName = userClaims.Find(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
            var userProfile = _userProfileService.CreateUserProfile(new Core.Entity.UserProfile
            {
                IdentityUserId = userIdentityId,
                FirstName = firstName,
                LastName = lastName
            });
            return userProfile;
        }
    }
}
