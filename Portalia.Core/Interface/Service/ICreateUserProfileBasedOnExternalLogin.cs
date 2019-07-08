using System.Collections.Generic;
using System.Security.Claims;
using Portalia.Core.Entity;

namespace Portalia.Core.Interface.Service
{
    public interface ICreateUserProfileBasedOnExternalLogin
    {
        UserProfile CreateUserProfile(string userIdentityId,List<Claim> userClaims);
    }
}
