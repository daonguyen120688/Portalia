using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Portalia.Attributes;
using Portalia.Resources;

namespace Portalia.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Resources.Login), Name = "RememberMe")]
        public bool RememberMe { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }

    public class RegisterViewModel
    {

        [Required(ErrorMessageResourceName = "PrenomRequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "Le Prénom doit comporter au moins 3 caractères.", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "NomRequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Le Nom de famille doit comporter au moins 3 caractères.", MinimumLength = 3)]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "EmailRequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "MotDePasseRequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [StringLength(100, ErrorMessage = "Le Mot de passe doit comporter au moins 8 caractères.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [PasswordValidation]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ce champ est requis")]
        public int Gender { get; set; }

        //[Required(ErrorMessage = "L'emplacement est requis")]
        //[StringLength(128, ErrorMessage = "Le Mot de passe doit comporter au moins 3 caractères.", MinimumLength = 3)]
        public string Location { get; set; }
    }

    public class ResetPasswordViewModel
    {

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [StringLength(100, ErrorMessage = "Le Mot de passe doit comporter au moins 8 caractères.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmation")]
        [Compare("NewPassword", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "ResetPasswordCodeRequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string Code { get; set; }

        [Required(ErrorMessageResourceName = "UserNotSpecifiedMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string UserId { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ChangePasswordForNewPolicyViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [StringLength(100, ErrorMessage = "Le Mot de passe doit comporter au moins 6 caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmation")]
        [Compare("NewPassword", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [StringLength(100, ErrorMessage = "Le Mot de passe doit comporter au moins 8 caractères.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string NewPassword { get; set; }
    }
}
