using System;
using Portalia.Core.Enum;
using Portalia.Core.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Portalia.Core.Dtos
{
    public class WorkContractDto
    {
        [IgnoreTrackingChange]
        public int ContractId { get; set; }

        [IgnoreTrackingChange]
        public string UserId { get; set; }

        [IgnoreTrackingChange]
        public int Type { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstTitles")]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [DataTypeInfor(FieldType =FieldType.Text)]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [DataTypeInfor(FieldType = FieldType.Text)]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [DataTypeInfor(FieldType = FieldType.DateTime,DateTimeFormat = "dd/MM/yyyy")]
        [Display(Name = "Date de naissance")]
        public DateTime? DateOfBirth { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstCountries")]
        [Display(Name = "Pays de naissance")]
        public string CountryOfBirth { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstNationalities")]
        [Display(Name = "Nationalité")]
        public string Nationality { get; set; }

        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Display(Name = "Code Postal")]
        public string PostCode { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstCities")]
        [Display(Name = "Ville")]
        public string City { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstCountries")]
        [Display(Name = "Pays")]
        public string Country { get; set; }

        [Display(Name = "Numéro de Sécurité Sociale")]
        public string SSN { get; set; }

        [Display(Name = "Référence du permis de travail")]
        public string VisaPermitNo { get; set; }

        [DataTypeInfor(FieldType = FieldType.DateTime, DateTimeFormat = "dd/MM/yyyy")]
        [Display(Name = "Date de début du contrat")]
        public DateTime? ContractStartDate { get; set; }

        [DataTypeInfor(FieldType = FieldType.DateTime, DateTimeFormat = "dd/MM/yyyy")]
        [Display(Name = "Date de fin du contrat (optionnel sauf si CDD)")]
        public DateTime? ContractEndDate { get; set; }

        [Display(Name = "Description du projet")]
        public string ProjectDescription { get; set; }

        [DataTypeInfor(FieldType = FieldType.MultiSelect, DatasourceProperty = "LstSkills")]
        [Display(Name = "Vos compétences")]
        public string Skills { get; set; }

        [Display(Name = "Nom de votre client (entré par le candidat)")]
        public string ClientNameByCandidate { get; set; }

        [Display(Name = "Adresse de votre client (entré par le candidat)")]
        public string ClientAddress { get; set; }

        [Display(Name = "Rémunération - Montant")]
        public decimal? Allowances { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstCurrencies")]
        [Display(Name = "Rémunération - Devise")]
        public string Currency { get; set; }

        [DataTypeInfor(FieldType = FieldType.DropDown, DatasourceProperty = "LstBasic")]
        [Display(Name = "Rémunération - Base")]
        public string Basic { get; set; }

        [IgnoreTrackingChange]
        public string CandidateId { get; set; }

        [IgnoreTrackingChange]
        public DateTime CreatedDate { get; set; }

        [IgnoreTrackingChange]
        public string CreatedBy { get; set; }

        [IgnoreTrackingChange]
        public DateTime? UpdatedDate { get; set; }

        [IgnoreTrackingChange]
        public string UpdatedBy { get; set; }

        [IgnoreTrackingChange]
        public string HighlightedFields { get; set; }

        public DateTime? LastCommentDate { get; set; }


        [IgnoreTrackingChange]
        public WorkContractStatusDto WorkContractStatus { get; set; } = new WorkContractStatusDto();

        [IgnoreTrackingChange]
        public WorkContractStatusEnum WorkContractStatusId { get; set; }

        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstTitles { get; set; } = new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstCountries { get; set; } = new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstNationalities { get; set; } = new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstCities { get; set; } = new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstSkills { get; set; } = new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstCurrencies { get; set; } = new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstBasic { get; set; }= new List<SelectListItem>();
        [IgnoreTrackingChange]
        public IEnumerable<SelectListItem> LstClientNames { get; set; } = new List<SelectListItem>();
        

        [IgnoreTrackingChange]
        public IEnumerable<DataFieldDto> DataFields { get; set; }

        // To display only
        [IgnoreTrackingChange]
        public string Email { get; set; }
        [IgnoreTrackingChange]
        public int ProposalId { get; set; }
        [IgnoreTrackingChange]
        public int DocumentId { get; set; }
    }
}
