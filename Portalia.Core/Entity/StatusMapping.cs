using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class StatusMapping : Repository.Pattern.Ef6.Entity
    {
        public StatusMapping()
        {
            WorkContractStatuses = new HashSet<WorkContractStatus>();
        }

        public int Id { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<WorkContractStatus> WorkContractStatuses { get;set;}
    }
}
