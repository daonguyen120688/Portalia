using Portalia.Core.Enum;
using System;

namespace Portalia.Core.Entity
{
    public class WorkContractStatus : Repository.Pattern.Ef6.Entity
    {
        public int WorkContractStatusId { get; set; }
        public int ContractId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool HasSentReminder { get; set; } = false;

        public virtual StatusMapping StatusMapping { get; set; }
        public virtual WorkContract WorkContract { get; set; }
    }
}
