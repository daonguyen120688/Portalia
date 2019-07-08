using System;
using System.Collections.Generic;
using Portalia.Core.Entity;
using Portalia.Core.Interface.Service;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace Portalia.Service
{
    public class AttributeTypeService : BaseService<AttributeType>, IAttributeTypeService
    {
        private readonly IRepositoryAsync<UserProfileAttribute> userProfileAttributeRepositoryAsync;
        private readonly IRepositoryAsync<UserProfile> userProfileRepository;
        public AttributeTypeService(IRepositoryAsync<AttributeType> repository, IUnitOfWorkAsync unitOfWork, IRepositoryAsync<UserProfileAttribute> userProfileAttributeRepositoryAsync, IRepositoryAsync<UserProfile> userProfileRepository) : base(repository, unitOfWork)
        {
            this.userProfileAttributeRepositoryAsync = userProfileAttributeRepositoryAsync;
            this.userProfileRepository = userProfileRepository;
        }

        public List<UserProfileAttribute> GetAttributesByUserIdentityId(int userProfileId)
        {
            throw new NotImplementedException();
        }
    }
}
