using System.ComponentModel.DataAnnotations;
using Portalia.Resources;

namespace Portalia.ViewModels
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string Message { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string Object { get; set; }
    }
}