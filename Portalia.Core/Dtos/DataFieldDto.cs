using System;

namespace Portalia.Core.Dtos
{
    public class DataFieldDto
    {
        public int DataFieldId { get; set; }
        public int ContractId { get; set; }
        public int FieldId { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
