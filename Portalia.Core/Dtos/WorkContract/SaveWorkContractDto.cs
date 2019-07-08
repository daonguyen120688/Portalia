using System;

namespace Portalia.Core.Dtos.WorkContract
{
    public class SaveWorkContractDto
    {
        public int ContractId { get; set; }
        public string UserId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SSN { get; set; }
        public string VisaPermitNo { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string ProjectDescription { get; set; }
        public string Skills { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public decimal? Allowances { get; set; }
        public string Currency { get; set; }
        public string Basic { get; set; }
        public string Comment { get; set; }
        public string CandidateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public WorkContractStatusDto WorkContractStatus { get; set; }
    }
}
