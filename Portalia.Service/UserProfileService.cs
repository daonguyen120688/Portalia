using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using PagedList;
using Portalia.Core.Dtos.User;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Extensions;
using Portalia.Core.Interface.Service;
using Portalia.Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace Portalia.Service
{
    public class UserProfileService : IUserProfileService
    {

        private readonly IRepositoryAsync<UserProfile> _userProfilerRepository;
        private readonly IRepositoryAsync<AspNetUser> _userIdentityRepository;
        private readonly IRepositoryAsync<AttributeDetail> _attributeDetailRepository;
        private readonly IRepositoryAsync<UserProfileAttribute> _userProfileAttributeRepository;
        private readonly IRepositoryAsync<TrackingAction> _trackingActionRepository;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private PortaliaContext _portaliaContext;

        public UserProfileService(IRepositoryAsync<UserProfile> repositoryAsync, IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<AspNetUser> userIdentityRepository, IRepositoryAsync<AttributeDetail> atteibuteDetailRepository,
            IRepositoryAsync<UserProfileAttribute> userProfileAttributeRepository, IRepositoryAsync<TrackingAction> trackingActionRepository,
            PortaliaContext portaliaContext)
        {
            _userProfilerRepository = repositoryAsync;
            _unitOfWork = unitOfWork;
            _userIdentityRepository = userIdentityRepository;
            _attributeDetailRepository = atteibuteDetailRepository;
            _userProfileAttributeRepository = userProfileAttributeRepository;
            _trackingActionRepository = trackingActionRepository;
            _portaliaContext = portaliaContext;
        }



        public UserProfile GetUserProfile(string userId)
        {
            var user = _userProfilerRepository.Queryable().FirstOrDefault(c => c.IdentityUserId == userId);
            return user;
        }

        public IPagedList<AspNetUser> GetUsersIdentity(int? page)
        {
            var pageNumber = page ?? 1;
            var user = _userIdentityRepository.Queryable().OrderByDescending(c => c.IsActive).ThenByDescending(c => c.Created).ToPagedList(pageNumber, 5);
            return user;
        }

        public IPagedList<AspNetUser> GetUsersIdentity(string emailAddress, int? page)
        {
            var pageNumber = page ?? 1;
            var user = _userIdentityRepository.Queryable().Where(c => c.Email.ToLower().Contains(emailAddress.ToLower())).OrderByDescending(c => c.Created).ToPagedList(pageNumber, 5);
            return user;
        }

        public AspNetUser GetUsersIdentityById(string id)
        {
            var user = _userIdentityRepository.Find(id);
            return user;
        }

        public void DeActiveUser(string userId)
        {
            var user = _userIdentityRepository.Find(userId);
            user.IsActive = false;
            user.ObjectState = ObjectState.Modified;
            _unitOfWork.SaveChanges();
        }

        public List<AspNetUser> GetUsersIdentityForApi()
        {
            _portaliaContext.Configuration.LazyLoadingEnabled = false;
            return _portaliaContext.AspNetUsers.ToList();
        }

        public void UpdateProfileIdentity(AspNetUser model)
        {
            var user = _userIdentityRepository.Find(model.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.ObjectState = ObjectState.Modified;
            _unitOfWork.SaveChanges();
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            _unitOfWork.SaveChanges();
        }

        public void UpdateEmployeeStatus(string email, bool status)
        {
            var userIdentity = _portaliaContext.AspNetUsers.FirstOrDefault(c => c.Email == email);
            if (userIdentity != null)
            {
                userIdentity.IsEmployee = status;
                userIdentity.ObjectState = ObjectState.Modified;
            }
            _portaliaContext.SaveChanges();
        }

        public void UpdateEmployeeStatus(string email, string status)
        {
            var userIdentity = _portaliaContext.AspNetUsers.FirstOrDefault(c => c.Email == email);
            if (userIdentity != null)
            {
                userIdentity.EmployeeStatus = status;
                userIdentity.ObjectState = ObjectState.Modified;
            }
            _portaliaContext.SaveChanges();
        }

        public List<AspNetUserExtraInfo> GetUsersIdentity()
        {
            var query = from c in _portaliaContext.AspNetUsers.ToList()
                        join p in _portaliaContext.UserProfiles.ToList() on c.Id equals p.IdentityUserId
                        select new AspNetUserExtraInfo()
                        {
                            IsActive = c.IsActive,
                            IsEmployee = c.IsEmployee,
                            IsNew = c.IsNew,
                            Email = c.Email,
                            Id = c.Id,
                            FullName = c.FullName,
                            Created = c.Created,
                            PercentOfInfo = GetPerCentOfInfo(c.Id),
                            PortaliaUserProfileId = p.UserProfileId
                        };
            return query.ToList();
        }
        
        public void UpdateStatusForNewUser(string userId, bool status)
        {
            var userIdentity = _portaliaContext.AspNetUsers.Find(userId);
            if (userIdentity != null)
            {
                userIdentity.IsNew = status;
                userIdentity.ObjectState = ObjectState.Modified;
            }
            _portaliaContext.SaveChanges();
        }

        public void ChangePasswordForUser(string userName)
        {
            _portaliaContext.Database.ExecuteSqlCommand("update [Portalia].[dbo].[AspNetUsers] set PasswordHash = 'AGPyc6vhK3UAkjx+uQ4zgOsSP9qqnuq5+7u2GFMlM69YRYuCvtx7xnXF8PXfpJN8dw==' where email = @userName", new SqlParameter("@userName", userName));
        }

        public List<AspNetUserExtraInfo> GetUsersIdentityByUserName(string userName)
        {
            return _portaliaContext.AspNetUsers.Where(c => c.FirstName.Contains(userName) || c.LastName.Contains(userName)).AsNoTracking().ToList().Select(user => new AspNetUserExtraInfo()
            {
                IsActive = user.IsActive,
                IsEmployee = user.IsEmployee,
                IsNew = user.IsNew,
                Email = user.Email,
                Id = user.Id,
                FullName = user.FullName,
                Created = user.Created,
                PercentOfInfo = GetPerCentOfInfo(user.Id)
            }).ToList();
        }

        public int GetUserProfileIdByUserIdentityId(string userIdentityId)
        {
            return
                _portaliaContext.UserProfiles.FirstOrDefault(c => c.IdentityUserId == userIdentityId)?.UserProfileId ?? 0;
        }

        public PagedList<AspNetUserExtraInfo> GetUsersIdentity(int page, int size, bool? isEmployee = null)
        {
            if (isEmployee == true)
            {
                var query = from c in _portaliaContext.AspNetUsers.ToList()
                            join p in _portaliaContext.UserProfiles.ToList() on c.Id equals p.IdentityUserId
                            where c.IsEmployee
                            select new AspNetUserExtraInfo()
                            {
                                IsActive = c.IsActive,
                                IsEmployee = c.IsEmployee,
                                IsNew = c.IsNew,
                                Email = c.Email,
                                Id = c.Id,
                                FullName = c.FullName,
                                Created = c.Created,
                                PercentOfInfo = GetPerCentOfInfo(c.Id),
                                PortaliaUserProfileId = p.UserProfileId,
                            };
                return new PagedList<AspNetUserExtraInfo>(query.OrderByDescending(c => c.Created).AsQueryable(), page, size);
            }
            else if(isEmployee == false)
            {
                var query = from c in _portaliaContext.AspNetUsers.ToList()
                            join p in _portaliaContext.UserProfiles.ToList() on c.Id equals p.IdentityUserId
                            where !c.IsEmployee
                            select new AspNetUserExtraInfo()
                            {
                                IsActive = c.IsActive,
                                IsEmployee = c.IsEmployee,
                                IsNew = c.IsNew,
                                Email = c.Email,
                                Id = c.Id,
                                FullName = c.FullName,
                                Created = c.Created,
                                PercentOfInfo = GetPerCentOfInfo(c.Id),
                                PortaliaUserProfileId = p.UserProfileId,
                            };
                return new PagedList<AspNetUserExtraInfo>(query.OrderByDescending(c => c.Created).AsQueryable(), page, size);
            }
            else
            {
                var query = from c in _portaliaContext.AspNetUsers.ToList()
                            join p in _portaliaContext.UserProfiles.ToList() on c.Id equals p.IdentityUserId
                            select new AspNetUserExtraInfo()
                            {
                                IsActive = c.IsActive,
                                IsEmployee = c.IsEmployee,
                                IsNew = c.IsNew,
                                Email = c.Email,
                                Id = c.Id,
                                FullName = c.FullName,
                                Created = c.Created,
                                PercentOfInfo = GetPerCentOfInfo(c.Id),
                                PortaliaUserProfileId = p.UserProfileId,
                            };
                return new PagedList<AspNetUserExtraInfo>(query.OrderByDescending(c => c.Created).AsQueryable(), page, size);
            }
        }

        public AspNetUserExtraInfo GetUsersIdentityByEmail(string email)
        {
            var query = from c in _portaliaContext.AspNetUsers.ToList()
                        join p in _portaliaContext.UserProfiles.ToList() on c.Id equals p.IdentityUserId
                        where c.Email == email
                        select new AspNetUserExtraInfo()
                        {
                            IsActive = c.IsActive,
                            IsEmployee = c.IsEmployee,
                            IsNew = c.IsNew,
                            Email = c.Email,
                            Id = c.Id,
                            FullName = c.FullName,
                            Created = c.Created,
                            PercentOfInfo = GetPerCentOfInfo(c.Id),
                            PortaliaUserProfileId = p.UserProfileId,
                        };
            return query.FirstOrDefault();
        }

        public AspNetUserExtraInfo GetUserById(int userId)
        {
            var query = from c in _portaliaContext.AspNetUsers.ToList()
                        join p in _portaliaContext.UserProfiles.ToList() on c.Id equals p.IdentityUserId
                        where p.UserProfileId == userId
                        select new AspNetUserExtraInfo()
                        {
                            IsActive = c.IsActive,
                            IsEmployee = c.IsEmployee,
                            IsNew = c.IsNew,
                            Email = c.Email,
                            Id = c.Id,
                            FullName = c.FullName,
                            Created = c.Created,
                            PercentOfInfo = GetPerCentOfInfo(c.Id),
                            PortaliaUserProfileId = p.UserProfileId,
                        };
            return query.FirstOrDefault();
        }

        public PasswordValidationMessageDto GetPasswordValidationMessage(string password)
        {
            var passwordValidationMessage = new PasswordValidationMessageDto
            {
                HasError = true,
                Messages = new List<string>
                {
                    "Password is required"
                }
            };

            if (string.IsNullOrEmpty(password))
            {
                return passwordValidationMessage;
            }

            passwordValidationMessage.Messages.Clear();

            if (password.Length < 8)
            {
                passwordValidationMessage.Messages.Add("Minimum lenght is 8");
            }

            if (!password.HasContainedLowerCase())
            {
                passwordValidationMessage.Messages.Add("At least 1 lower case");
            }

            if (!password.HasContainedUpperCase())
            {
                passwordValidationMessage.Messages.Add("At least 1 upper case");
            }

            if (!password.HasContainedNumber())
            {
                passwordValidationMessage.Messages.Add("At least 1 number");
            }

            if (!password.HasContainedSpecialCharacter())
            {
                passwordValidationMessage.Messages.Add("At least 1 special character");
            }

            passwordValidationMessage.HasError = passwordValidationMessage.Messages.Count > 0;

            return passwordValidationMessage;
        }

        public UserProfile CreateUserProfile(UserProfile passedUserProfile)
        {
            if (passedUserProfile == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(passedUserProfile.Location))
            {
                passedUserProfile.Location = "Unknown";
            }

            var userProfile = new UserProfile()
            {
                FirstName = passedUserProfile.FirstName,
                LastName = passedUserProfile.LastName,
                IdentityUserId = passedUserProfile.IdentityUserId,
                Location = passedUserProfile.Location
            };

            _userProfilerRepository.Insert(userProfile);
            CreateDefaultUserProfileAttribute(userProfile);

            _unitOfWork.SaveChanges();

            UpdateProfileAttribute(userProfile, passedUserProfile.FirstName, passedUserProfile.LastName);

            return userProfile;
        }

        public  void UpdateProfileAttribute(UserProfile userProfile, string firstName, string lastName)
        {
            var firstNameAttribute =
                            userProfile.UserProfileAttributes.FirstOrDefault(c => c.AttributeDetailId == (int)AttributeDetailId.FirstName);

            if (firstNameAttribute != null)
            {
                firstNameAttribute.Value = firstName;
                firstNameAttribute.ObjectState = ObjectState.Modified;
            }

            var lastNameAttribute =
                userProfile.UserProfileAttributes.FirstOrDefault(c => c.AttributeDetailId == (int)AttributeDetailId.LastName);

            if (lastNameAttribute != null)
            {
                lastNameAttribute.Value = lastName;
                lastNameAttribute.ObjectState = ObjectState.Modified;
            }

            _unitOfWork.SaveChanges();
        }

        public bool IsUserNameValid(string username)
        {
            return !_userIdentityRepository.Queryable().Any(s => s.Email == username);
        }

        public void SaveTrackingAction(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            var trackingAction = new TrackingAction
            {
                TrackingActionId = Guid.NewGuid(),
                Data = data
            };

            _trackingActionRepository.Insert(trackingAction);
            _unitOfWork.SaveChanges();
        }

        public PagingUserDto PageUsers(FilterUserDto filterDto)
        {
            IQueryable<AspNetUser> userDbQuery = _portaliaContext.AspNetUsers.AsQueryable().AsNoTracking();
            //filter by employee status
            if (filterDto.IsEmployee.HasValue)
            {
                userDbQuery = userDbQuery.Where(s => s.IsEmployee == filterDto.IsEmployee.Value);
            }
            // filter by employee name
            if (!string.IsNullOrEmpty(filterDto.SearchUserNameQuery))
            {
                userDbQuery = userDbQuery.Where(s => s.UserName.ToLower().Contains(filterDto.SearchUserNameQuery)
                                         || s.LastName.ToLower().Contains(filterDto.SearchUserNameQuery)
                                         || s.FirstName.ToLower().Contains(filterDto.SearchUserNameQuery));
            }
            // filter by work contract status
            if (filterDto.Status > 0)
            {
                userDbQuery = FilterUsersByWorkContractStatus(userDbQuery, filterDto.Status);
            }
            // join users with user profiles to get response
            var queryPagingUserItem = (from u in userDbQuery
                                       join p in _portaliaContext.UserProfiles
                                       on u.Id equals p.IdentityUserId
                                       select new PagingUserItemDto
                                       {
                                           IsActive = u.IsActive,
                                           IsEmployee = u.IsEmployee,
                                           IsNew = u.IsNew,
                                           Email = u.Email,
                                           Id = u.Id,
                                           FirstName = u.FirstName,
                                           LastName = u.LastName,
                                           Created = u.Created,
                                           PortaliaUserProfileId = p.UserProfileId
                                       })
                .OrderByDescending(s => s.Created);
            List<PagingUserItemDto> pagingUserItemDtos = queryPagingUserItem
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .ToList();
            // Set percent of info and work contract
            SetUsersData(pagingUserItemDtos);
            var countUsers = queryPagingUserItem.Count();
            // Set data to response
            PagingUserDto pagingUserDto = new PagingUserDto
            {
                TotalPages = countUsers % filterDto.PageSize == 0
                ? countUsers / filterDto.PageSize
                : (countUsers / filterDto.PageSize) + 1,
                CurrentPage = filterDto.PageNumber,
                Users = pagingUserItemDtos
            };

            return pagingUserDto;
        }

        public PagingUserItemDto GetPagingUserItem(string userId)
        {
            // Get user base on userId
            var user = _portaliaContext.AspNetUsers.FirstOrDefault(s => s.Id == userId);
            
            // If user not found, return null
            if (user == null)
            {
                return null;
            }

            // Get user profile based on userId
            var userProfileId = _portaliaContext.UserProfiles.FirstOrDefault(s => s.IdentityUserId == user.Id)
                ?.UserProfileId ?? 0;

            // If user profile not found, return null
            if (userProfileId == 0)
            {
                return null;
            }

            // Declare dto object
            var pagingUserItemDto = new PagingUserItemDto
            {
                IsActive = user.IsActive,
                IsEmployee = user.IsEmployee,
                IsNew = user.IsNew,
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Created = user.Created,
                PortaliaUserProfileId = userProfileId
            };

            // Set user data
            SetUserData(pagingUserItemDto);

            // Return data
            return pagingUserItemDto;
        }

        private void CreateDefaultUserProfileAttribute(UserProfile userProfile)
        {
            var attributeDetails = _attributeDetailRepository.Queryable().AsNoTracking().ToList();
            var newUserProfileAttributes = attributeDetails.Select(attributeDetail => new UserProfileAttribute()
            {
                UserProfile = userProfile,
                Value = null,
                AttributeDetailId = attributeDetail.AttributeDetailId
            }).ToList();

            var nationalityField = newUserProfileAttributes.Find(c => c.AttributeDetailId == (int)FieldEnums.Nationality);
            nationalityField.Value = "FR";

            var birthPlace = newUserProfileAttributes.Find(c => c.AttributeDetailId == (int)FieldEnums.BirdPlace);
            birthPlace.Value = "FR";

            var preferedLanguage = newUserProfileAttributes.Find(c => c.AttributeDetailId == (int)FieldEnums.PreferredLanguage);
            preferedLanguage.Value = "fr";

            var currency = newUserProfileAttributes.Find(c => c.AttributeDetailId == (int)FieldEnums.Currency);
            currency.Value = "EUR";

            var system = newUserProfileAttributes.Find(c => c.AttributeDetailId == (int)FieldEnums.System);
            system.Value = "1";

            _userProfileAttributeRepository.InsertRange(newUserProfileAttributes);
        }

        private int GetPerCentOfInfo(string id)
        {
            //Get personal style
            var userManatoryField =
                _portaliaContext.UserProfileAttributes.Where(
                    c => c.UserProfile.IdentityUserId == id && c.AttributeDetail.AttributeTypeId == 1 && c.AttributeDetail.IsRequired).ToList();

            // If there is no mandantory fields, return 0
            if (userManatoryField.Count == 0)
            {
                return 0;
            }

            // Calculate the percentage of completion
            var userFieldHaveNovalue = userManatoryField.Where(c => string.IsNullOrEmpty(c.Value)).ToList();
            var completeField = userManatoryField.Count - userFieldHaveNovalue.Count;
            var percentComplete = (int)Math.Round((double)(100 * completeField) / userManatoryField.Count);

            return percentComplete;
        }

        private void SetUserWorkContract(PagingUserItemDto user)
        {
            // N/A for employees
            if (user.IsEmployee)
            {
                return;
            }

            // Get work contract
            WorkContract userWorkContract =
                (from wc in _portaliaContext.WorkContracts
                join wcs in _portaliaContext.WorkContractStatuses on wc.ContractId equals wcs.ContractId
                where wc.UserId == user.Id
                select wc)
                .FirstOrDefault();

            if (userWorkContract != null && userWorkContract.WorkContractStatuses.Any())
            {
                // Get current status of the work contract
                int statusId = userWorkContract.WorkContractStatuses
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => x.StatusId)
                    .FirstOrDefault();
                user.WorkContractStatusId = (WorkContractStatusEnum)statusId;
                user.WorkContractId = userWorkContract.ContractId;
                user.DocumentId = userWorkContract.DocumentId;
            }
            else
            {
                // Candidates with no work contracts => Disabled by default
                user.WorkContractStatusId = WorkContractStatusEnum.Disabled;
            }
        }

        private void SetUsersData(List<PagingUserItemDto> users)
        {
            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                SetUserData(user);
            }
        }



        private void SetUserData(PagingUserItemDto user)
        {
            if (user == null)
            {
                return;
            }

            user.PercentOfInfo = GetPerCentOfInfo(user.Id);
            SetUserWorkContract(user);
        }

        private IQueryable<AspNetUser> FilterUsersByWorkContractStatus(IQueryable<AspNetUser> users, byte statusId)
        {
            // N/A for employees
            IQueryable<AspNetUser> matchedUsers = users.Where(x => !x.IsEmployee);

            List<int> matchedStatuses = new List<int> { statusId };
            if (statusId == (byte)WorkContractStatusEnum.PendingOnCandidate)
            {
                // When status is PendingOnCandidate, also get PendingOnCandidateRevision
                matchedStatuses.Add((int)WorkContractStatusEnum.PendingOnCandidateRevision);
            }

            // Latest work contract statuses, which matches the filter
            var contractIDsMatchTheStatus =
                (from wcs in _portaliaContext.WorkContractStatuses
                 group wcs by wcs.ContractId into groups
                 select groups.OrderByDescending(x => x.CreatedDate).FirstOrDefault())
                            .Where(x => matchedStatuses.Contains(x.StatusId))
                            .Select(x => x.ContractId);

            // User IDs that match the filtered status
            var userIDsMatchTheStatus =
                (from wc in _portaliaContext.WorkContracts
                 join id in contractIDsMatchTheStatus on wc.ContractId equals id
                 select wc.UserId);

            // Filter users
            if (statusId == (byte)WorkContractStatusEnum.Disabled)
            {
                // Disabled status: get both users with disabled work contracts and
                // users without work contracts
                var userIDsHavingWorkContracts = _portaliaContext.WorkContracts.Select(x => x.UserId);
                matchedUsers = matchedUsers.Where(x => !userIDsHavingWorkContracts.Contains(x.Id) || userIDsMatchTheStatus.Contains(x.Id));
            }
            else
            {
                // Get users match the status
                matchedUsers = matchedUsers.Where(x => userIDsMatchTheStatus.Contains(x.Id));                
            }

            return matchedUsers;
        }
    }
}
