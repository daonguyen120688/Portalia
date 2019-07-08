using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Portalia.Auth0.Constants;
using Portalia.Auth0.TokenGenerator;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using Portalia.Models;
using Portalia.ViewModels;

namespace Portalia.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly AuthenticationApiClient _auth0Client;
        private readonly ITokenGenerator _auth0TokenGenerator;

        private readonly IUserProfileService _profileService;
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IUserProfileAttributeService _profileAttributeService;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ManageController(IUserProfileService profileService, IUserProfileAttributeService profileAttributeService, ApplicationSignInManager signInManager, ApplicationUserManager userManager,
            AuthenticationApiClient auth0Client,
            ITokenGenerator auth0TokenGenerator)
        {
            _profileService = profileService;
            _profileAttributeService = profileAttributeService;
            _signInManager = signInManager;
            _userManager = userManager;
            _auth0Client = auth0Client;
            _auth0TokenGenerator = auth0TokenGenerator;
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message, string userProfileId = null)
        {
            try
            {
                ViewBag.StatusMessage =
                        message == ManageMessageId.ChangePasswordSuccess
                            ? "votre mot de passe a été changé."
                            : message == ManageMessageId.SetPasswordSuccess
                                ? "Votre mot de passe a été défini."
                                : message == ManageMessageId.SetTwoFactorSuccess
                                    ? "Votre fournisseur d'authentification à deux facteurs a été configuré."
                                    : message == ManageMessageId.Error
                                        ? "Une erreur est survenue."
                                        : message == ManageMessageId.AddPhoneSuccess
                                            ? "Votre numéro de téléphone a été ajouté."
                                            : message == ManageMessageId.RemovePhoneSuccess
                                                ? "Votre numéro de téléphone a été supprimé.."
                                                : "";

                UserProfile userProfile;
                if (User.IsInRole(Roles.Administrator.ToString()) && !userProfileId.IsNullOrWhiteSpace())
                {
                    userProfile = _profileService.GetUserProfile(userProfileId);
                    _profileService.UpdateStatusForNewUser(userProfileId, false);
                }
                else
                {
                    userProfileId = User.Identity.GetUserId();
                    userProfile = _profileService.GetUserProfile(userProfileId);
                }

            var model = new IndexViewModel
            {
                HasPassword = User.GetCanChangePasswordClaim(),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(userProfile.IdentityUserId),
                TwoFactor = await _userManager.GetTwoFactorEnabledAsync(userProfile.IdentityUserId),
                Logins = await _userManager.GetLoginsAsync(userProfile.IdentityUserId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userProfile.IdentityUserId),
                LastName = userProfile.LastName,
                FirstName = userProfile.FirstName,
                AttributeTypes = _profileAttributeService.GetAttributeTypesByUserProfileId(userProfile.UserProfileId),
                UserId = userProfile.UserProfileId,
                IdentityUserId = User.Identity.GetUserId()
            };

                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result =
                await
                    _userManager.RemoveLoginAsync(User.Identity.GetUserId(),
                        new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, false, false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (_userManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await _userManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await _userManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await _signInManager.SignInAsync(user, false, false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await _userManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await _signInManager.SignInAsync(user, false, false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null
                ? View("Error")
                : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result =
                await _userManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, false, false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await _userManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await _signInManager.SignInAsync(user, false, false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            if (!User.GetCanChangePasswordClaim())
                return RedirectToAction("MySpace", "Proposal");
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.GetCanChangePasswordClaim())
                return RedirectToAction("MySpace", "Proposal");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var passwordValidationMessages = _profileService.GetPasswordValidationMessage(model.NewPassword);

            // If password is not valid, then return error
            if (passwordValidationMessages.HasError)
            {
                AddValidationMessagesFromPasswordToModelState(passwordValidationMessages);
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            // If user does not exist, then return error
            if (user == null)
            {
                AddErrorsToModelState("L'utilisateur n'existe pas"); // User does not exist
                return View(model);
            }

            try
            {
                var resultAuth = await _auth0Client.GetTokenAsync(new ResourceOwnerTokenRequest
                {
                    Audience = "erpapi",
                    ClientId = Auth0Constants.ClientId,
                    ClientSecret = Auth0Constants.ClientSecret,
                    Scope = "openid profile email",
                    Realm = Auth0Constants.Connection,
                    Username = user.Email,
                    Password = model.OldPassword
                });
            }
            catch (Exception e)
            {
                // If current password and user input password are not matched, then return error
                AddErrorsToModelState("Vous avez entré un mauvais mot de passe actuel");
                return View(model);
            }

            try
            {
                var client = new ManagementApiClient(_auth0TokenGenerator.GetManagementToken(), new Uri($"{Auth0Constants.BaseUrl}api/v2"));

                await client.Users.UpdateAsync(User.Identity.GetUserId(), new UserUpdateRequest
                {
                    ClientId = Auth0Constants.ClientId,
                    Connection = Auth0Constants.Connection,
                    Password = model.NewPassword
                });

                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, false, false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess
                    ? "The external login was removed."
                    : message == ManageMessageId.Error
                        ? "An error has occurred."
                        : "";
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins =
                AuthenticationManager.GetExternalAuthenticationTypes()
                    .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                    .ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"),
                User.Identity.GetUserId());
        }

        [HttpPost]
        public ActionResult UpdateUserProfile(int pk, string name, string value)
        {
            try
            {
                if (User.IsInRole(Roles.Administrator.ToString()) || _profileAttributeService.HaveUserProfileAttribute(pk, User.Identity.GetUserId()))
                {
                    _profileAttributeService.UpdateAttribute(pk, name, value);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public ActionResult UploadUserProfile(int userId, int attributeDetailId, int userProfileAttributeId, string attributeName)
        {
            var userProfileUpdateFileViewModel = new UserProfileUploadFileViewModel()
            {
                UserId = userId,
                AttributeDetailId = attributeDetailId,
                UserProfileAttributeId = userProfileAttributeId,
                AttributeName = attributeName
            };
            return PartialView("UploadUserProfileFile", userProfileUpdateFileViewModel);
        }

        public ActionResult UploadUserPicture(string userId)
        {
            return PartialView("UploadUserPicture", userId);
        }

        public ActionResult GetUserPicture(string userId = null)
        {
            // Get current user ID if not provided
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = User.Identity.GetUserId();
            }

            var userProfile = _profileService.GetUserProfile(userId);
            if (userProfile?.PictureFileBinary != null && userProfile.PictureName != null)
            {
                string base64String = Convert.ToBase64String(userProfile.PictureFileBinary);
                return Content($"data:image/{userProfile.PictureName.Split('.')[1]};base64,{base64String}");
            }

            var userGender =
                _profileAttributeService.GetUserProfileByUserIdAndAttributeName(userId, "Gender");
            if (userGender == null)
            {
                return Content("/Content/images/default-avatar-200x200.jpg");
            }
            return Content(userGender == "1" ? "https://portalia.fr/Portalia/Content/images/default-avatar-200x200.jpg" : "https://portalia.fr/Portalia/Content/images/avatar-woman.png");
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded
                ? RedirectToAction("ManageLogins")
                : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        public ActionResult Download(int userProfileAttributeId, string userId)
        {
            UserProfileAttribute userProfileDocument;
            if (User.IsInRole(Roles.Administrator.ToString()) && !userId.IsNullOrWhiteSpace())
            {
                userProfileDocument = _profileAttributeService.GetUserProfileDocument(userProfileAttributeId,
              userId);
            }
            else
            {
                userProfileDocument = _profileAttributeService.GetUserProfileDocument(userProfileAttributeId,
              User.Identity.GetUserId());
            }

            if (userProfileDocument == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return File(userProfileDocument.FileBinary, System.Net.Mime.MediaTypeNames.Application.Octet, userProfileDocument.Value);
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrorsToModelState(string error)
        {
            ModelState.AddModelError("", error);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                AddErrorsToModelState(error);
            }
        }

        private bool HasPhoneNumber()
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

    }
}