namespace Portalia.ViewModels.Users
{
    public class FilterUserViewModel
    {
        public byte PageNumber { get; set; } = 1;
        public byte PageSize { get; set; } = 10;
        public bool? IsEmployee { get; set; } = null;
        public string SearchUserNameQuery { get; set; } = string.Empty;
        public byte Status { get; set; } = 0;
    }
}