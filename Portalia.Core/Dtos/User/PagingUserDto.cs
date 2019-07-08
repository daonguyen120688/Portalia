using System.Collections.Generic;

namespace Portalia.Core.Dtos.User
{
    public class PagingUserDto
    {
        public IList<PagingUserItemDto> Users { get; set; } = new List<PagingUserItemDto>();
        public int TotalPages { get; set; } = 0;
        public int CurrentPage { get; set; }
    }
}
