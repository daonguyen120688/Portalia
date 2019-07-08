using System;
using Portalia.Core.Enum;

namespace Portalia.Core.Dtos.User
{
    public class PagingUserItemDto
    {
        public int PercentOfInfo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{LastName} {FirstName}";
        public bool IsActive { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsNew { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public int PortaliaUserProfileId { get; set; }
        public WorkContractStatusEnum WorkContractStatusId { get; set; }
        public int WorkContractId { get; set; }
        public int? DocumentId { get; set; }
    }
}
