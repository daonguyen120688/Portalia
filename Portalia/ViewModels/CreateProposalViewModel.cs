using System.ComponentModel.DataAnnotations;
using Portalia.Resources;

namespace Portalia.ViewModels
{
    public class CreateProposalViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Display(Name = "Project Name")]
        [StringLength(100, ErrorMessage = "Le nom du projet doit comporter au moins 3 caractères.", MinimumLength = 3)]
        public string ProjectName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ValidationMessage))]
        [Display(Name = "Client Name")]
        [StringLength(100, ErrorMessage = "Le Nom du client doit comporter au moins 3 caractères.", MinimumLength = 3)]
        public string ClientName { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
    }
}