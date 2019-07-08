using Portalia.Core.Enum;

namespace Portalia.ViewModels.Proposals
{
    public class ProposalDetailViewModel
    {
        public int ProposalId { get; set; }
        public string UserId { get; set; }
        public bool CanSeeMenu { get; set; }
        public bool CanSeeContract { get; set; }
        public string EmployeeStatus { get; set; }
        public bool IsEmployee { get; set; }

        public DocumentViewModel Document { get; set; } = new DocumentViewModel();
    }
}