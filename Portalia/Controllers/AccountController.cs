using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using MultiBranding.ApiClient;
using Newtonsoft.Json;
using Portalia.App_Start;
using Portalia.Core.Interface.Service;
using Portalia.Extentions;
using Portalia.Models;
using Portalia.ViewModels;
using Portalia.Resources;
using System.Drawing;
using System.Security.Claims;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Owin.Security.Cookies;
using Portalia.Attributes;
using Portalia.Auth0.Constants;
using Portalia.Auth0.TokenGenerator;
using Portalia.Core.Entity;
using AuthorizationCodeTokenRequest = Auth0.AuthenticationApi.Models.AuthorizationCodeTokenRequest;
using UserProfile = Portalia.Resources.UserProfile;
using Portalia.Core.Enum;
using Portalia.Core.Helpers;

namespace Portalia.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly AuthenticationApiClient _auth0Client;
        private readonly ITokenGenerator _auth0TokenGenerator;
        private readonly IMigrationService _migrationService;

        private readonly IUserProfileService _userProfileService;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IUserProfileAttributeService _profileAttributeService;
        private readonly IProposalService _proposalService;
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AccountController(IUserProfileService userProfileService, 
            ApplicationSignInManager signInManager, 
            ApplicationUserManager userManager,
            IUserProfileAttributeService userProfileAttributeService,
            IProposalService proposalService,
            IMigrationService migrationService,
            AuthenticationApiClient auth0Client,
            ITokenGenerator auth0TokenGenerator)
        {
            _userProfileService = userProfileService;
            _profileAttributeService = userProfileAttributeService;
            _migrationService = migrationService;
            _auth0Client = auth0Client;
            _auth0TokenGenerator = auth0TokenGenerator;
            _proposalService = proposalService;
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IProposalService proposalService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _proposalService = proposalService;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetLoggedUserInfor()
        {
            var model = new LoggedInforViewModel
            {
                IsAuthenticated = Request.IsAuthenticated
            };

            if (model.IsAuthenticated)
            {
                if (User.IsInRole("Administrator"))
                {
                    model.UserRole = Role.Administrator.ToString();
                }
                else if (User.Identity.IsEmployee())
                {
                    model.UserRole = Role.Employee.ToString();
                }
                else
                {
                    model.UserRole = Role.Visitor.ToString();
                }
            }

            model.Links.Add(new LinkViewModel
            {
                Url = Request.Url?.GetLeftPart(UriPartial.Authority) + "/Entreprise",
                Text = HomePage.Company,
                Type = "1"
            });

            if (model.IsAuthenticated)
            {
                model.FirstName = User.Identity.GetFirstname();
                model.Fullname = User.Identity.GetFullname();
                CreateMenuLinksForLoggedUser(model);
                model.PictureUrl = GetUserPicture();
                model.CanSeeWelcomeCards = User.GetCanSeeWelcomeCardsClaim().ConvertToBool();
                model.EmployeeStatus = User.GetEmployeeStatus();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        [ApiOutputCache(Duration = 259200)] // Cache file for 3 days
        public ActionResult Files(string filename)
        {
            var userProfile = _userProfileService.GetUserProfile(User.Identity.GetUserId());
            var isUserAvatarAvailable = userProfile?.PictureFileBinary != null && userProfile.PictureName != null;
            if (isUserAvatarAvailable)
            {
                string base64String = Convert.ToBase64String(userProfile.PictureFileBinary);
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    Response.AppendHeader("Content-Disposition", $"attachment; filename={userProfile.PictureName}");
                    Response.AppendHeader("Last-Modified", DateTime.MinValue.ToUniversalTime().ToString("R"));
                    return File(imageBytes, $"image/{userProfile.PictureName.Split('.')[1]}");
                }
            }
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CloseWelcomeCards()
        {
            var message = new MessageViewModel
            {
                HasError = true,
                Message = "User is not authenticated"
            };

            if (!User.Identity.IsAuthenticated)
            {
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            // If user does not exist, then return error
            if (user == null)
            {
                message.Message = "User does not exist";

                return Json(message, JsonRequestBehavior.AllowGet);
            }

            // We go this far, update CanSeeWelcomeCards property
            user.CanSeeWelcomeCards = false;

            var isUpdateSuccessfully = await UserManager.UpdateAsync(user);

            if (!isUpdateSuccessfully.Succeeded)
            {
                message.Message = "Cannot close welcome cards";

                return Json(message, JsonRequestBehavior.AllowGet);
            }

            UpdateCanSeeWelcomeCardsClaimValue(false);

            message.HasError = false;
            message.Message = "Close card successfully";

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmployeeEmailDomains = Auth0Constants.AdfsDomains;
            ViewBag.AdfsConnection = Auth0Constants.AdfsConnection;
            ViewBag.LinkedInConnection = Auth0Constants.LinkedInConnection;
            return View(new LoginViewModel());
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.EmployeeEmailDomains = Auth0Constants.AdfsDomains;
                ViewBag.AdfsConnection = Auth0Constants.AdfsConnection;
                ViewBag.LinkedInConnection = Auth0Constants.LinkedInConnection;
                return View(model);
            }

            try
            {
                var result = await _auth0Client.GetTokenAsync(new ResourceOwnerTokenRequest
                {
                    ClientId = Auth0Constants.ClientId,
                    ClientSecret = Auth0Constants.ClientSecret,
                    Scope = Auth0Constants.Scopes,
                    Realm = Auth0Constants.Connection,
                    Username = model.Email,
                    Password = model.Password
                });

                await LoginFromAccessToken(result.AccessToken);

                return RedirectToLocal(returnUrl);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Password", e.Message);
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmployeeEmailDomains = Auth0Constants.AdfsDomains;
            ViewBag.AdfsConnection = Auth0Constants.AdfsConnection;
            ViewBag.LinkedInConnection = Auth0Constants.LinkedInConnection;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoginExternal(LoginViewModel model)
        {
            throw new NotSupportedException();
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result =
                await
                    SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                        model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Code invalide.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
                    CanSeeWelcomeCards = true,
                    HasChangedPassword = true,
                    Id = authUser.UserId
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                var userProfile = _userProfileService.CreateUserProfile(new Core.Entity.UserProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Location = model.Location
                });
                if (result.Succeeded && userProfile != null)
                {
                    // Save user consent
                    _userProfileService.SaveTrackingAction(Request.GetRequestInfoInJson(user.UserName, user.Id));

                    // Sign user into cookie middleware
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, CreateIdentity(authUser, user));

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                    //    Request.Url.Scheme);
                    //await this.UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    CreateCandidate(user, userProfile, model.Gender);

                    return RedirectToAction("MySpace", "Proposal", new { userId = user.Id });
                }

                AddErrors(result);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                LoggerHelpers.Error(e);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IsUserNameValid(string username)
        {
            var response = new MessageViewModel
            {
                HasError = true,
                Message = "Invalid data"
            };

            if (string.IsNullOrEmpty(username))
            {
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            var isValid = _userProfileService.IsUserNameValid(username.Trim());

            response.HasError = !isValid;
            response.Message = isValid ? string.Empty : $"L'adresse de messagerie '{username.Trim()}' est déjà utilisée.";

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult DisplayEmail()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.EmployeeEmailDomains = Auth0Constants.AdfsDomains;
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewBag.EmployeeEmailDomains = Auth0Constants.AdfsDomains;
            if (ModelState.IsValid)
            {
                var user = UserManager.Users.SingleOrDefault(x => x.Email == model.Email && x.PasswordHash != null);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.Message = "Le courrier électronique n'existe pas";
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code },
                    Request.Url.Scheme);
                //await this.UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
                MultiBrandingHelper.SendEmail(new SendEmailParameter
                {
                    ApplicationName = "Multibranding",
                    Holding = "Portalia",
                    TemplateName = "ContactUs",
                    Variable = JsonConvert.SerializeObject(new
                    {
                        EmailContent = $"<p>Bonjour,</p>" +
                                       $"<p>Cliquez sur ce <a href=\"{callbackUrl}\">lien</a> pour réinitialiser votre mot de passe.</p>" +
                                       "<p>L'équipe Portalia</p>",
                        Subject = $"[Portalia] Mot de passe oublié",
                        SendFrom = ConfigurationManager.AppSettings["MailFrom"],
                        SendTo = user.Email
                    })
                });
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId))
            {
                return View("Error");
            }

            var viewModel = new ResetPasswordViewModel
            {
                UserId = userId,
                Code = code
            };

            return View(viewModel);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                AddErrorsToModelState(ValidationMessage.UserNotSpecifiedMessage);
                return View(model);
            }

            if (user.Email != model.Email)
            {
                AddErrorsToModelState(ValidationMessage.EmailNotMatchedMessage);
                return View(model);
            }

            var passwordValidationMessages = _userProfileService.GetPasswordValidationMessage(model.NewPassword);

            // Password is not valid
            if (passwordValidationMessages.HasError)
            {
                AddValidationMessagesFromPasswordToModelState(passwordValidationMessages);

                return View(model);
            }

            try
            {
                var client = new ManagementApiClient(_auth0TokenGenerator.GetManagementToken(), new Uri($"{Auth0Constants.BaseUrl}api/v2"));

                await client.Users.UpdateAsync(model.UserId, new UserUpdateRequest
                {
                    ClientId = Auth0Constants.ClientId,
                    Connection = Auth0Constants.Connection,
                    Password = model.NewPassword
                });

                user.HasChangedPassword = true;

                await UserManager.UpdateAsync(user);

                UpdateHasChangedPasswordClaimValue(true);

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                _logger.Debug("ResetPassword");
                _logger.Debug(e);
            }

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions =
                userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return
                View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode",
                new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl = "", string email = "")
        {
            if (string.IsNullOrWhiteSpace(provider))
                return RedirectToAction("Login");

            if (provider == Auth0Constants.AdfsConnection)
            {
                var lowerEmail = email.ToLower();
                if (!Auth0Constants.AdfsDomains.Split(',').Any(d => lowerEmail.EndsWith(d.ToLower())))
                    return RedirectToAction("Login");
            }

            var redirectUri = GetExternalLoginCallbackUrl(returnUrl, provider);

            var authUrl =
                $"{Auth0Constants.BaseUrl}authorize?response_type=code&scope={Auth0Constants.Scopes}&client_id={Auth0Constants.ClientId}&connection={provider}&redirect_uri={redirectUri}";
            return Redirect(authUrl);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string provider, string code, string returnUrl = "")
        {
            //use the code to get the access and refresh token
            var redirectUri = GetExternalLoginCallbackUrl(returnUrl, provider);

            var tokenResponse = await _auth0Client.GetTokenAsync(new AuthorizationCodeTokenRequest
            {
                ClientId = Auth0Constants.ClientId,
                ClientSecret = Auth0Constants.ClientSecret,
                Code = code,
                RedirectUri = redirectUri
            });

            var token = tokenResponse.AccessToken;

            await LoginFromAccessToken(token);

            return RedirectToLocal(returnUrl);
        }

        private string GetExternalLoginCallbackUrl(string returnUrl, string provider)
        {
            return Url.Action(
                "ExternalLoginCallback",
                "Account",
                new { returnUrl, provider },
                "https");
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var service = UnityConfig.GetConfiguredContainer().Resolve<ICreateUserProfileBasedOnExternalLogin>(info.Login.LoginProvider);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var userProfile = service.CreateUserProfile(user.Id, info.ExternalIdentity.Claims.ToList());
                user.LastName = userProfile.LastName;
                user.FirstName = userProfile.FirstName;

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GET: /Account/LogOff?returnUrl=
        /// </summary>
        /// <param name="returnUrl">the url that is used to redirect to</param>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            }

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");
            return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public ActionResult ChangePasswordForNewPolicy()
        {
            if (User.Identity.HasChangedPassword() || !User.Identity.CanChangePassword())
            {
                return RedirectToAction("MySpace", "Proposal");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePasswordForNewPolicy(ChangePasswordForNewPolicyViewModel viewModel)
        {
            if (User.Identity.HasChangedPassword() || !User.Identity.CanChangePassword())
            {
                return RedirectToAction("MySpace", "Proposal");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var passwordValidationMessages = _userProfileService.GetPasswordValidationMessage(viewModel.NewPassword);

            // If password is not valid, then return error
            if (passwordValidationMessages.HasError)
            {
                AddValidationMessagesFromPasswordToModelState(passwordValidationMessages);
                return View(viewModel);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            // If user does not exist, then return error
            if (user == null)
            {
                AddErrorsToModelState("L'utilisateur n'existe pas"); // User does not exist
                return View(viewModel);
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
                    Password = viewModel.OldPassword
                });
            }
            catch (Exception e)
            {
                // If current password and user input password are not matched, then return error
                AddErrorsToModelState("Vous avez entré un mauvais mot de passe actuel");
                return View(viewModel);
            }

            try
            {
                var client = new ManagementApiClient(_auth0TokenGenerator.GetManagementToken(),
                    new Uri($"{Auth0Constants.BaseUrl}api/v2"));

                await client.Users.UpdateAsync(User.Identity.GetUserId(), new UserUpdateRequest
                {
                    ClientId = Auth0Constants.ClientId,
                    Connection = Auth0Constants.Connection,
                    Password = viewModel.NewPassword
                });

                user.HasChangedPassword = true;

                var isUpdateSuccessfully = await UserManager.UpdateAsync(user);

                if (isUpdateSuccessfully.Succeeded)
                {
                    UpdateHasChangedPasswordClaimValue(true);
                    return RedirectToAction("MySpace", "Proposal");
                }
                else
                {
                    AddErrorsToModelState("Échec de la mise à jour du mot de passe"); // Fail to update password
                }
            }
            catch (Exception e)
            {
                AddErrorsToModelState("Échec de la mise à jour du mot de passe"); // Fail to update password
            }
            return View(viewModel);
        }



        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("MySpace", "Proposal");
            //return RedirectToDefaultMySpace();
        }

        private ActionResult RedirectToDefaultMySpace()
        {
            var userId = User.Identity.GetUserId();
            var proposalId = _proposalService.GetById(userId)?.ProposalId;
            var defaultFolderType = FolderType.MyProject;

            return RedirectToAction("Documents", "Proposal", new
            {
                proposalId = proposalId,
                folderType = defaultFolderType,
                userId = userId
            });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }



        /// <summary>
        /// Create menu links when user has logged-in
        /// </summary>
        /// <param name="model">LoggedInforViewModel object</param>
        private void CreateMenuLinksForLoggedUser(LoggedInforViewModel model)
        {
            List<LinkViewModel> lstLinks = new List<LinkViewModel>();

            //My user profile
            lstLinks.Add(new LinkViewModel()
            {
                Url = Url.Action("Index", "Manage"),
                Text = HomePage.MyUserProfile,
                Type = "2",
                MissingFieldCounting = _profileAttributeService.CountMissingField(User, UserProfile.ResourceManager)
            });

            if (User.GetCanChangePasswordClaim())
                // change pass
                lstLinks.Add(new LinkViewModel()
                {
                    Url = Url.Action("ChangePassword", "Manage"),
                    Text = HomePage.ChangePassword,
                    Type = "2"
                });

            //About
            lstLinks.Add(new LinkViewModel()
            {
                Url = "https://www.portalia.fr/a-propos",
                Text = HomePage.About,
                Type = "2"
            });

            //Sign-out
            lstLinks.Add(new LinkViewModel()
            {
                Url = Url.Action("LogOff", "Account"),
                Text = Resources.Login.SignOut,
                Type = "2"
            });

            model.Links.AddRange(lstLinks);
        }

        /// <summary>
        /// Get user profile picture
        /// </summary>
        /// <returns>Path of image or data of image as Base64</returns>
        public string GetUserPicture()
        {
            var userProfile = _userProfileService.GetUserProfile(User.Identity.GetUserId());
            //user has avatar -> get by custom url
            if (userProfile?.PictureFileBinary != null && userProfile.PictureName != null)
            {
                return $"portalia/account/files/{userProfile.PictureName}" + "?v=" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            }
            return "https://portalia.fr/Portalia/Content/images/default-avatar-200x200.jpg";
        }

        private void AddErrorsToModelState(string error)
        {
            ModelState.AddModelError("", error);
        }

        private void UpdateHasChangedPasswordClaimValue(bool value)
        {
            User.UpdateHasChangedPasswordClaimValue(value.ToString());
        }

        private void UpdateCanSeeWelcomeCardsClaimValue(bool value)
        {
            User.UpdateCanSeeWelcomeCardsClaimValue(value.ToString());
        }

        private void UpdateEmployeeStatusValue(string value)
        {
            User.UpdateEmployeeStatusClaimValue(value);
        }

        private async Task LoginFromAccessToken(string accessToken, bool rememberMe = false)
        {
            // Get user info from token
            var user = await _auth0Client.GetUserInfoAsync(accessToken);

            var appUser = await UserManager.FindByIdAsync(user.UserId);
            if (appUser == null && (user.IsUsernamePassword() || user.IsFromAdfs()))
            {
                appUser = await UserManager.FindByNameAsync(user.Email);
                if (appUser != null)
                {
                    _migrationService.Create(new Migration
                    {
                        OldAspNetUserId = appUser.Id,
                        NewAuth0Id = user.UserId
                    });
                    appUser.Id = appUser.UserName = user.UserId;
                }
            }

            // Create user if it doen't exit on login (ADFS / LinkedIn)
            if (appUser == null)
            {
                appUser = new ApplicationUser
                {
                    UserName = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CanSeeWelcomeCards = true,
                    HasChangedPassword = true,
                    Id = user.UserId
                };

                var result = await UserManager.CreateAsync(appUser);
                var userProfile = _userProfileService.CreateUserProfile(new Core.Entity.UserProfile
                {
                    IdentityUserId = appUser.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Location = "Unknown"
                });

                if (!result.Succeeded)
                    _logger.Error($"Unable to create the user for '{user.UserId}'");

                if (userProfile == null)
                    _logger.Error($"Unable to create profile for user Id:{appUser.Id} Email:{appUser.Email}");

                if (user.IsFromLinkedIn())
                    CreateCandidate(appUser, userProfile);
            }

            // Sign user into cookie middleware
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, CreateIdentity(user, appUser));

            MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
            var employeeStatusResult = MultiBrandingHelper.IsEmployeeExisted(user.Email);
            if (employeeStatusResult != null)
            {
                _userProfileService.UpdateEmployeeStatus(user.Email, employeeStatusResult.Success);
            }
            else
            {
                _logger.Error("Can't call api check IsEmployeeExisted");
            }
            if (!string.IsNullOrEmpty(User.Identity.EmployeeStatus()))
            {
                _userProfileService.UpdateEmployeeStatus(user.Email, User.Identity.EmployeeStatus());
            }
        }

        private ClaimsIdentity CreateIdentity(UserInfo authUser, ApplicationUser appUser)
        {
            var isEmployeeQueryResult = MultiBrandingHelper.IsEmployeeExisted(authUser.Email);
            var isEmployee = isEmployeeQueryResult?.Success ?? false;

            var employeeStatusQueryResult = MultiBrandingHelper.GetEmployeeStatus(authUser.Email, holdingId: 13);
            var employeeStatus = employeeStatusQueryResult?.Data?.ToString() ?? string.Empty;

            var canChangePassword = authUser.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] != null &&
                                    (string)authUser.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] == Auth0Constants.Auth0Connection;
            var isAdfsUser = authUser.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] != null &&
                             (string)authUser.AdditionalClaims[Auth0Constants.ConnectionTypeClaim] == Auth0Constants.AdfsConnection.ToLower();

            // Create claims principal
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                // Auth claims
                new Claim(ClaimTypes.NameIdentifier, authUser.UserId, "http://www.w3.org/2001/XMLSchema#string", Auth0Constants.BaseUrl),
                new Claim(ClaimTypes.Name, authUser.UserId, "http://www.w3.org/2001/XMLSchema#string", Auth0Constants.BaseUrl),
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", Auth0Constants.BaseUrl, "http://www.w3.org/2001/XMLSchema#string", Auth0Constants.BaseUrl),

                // User info claims
                new Claim(ClaimTypes.GivenName, appUser.FirstName),
                new Claim(ClaimTypes.Surname, appUser.LastName),
                new Claim(ClaimTypes.Email, authUser.Email),

                // App specific claims
                new Claim("CanChangePassword", canChangePassword.ToString()),
                new Claim("IsAdfsUser", isAdfsUser.ToString()),
                new Claim("HasChangedPassword", (!canChangePassword || appUser.HasChangedPassword).ToString()),
                new Claim("CanSeeWelcomeCards", appUser.CanSeeWelcomeCards.ToString()),
                new Claim("IsEmployee", isEmployee.ToString()),
                new Claim("EmployeeStatus", employeeStatus)
            }, CookieAuthenticationDefaults.AuthenticationType, ClaimTypes.NameIdentifier, ClaimTypes.Role);

            var roles = authUser.AdditionalClaims[ClaimTypes.Role].ToList();
            foreach (var role in roles.Select(r => (string)r))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return claimsIdentity;
        }

        private void CreateCandidate(ApplicationUser appUser, Core.Entity.UserProfile userProfile, int gender = 1)
        {
            MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
            var createCandidateResult = MultiBrandingHelper.CreateCandidate(new CreateCandidateParameter()
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                Status = CandidateStatus.Unqualified,
                SourceId = CandidateSource.Application,
                SourceCountryId = "FR",
                HoldingId = 13,
                IsPotentialManager = false,
                Gender = gender == 1 ? Gender.Male : Gender.Female
            });

            if (createCandidateResult == null)
            {
                _logger.Error("can't call api CreateCandidate");
            }
            else
            {
                _logger.Info($"create candidate: {appUser.FirstName} {appUser.LastName} result:{JsonConvert.SerializeObject(createCandidateResult)}");
            }

            _profileAttributeService.UpdateUserProfileAttribute(userProfile.UserProfileId, "Gender", gender.ToString());

            var isSendMail = bool.Parse(ConfigurationManager.AppSettings["IsSendMail"]);
            _userProfileService.UpdateStatusForNewUser(appUser.Id, true);
            if (isSendMail)
            {
                //MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
                var resultSendMail = MultiBrandingHelper.SendEmail(new SendEmailParameter()
                {
                    ApplicationName = "Multibranding",
                    Holding = "Portalia",
                    TemplateName = "ContactUs",
                    Variable = JsonConvert.SerializeObject(new
                    {
                        EmailContent = $"<p>Hello,</p>" +
                               $"<p>We have a new comer [{appUser.Email} - {appUser.FirstName} {appUser.LastName}] on Portalia, please connect to https://portalia.fr/Administrator</p>" +
                               $"<p>Portalia Team</p>",
                        Subject = $"[Portalia] New candidate {User.Identity.GetUserName()}",
                        SendFrom = ConfigurationManager.AppSettings["MailFrom"],
                        SendTo = ConfigurationManager.AppSettings["HrEmails"]
                    })
                });
                if (resultSendMail == null)
                {
                    _logger.Error("can't call api SendEmail for HR");
                }
                else
                {
                    _logger.Info($"send mail for hr result:{JsonConvert.SerializeObject(resultSendMail)}");
                }
                var resultSendMailForUser = MultiBrandingHelper.SendEmail(new SendEmailParameter()
                {
                    ApplicationName = "Multibranding",
                    Holding = "Portalia",
                    TemplateName = "PortaliaWebsiteAccountConfirmation",
                    Variable = JsonConvert.SerializeObject(new
                    {
                        EmailContent = StringExtentions.RenderRazorViewToString(Server.MapPath("~/views/emails/NewCandidateEmailTemplate.cshtml"), new RegisterViewModel
                        {
                            FirstName = appUser.FirstName,
                            LastName = appUser.LastName
                        }),
                        Subject = "[Portalia] Welcome to Portalia",
                        SendFrom = ConfigurationManager.AppSettings["MailFrom"],
                        SendTo = appUser.Email
                    })
                });
                if (resultSendMailForUser == null)
                {
                    _logger.Error("can't call api SendEmail For User");
                }
                else
                {
                    _logger.Info($"send mail to user result:{JsonConvert.SerializeObject(resultSendMailForUser)}");
                }
            }
        }
    }
}