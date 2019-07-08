using System.Collections.Generic;
using Portalia.Core.Entity;
using MultiBranding.ApiClient;
using System.Security.Principal;
using System.Resources;

namespace Portalia.Core.Interface.Service
{
    public interface IUserProfileAttributeService
    {
        List<UserProfileAttribute> GetUserProfileAttributeByUserProfileId(int identityUserId);
        List<AttributeType> GetAttributeTypesByUserProfileId(int userProfileId);
        void UpdateAttribute(int pk, string name, string value);
        bool HaveUserProfileAttribute(int userProfileAttributeId, string userId);
        void UpdateAttribute(int pk, string name, string value, byte[] fileBinary);
        List<string> IsValidUserProfile(string identityUserId);
        CreateCandidateParameter GetUserInfoToCreateCandidate(string identityUserId);
        int CountMissingField(int userId, string attributeType);
        void UpdateUserProfileAttribute(int userProfileId, string attributeName, string value);
        string GetUserProfileByUserIdAndAttributeName(string identityUserId, string attributeName);
        UserProfileAttribute GetUserProfileDocument(int userProfileAttributeId, string userId);
        string CountMissingField(IPrincipal user, ResourceManager userProfileResource);
    }
}
