using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using MultiBranding.ApiClient;
using Portalia.Core.Entity;
using Portalia.Core.Interface.Service;
using Portalia.Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using System.Resources;
using Microsoft.AspNet.Identity;

namespace Portalia.Service
{
    public class UserProfileAttributeService : BaseService<UserProfileAttribute>, IUserProfileAttributeService
    {
        private IRepositoryAsync<AttributeType> _attributeTypeRepository;
        private IRepositoryAsync<AspNetUser> _aspNetUserRepository;
        private PortaliaContext _portaliaContext;
        public UserProfileAttributeService(IRepositoryAsync<UserProfileAttribute> repository,
            IUnitOfWorkAsync unitOfWork, IRepositoryAsync<AttributeType> attributeTypeRepository, IRepositoryAsync<AspNetUser> aspNetUserRepository, PortaliaContext portaliaContext)
            : base(repository, unitOfWork)
        {
            _attributeTypeRepository = attributeTypeRepository;
            _aspNetUserRepository = aspNetUserRepository;
            _portaliaContext = portaliaContext;
        }

        public List<UserProfileAttribute> GetUserProfileAttributeByUserProfileId(int identityUserId)
        {
            return this._Repository.Queryable().Where(c => c.UserProfileId == identityUserId).ToList();
        }

        public List<AttributeType> GetAttributeTypesByUserProfileId(int userProfileId)
        {
            //Get personal type
            var attributeTypes = _attributeTypeRepository.Queryable().Include(c => c.AttributeDetails).Where(c => c.AttributeTypeId == 1).ToList();
            var userProfileAttribute = _Repository.Queryable().Where(c => c.UserProfileId == userProfileId).ToList();
            foreach (var attributeType in attributeTypes)
            {
                foreach (var atributeDetail in attributeType.AttributeDetails)
                {
                    atributeDetail.UserProfileAttributes =
                        userProfileAttribute.Where(c => c.AttributeDetailId == atributeDetail.AttributeDetailId).ToList();
                }
            }
            return attributeTypes;
        }

        public void UpdateAttribute(int pk, string name, string value)
        {
            var userProfileAttribute = this._Repository.Find(pk);
            userProfileAttribute.Value = value;
            userProfileAttribute.ObjectState = ObjectState.Modified;

            if (name == "Last Name")
            {
                var identityUser = _aspNetUserRepository.Find(userProfileAttribute.UserProfile.IdentityUserId);

                userProfileAttribute.UserProfile.LastName = value;
                userProfileAttribute.UserProfile.ObjectState = ObjectState.Modified;

                identityUser.LastName = value;
                identityUser.ObjectState = ObjectState.Modified;

            }
            if (name == "First Name")
            {
                var identityUser = _aspNetUserRepository.Find(userProfileAttribute.UserProfile.IdentityUserId);
                userProfileAttribute.UserProfile.FirstName = value;
                userProfileAttribute.UserProfile.ObjectState = ObjectState.Modified;

                identityUser.FirstName = value;
                identityUser.ObjectState = ObjectState.Modified;

            }
            _unitOfWork.SaveChanges();
        }

        public bool HaveUserProfileAttribute(int userProfileAttributeId, string userId)
        {
            return _Repository.Queryable()
                    .Any(c => c.UserProfile.IdentityUserId == userId && c.UserProfileAttributeId == userProfileAttributeId);
        }

        public void UpdateAttribute(int pk, string name, string value, byte[] fileBinary)
        {
            var userProfileAttribute = this._Repository.Find(pk);
            userProfileAttribute.Value = value;
            userProfileAttribute.ObjectState = ObjectState.Modified;
            userProfileAttribute.FileBinary = fileBinary;
            _unitOfWork.SaveChanges();
        }

        public void UpdateUserProfileAttribute(int userId, string attributeName, string value)
        {
            var attribute = this._Repository.Queryable().FirstOrDefault(c => c.UserProfileId == userId && c.AttributeDetail.Name == attributeName);

            if (attribute == null) return;

            attribute.Value = value;
            attribute.ObjectState = ObjectState.Modified;
            _unitOfWork.SaveChanges();
        }

        public List<string> IsValidUserProfile(string identityUserId)
        {
            var errorList = new List<string>();
            //Get personal Attribute
            var userProfile = _portaliaContext.UserProfiles.Include(c => c.UserProfileAttributes).FirstOrDefault(c => c.IdentityUserId == identityUserId);
            var isFrance = false;
            var isEU = false;
            var enableSwift = false;
            var american = "3";
            var asian = "4";
            var listEU = new List<string>() { "AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FN", "FR", "GB", "GR", "HR", "HU", "IE", "IT", "LT", "LU", "LV", "MT", "NL", "PL", "PT", "RO", "SE", "SI", "SK" };
            if (userProfile != null && userProfile.UserProfileAttributes.Any())
            {
                //Get personal attribute type
                var userProfileAttributes =
                    userProfile.UserProfileAttributes.Where(c => c.AttributeDetail.AttributeTypeId == 1).ToList();
                foreach (var userProfileAttribute in userProfileAttributes)
                {
                    if (userProfileAttribute.AttributeDetail.Name == "Nationality" && !string.IsNullOrEmpty(userProfileAttribute.Value) && userProfileAttribute.Value == "FR")
                    {
                        isFrance = true;
                    }
                    if (!string.IsNullOrEmpty(userProfileAttribute.Value) && userProfileAttribute.AttributeDetail.Name == "Nationality" && (userProfileAttribute.Value == "FR" || listEU.Contains(userProfileAttribute.Value)))
                    {
                        isEU = true;
                    }
                    if (userProfileAttribute.AttributeDetail.Name == "System" && !string.IsNullOrEmpty(userProfileAttribute.Value) && (userProfileAttribute.Value == american || userProfileAttribute.Value == asian))
                    {
                        enableSwift = true;
                    }

                    if (string.IsNullOrEmpty(userProfileAttribute.Value))
                    {
                        errorList.Add(userProfileAttribute.AttributeDetail.Name);
                    }
                }
            }
            if (isFrance)
            {
                errorList.Remove("Passport");
            }
            if (isEU == false || isFrance == false)
            {
                errorList.Remove("ID Card");
                errorList.Remove("Social security number");
            }
            if (enableSwift == false)
            {
                errorList.Remove("Swift");
            }

            if (isEU)
            {
                errorList.Remove("Work permit");
            }
            errorList.Remove("Degrees");
            errorList.Remove("License");
            errorList.Remove("Registration");
            errorList.Remove("Casier Judiciaire");
            errorList.Remove("Social security number");
            return errorList;
        }

        public CreateCandidateParameter GetUserInfoToCreateCandidate(string identityUserId)
        {
            var userProfile = _portaliaContext.UserProfiles.Include(c => c.UserProfileAttributes).FirstOrDefault(c => c.IdentityUserId == identityUserId);
            var userIdentity = _portaliaContext.AspNetUsers.Find(identityUserId);
            var candidateParameter = new CreateCandidateParameter();
            if (userProfile != null)
            {
                var userAttributes = userProfile.UserProfileAttributes;
                candidateParameter.FirstName = userAttributes.FirstOrDefault(c => c.AttributeDetail.Name == "First Name").Value;
                candidateParameter.LastName = userAttributes.FirstOrDefault(c => c.AttributeDetail.Name == "Last Name").Value;
                candidateParameter.HoldingId = 13; //Portalia
                candidateParameter.IsPotentialManager = false;
                candidateParameter.Date = DateTime.Now;
                candidateParameter.SourceId = CandidateSource.Application;
                candidateParameter.SourceCountryId = userAttributes.FirstOrDefault(c => c.AttributeDetail.Name == "Nationality").Value;
                candidateParameter.Email = userIdentity.Email;
                candidateParameter.Status = CandidateStatus.ProposalSubmitted;
                var gender = userAttributes.FirstOrDefault(c => c.AttributeDetail.Name == "Gender").Value;
                candidateParameter.Gender = gender == 1.ToString() ? Gender.Male : Gender.Female;
            }
            return candidateParameter;
        }
        public int CountMissingField(int userId, string attributeType)
        {
            var userProfile = _portaliaContext.UserProfiles.Include(c => c.UserProfileAttributes).FirstOrDefault(c => (c.UserProfileId == userId));
            if (userProfile != null)
                return
                    userProfile.UserProfileAttributes.Count(c => c.AttributeDetail.AttributeType.Label == attributeType && c.AttributeDetail.IsRequired && string.IsNullOrEmpty(c.Value));
            return 0;
        }

        public string GetUserProfileByUserIdAndAttributeName(string identityUserId, string attributeName)
        {
            var userProfile = _portaliaContext.UserProfiles.Include(c => c.UserProfileAttributes).FirstOrDefault(c => c.IdentityUserId == identityUserId);
            var genderAttribute =
                userProfile?.UserProfileAttributes.FirstOrDefault(c => c.AttributeDetail.Name == attributeName);
            return genderAttribute?.Value;
        }

        public UserProfileAttribute GetUserProfileDocument(int userProfileAttributeId, string userId)
        {
            return
                _portaliaContext.UserProfileAttributes.FirstOrDefault(
                    c => c.UserProfileAttributeId == userProfileAttributeId && c.UserProfile.IdentityUserId == userId);

        }

        public string CountMissingField(IPrincipal user, ResourceManager userProfileResource)
        {
            var isValidUserProfile = IsValidUserProfile(user.Identity.GetUserId()).Select(s => userProfileResource.GetString(s.Replace(" ", string.Empty)));

            var validUserProfile = isValidUserProfile as string[] ?? isValidUserProfile.ToArray();

            return validUserProfile.Any() ? validUserProfile.Count().ToString() : string.Empty;
        }
    }
}
