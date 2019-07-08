using System;
using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class TrackingChange : Repository.Pattern.Ef6.Entity
    {
        public int TrackingChangeId { get; set; }
        public int ContractId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public virtual WorkContract WorkContract { get; set; }

    }
}
