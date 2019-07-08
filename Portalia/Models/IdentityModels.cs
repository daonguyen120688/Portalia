using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MultiBranding.ApiClient;
using Portalia.Core.Interface.Service;

namespace Portalia.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {        
        public ApplicationUser()
        {
            IsActive = true;
            Created = DateTime.Now;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }
        public bool CanSeeWelcomeCards { get; set; }
        public bool HasChangedPassword { get; set; }
        public string EmployeeStatus { get; set; }
    }

    public static class IdentityExtension
    {
        public static string GetFullname(this IIdentity identity)
        {
            var firstName = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.GivenName);
            var lastName = ((ClaimsIdentity) identity).FindFirst(ClaimTypes.Surname);
            // Test for null to avoid issues during local testing
            return (firstName != null && lastName != null) ? $"{firstName.Value} {lastName.Value}" : string.Empty;
        }

        public static string GetFirstname(this IIdentity identity)
        {
            var firstName = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.GivenName);
            // Test for null to avoid issues during local testing
            return (firstName != null) ? firstName.Value : string.Empty;
        }
        
        public static bool IsEmployee(this IIdentity identity)
        {
            try
            {
                var value = ((ClaimsIdentity)identity).FindFirst("IsEmployee").Value;
                return bool.Parse(value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CanChangePassword(this IIdentity identity)
        {
            try
            {
                var hasChangedPassword = ((ClaimsIdentity)identity).FindFirst("CanChangePassword").Value;
                // Test for null to avoid issues during local testing
                return bool.Parse(hasChangedPassword);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static bool HasChangedPassword(this IIdentity identity)
        {
            try
            {
                var hasChangedPassword = ((ClaimsIdentity)identity).FindFirst("HasChangedPassword").Value;
                // Test for null to avoid issues during local testing
                return bool.Parse(hasChangedPassword);
            }
            catch (Exception e)
            {
                return false;
            }
          
        }

        public static bool CanSeeWelcomeCards(this IIdentity identity)
        {
            try
            {
                var canSeeWelcomeCards = ((ClaimsIdentity)identity).FindFirst("CanSeeWelcomeCards").Value;
                // Test for null to avoid issues during local testing
                return bool.Parse(canSeeWelcomeCards);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string EmployeeStatus(this IIdentity identity)
        {
            try
            {
                var EmployeeStatus = ((ClaimsIdentity)identity).FindFirst("EmployeeStatus").Value;
                // Test for null to avoid issues during local testing
                return EmployeeStatus;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public static bool HaveProposal(this IIdentity identity)
        {
            var userService = DependencyResolver.Current.GetService<IProposalService>();
            var haveProposal = userService.GetByUser(identity.GetUserId()).Any();
            return haveProposal;
        }

        public static int UserProfileId(this IIdentity identity)
        {
            var userService = DependencyResolver.Current.GetService<IUserProfileService>();
            return userService.GetUserProfileIdByUserIdentityId(identity.GetUserId());
        }

        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            try
            {
                var identity = currentPrincipal.Identity as ClaimsIdentity;
                if (identity == null)
                    return null;

                var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
                return claim.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void UpdateHasChangedPasswordClaimValue(this IPrincipal currentPrincipal, string value)
        {
            currentPrincipal.AddUpdateClaim("HasChangedPassword", value);
        }

        public static void UpdateCanSeeWelcomeCardsClaimValue(this IPrincipal currentPrincipal, string value)
        {
            currentPrincipal.AddUpdateClaim("CanSeeWelcomeCards", value);
        }

        public static void UpdateEmployeeStatusClaimValue(this IPrincipal currentPrincipal, string value)
        {
            currentPrincipal.AddUpdateClaim("EmployeeStatus", value);
        }

        public static string GetHasChangedPasswordClaim(this IPrincipal currentPrincipal)
        {
            return currentPrincipal.GetClaimValue("HasChangedPassword");
        }

        public static string GetCanSeeWelcomeCardsClaim(this IPrincipal currentPrincipal)
        {
            return currentPrincipal.GetClaimValue("CanSeeWelcomeCards");
        }

        public static bool GetCanChangePasswordClaim(this IPrincipal currentPrincipal)
        {
            return Convert.ToBoolean(currentPrincipal.GetClaimValue("CanChangePassword"));
        }

        public static bool GetIsAdfsUserClaim(this IPrincipal currentPrincipal)
        {
            var isAdfs = currentPrincipal.GetClaimValue("IsAdfsUser");

            if (string.IsNullOrEmpty(isAdfs))
                return false;
            return Convert.ToBoolean(isAdfs);
        }

        public static string GetEmployeeStatus(this IPrincipal currentPrincipal)
        {
            return currentPrincipal.GetClaimValue("EmployeeStatus");
        }

        public static string GetEmail(this IPrincipal currentPrincipal)
        {
            return currentPrincipal.GetClaimValue(ClaimTypes.Email);
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("PortaliaContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}