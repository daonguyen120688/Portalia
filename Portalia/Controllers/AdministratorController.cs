using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using MultiBranding.ApiClient;
using Portalia.Auth0.Constants;
using Portalia.Core.Entity;
using Portalia.Core.Extensions;
using Portalia.Core.Interface.Service;
using Portalia.Extentions;
using Portalia.Models;
using Portalia.Repository;
using Portalia.ViewModels;
using Portalia.ViewModels.Users;
using Repository.Pattern.Infrastructure;

namespace Portalia.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : BaseController
    {
        private readonly AuthenticationApiClient _auth0Client;
        private readonly ApplicationUserManager _userManager;
        private readonly IUserProfileService _userProfileService;
        private PortaliaContext _portaliaContext;
        public AdministratorController(ApplicationUserManager userManager, IUserProfileService userProfileService, PortaliaContext portaliaContext, AuthenticationApiClient auth0Client)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
            _portaliaContext = portaliaContext;
            _auth0Client = auth0Client;
        }

        public ActionResult Index(FilterUserViewModel filterUserViewModel)
        {
            var pagingUserDto = _userProfileService.PageUsers(filterUserViewModel.ToFilterUserDto());
            var pagingUserViewModel = pagingUserDto.ToPagingUserViewModel();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_PagingUsers", pagingUserViewModel);
            }

            // Get work contract statues for drop down list
            pagingUserViewModel = pagingUserViewModel ?? new PagingUserViewModel();
            pagingUserViewModel.WorkContractStatuses = EnumExtensions.GetWorkContractStatuses()
                .Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value });

            return View(pagingUserViewModel);
        }

        public ActionResult UpdateUserProfileIndex(string userId)
        {
            var userIdentity = _userProfileService.GetUsersIdentityById(userId);
            var users = new EditUserIdentityProfile
            {
                UserId = userId,
                FirstName = userIdentity.FirstName,
                LastName = userIdentity.LastName
            };
            return View(users);
        }

        [HttpPost]
        public ActionResult UpdateUserProfile(EditUserIdentityProfile model)
        {
            if (ModelState.IsValid)
            {
                var userIdentity = new AspNetUser
                {
                    Id = model.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                _userProfileService.UpdateProfileIdentity(userIdentity);
            }
            return RedirectToAction("UpdateUserProfileIndex", new { userId = model.UserId });
        }
        public ActionResult DeActiveUser(string userId, int? page)
        {
            _userProfileService.DeActiveUser(userId);
            return RedirectToAction("Index", new { page });
        }

        //[HttpGet]
        //public ActionResult CreateUserIdentityIndex()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<ActionResult> CreateUserIdentityUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var registerResult = await _auth0Client.SignupUserAsync(new SignupUserRequest
                    {
                        ClientId = Auth0Constants.ClientId,
                        Connection = Auth0Constants.Connection,
                        Email = model.Email,
                        Password = model.Password,
                        UserMetadata = new
                        {
                            given_name = model.FirstName,
                            family_name = model.LastName
                        }
                    });

                    var authResult = await _auth0Client.GetTokenAsync(new ResourceOwnerTokenRequest
                    {
                        ClientId = Auth0Constants.ClientId,
                        ClientSecret = Auth0Constants.ClientSecret,
                        Scope = "openid profile email",
                        Realm = Auth0Constants.Connection,
                        Username = model.Email,
                        Password = model.Password
                    });

                    // Get user info from token
                    var authUser = await _auth0Client.GetUserInfoAsync(authResult.AccessToken);

                    var user = new ApplicationUser
                    {
                        UserName = authUser.UserId,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Created = DateTime.Now
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    var userProfile = _userProfileService.CreateUserProfile(new Core.Entity.UserProfile
                    {
                        IdentityUserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });

                    if (result.Succeeded && userProfile != null)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                }

                catch (Exception e)
                {
                }

                return RedirectToAction("CreateUserIdentityIndex");
            }
            return RedirectToAction("CreateUserIdentityIndex");
        }

        [HttpGet]
        public ActionResult SearchIdentityUser(string mailAddress, int? page)
        {
            var users = _userProfileService.GetUsersIdentity(mailAddress, page);
            var pagingViewModel = new PagingViewModel<AspNetUser>
            {
                CurrenPage = page,
                Models = users
            };
            return View("Index");
        }
        [AllowAnonymous]
        public ActionResult AddStatusForAllUser()
        {
            var users = _portaliaContext.AspNetUsers.Where(c => c.IsActive);
            try
            {
                foreach (var aspNetUser in users)
                {
                    MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
                    var employeeStatusResult = MultiBrandingHelper.IsEmployeeExisted(aspNetUser.Email);
                    if (employeeStatusResult != null)
                    {
                        aspNetUser.IsEmployee = employeeStatusResult.Success;
                        aspNetUser.ObjectState = ObjectState.Modified;
                    }
                }
                _portaliaContext.SaveChanges();
                return Content("ok");
            }
            catch (Exception exception)
            {
                return Content(exception.ToString());
            }
        }

        public ActionResult ChangePasswordForUser(string userName)
        {
            //TODO Team Portalia => It won't work now, what are you trying to achieve here? We can help you so tell me :) (AME04)
            _userProfileService.ChangePasswordForUser(userName);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestAPI(string email)
        {
            MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
            var employeeStatusResult = MultiBrandingHelper.IsEmployeeExisted(email);
            return Json(employeeStatusResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveWorkContract()
        {
            return View();
        }
    }
}