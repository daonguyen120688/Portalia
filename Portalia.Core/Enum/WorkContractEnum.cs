using System.ComponentModel.DataAnnotations;

namespace Portalia.Core.Enum
{
    public enum WorkContractStatusEnum
    {
        [Display(Name = "Disabled")]
        Disabled = 1,
        [Display(Name = "Pending on Candidate")]
        PendingOnCandidate = 2,
        [Display(Name = "Pending on Candidate")]
        PendingOnCandidateRevision = 4,
        [Display(Name = "Submitted")]
        Validated = 5,
        [Display(Name = "Uploaded")]
        Uploaded = 7
    }

    public enum WorkContractType
    {
        Candidate = 1
    }
}
