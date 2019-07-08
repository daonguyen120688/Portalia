using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Portalia.Resources;

namespace Portalia.ViewModels
{
    public class CreateAppointment
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [DisplayName("prenom")]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [DisplayName("nom de famille")]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [EmailAddress(ErrorMessage = "Le champ e-mail n'est pas une adresse e-mail valide")]
        public string Email { get; set; }

        public string DateTime { get; set; }
    }
}