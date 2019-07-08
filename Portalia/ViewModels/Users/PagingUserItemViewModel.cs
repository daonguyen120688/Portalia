using Portalia.Core.Enum;

namespace Portalia.ViewModels.Users
{
    public class PagingUserItemViewModel
    {
        public int PercentOfInfo { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsNew { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string CreatedDate { get; set; }
        public int PortaliaUserProfileId { get; set; }
        public WorkContractStatusEnum WorkContractStatusId { get; set; } = WorkContractStatusEnum.Disabled;
        public int WorkContractId { get; set; }
        public int DocumentId { get; set; }
    }
}