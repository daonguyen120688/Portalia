using System.Collections.Generic;
using Portalia.Core.Entity;

namespace Portalia.Core.Interface.Service
{
    public interface IAttributeTypeService
    {
        List<UserProfileAttribute> GetAttributesByUserIdentityId(int userProfileId);
    }
}
