using System;
using System.Collections.Generic;
using Portalia.Core.Attributes;

namespace Portalia.Core.Entity
{
    public class WorkContract : Repository.Pattern.Ef6.Entity
    {
        public WorkContract()
        {
            WorkContractStatuses = new HashSet<WorkContractStatus>();
            DataFields = new HashSet<DataField>();
            TrackingChanges = new HashSet<TrackingChange>();
        }


        public int ContractId { get; set; }
        public string UserId { get; set; }
        public int Type { get; set; }
        [WorkContractMappingDataField("Title")]
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [WorkContractMappingDataField("DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }
        [WorkContractMappingDataField("CountryOfBirth")]
        public string CountryOfBirth { get; set; }
        [WorkContractMappingDataField("Nationality")]
        public string Nationality { get; set; }
        [WorkContractMappingDataField("Address")]
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [WorkContractMappingDataField("SSN")]
        public string SSN { get; set; }
        [WorkContractMappingDataField("VisaPermitNo")]
        public string VisaPermitNo { get; set; }
        [WorkContractMappingDataField("ContractStartDate")]
        public DateTime? ContractStartDate { get; set; }
        [WorkContractMappingDataField("ContractEndDate")]
        public DateTime? ContractEndDate { get; set; }
        [WorkContractMappingDataField("ProjectDescription")]
        public string ProjectDescription { get; set; }
        [WorkContractMappingDataField("Skills")]
        public string Skills { get; set; }
        public string ClientNameByCandidate { get; set; }
        public string ClientAddress { get; set; }
        [WorkContractMappingDataField("Allowances")]
        public decimal? Allowances { get; set; }
        public string Currency { get; set; } = "EUR";
        public string Basic { get; set; } = "Par jour";
        public string CandidateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string HighlightedFields { get; set; }
        public int? DocumentId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<WorkContractStatus> WorkContractStatuses { get; set; }
        public virtual ICollection<DataField> DataFields { get; set; }
        public virtual ICollection<TrackingChange> TrackingChanges { get; set; }
    }
}
