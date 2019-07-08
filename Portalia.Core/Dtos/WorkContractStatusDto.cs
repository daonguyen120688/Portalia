using Portalia.Core.Enum;
using System;

namespace Portalia.Core.Dtos
{
    public class WorkContractStatusDto
    {
        public int WorkContractStatusId { get; set; }
        public int ContractId { get; set; }
        public WorkContractStatusEnum StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
