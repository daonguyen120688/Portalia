using System.Collections.Generic;
using PagedList;
using Portalia.Core.Dtos.User;
using Portalia.Core.Entity;

namespace Portalia.Core.Interface.Service
{
    public interface IUserProfileService
    {
        UserProfile GetUserProfile(string userId);
        IPagedList<AspNetUser> GetUsersIdentity(int? page);
        IPagedList<AspNetUser> GetUsersIdentity(string emailAddress, int? page);
        AspNetUser GetUsersIdentityById(string id);
        void DeActiveUser(string userId);
        List<AspNetUser> GetUsersIdentityForApi();
        void UpdateProfileIdentity(AspNetUser model);
        void UpdateUserProfile(UserProfile userProfile);
        void UpdateEmployeeStatus(string email, bool status);
        void UpdateEmployeeStatus(string email, string status);
        List<AspNetUserExtraInfo> GetUsersIdentity();
        void UpdateStatusForNewUser(string userId, bool status);
        void ChangePasswordForUser(string userName);
        List<AspNetUserExtraInfo> GetUsersIdentityByUserName(string email);
        int GetUserProfileIdByUserIdentityId(string userIdentityId);
        PagedList<AspNetUserExtraInfo> GetUsersIdentity(int page, int size,bool? isEmployee = null);
        AspNetUserExtraInfo GetUsersIdentityByEmail(string email);
        UserProfile CreateUserProfile(UserProfile passedUserProfile);
        bool IsUserNameValid(string username);
        AspNetUserExtraInfo GetUserById(int userId);
        PasswordValidationMessageDto GetPasswordValidationMessage(string password);
        void SaveTrackingAction(string data);
        PagingUserDto PageUsers(FilterUserDto filterDto);
        PagingUserItemDto GetPagingUserItem(string userId);
        void UpdateProfileAttribute(UserProfile userProfile, string firstName, string lastName);
    }
}
