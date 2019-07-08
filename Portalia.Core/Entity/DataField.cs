using System;

namespace Portalia.Core.Entity
{
    public class DataField:Repository.Pattern.Ef6.Entity
    {
        public int DataFieldId { get; set; }
        public int ContractId { get; set; }
        public int FieldId { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual WorkContract WorkContract { get; set; }
    }
}
