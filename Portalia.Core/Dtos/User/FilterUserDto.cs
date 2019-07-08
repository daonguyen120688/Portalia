namespace Portalia.Core.Dtos.User
{
    public class FilterUserDto
    {
        public byte PageNumber { get; set; }
        public byte PageSize { get; set; }
        public bool? IsEmployee { get; set; }
        public string SearchUserNameQuery { get; set; }
        public byte Status { get; set; }
    }
}
