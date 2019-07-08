using System.ComponentModel.DataAnnotations;
using Portalia.Resources;

namespace Portalia.ViewModels
{
    public class CallBackViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        public string Phone { get; set; }
    }
}