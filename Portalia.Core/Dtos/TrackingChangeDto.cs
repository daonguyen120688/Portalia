using System;

namespace Portalia.Core.Dtos
{
    public class TrackingChangeDto
    {
        public int TrackingChangeId { get; set; }
        public int ContractId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
