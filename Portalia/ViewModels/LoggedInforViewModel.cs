using System.Collections.Generic;

namespace Portalia.ViewModels
{
    public class LoggedInforViewModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserRole { get; set; }
        public string FirstName { get; set; }
        public string Fullname { get; set; }
        public List<LinkViewModel> Links { get; set; }
        public string PictureUrl { get; set; }
        public bool CanSeeWelcomeCards { get; set; }
        public string UserId { get; set; }
        public string EmployeeStatus { get; set; }

        public LoggedInforViewModel()
        {
            Links = new List<LinkViewModel>();
        }
    }

    public enum Role
    {
        Administrator=1,
        Employee=2,
        Visitor=3
    }
}