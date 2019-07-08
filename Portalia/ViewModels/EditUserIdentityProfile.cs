using System.ComponentModel.DataAnnotations;

namespace Portalia.ViewModels
{
    public class EditUserIdentityProfile
    {
        public string UserId { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}