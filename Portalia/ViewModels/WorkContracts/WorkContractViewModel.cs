using System;
using Portalia.Core.Enum;
using Portalia.Attributes;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Portalia.ViewModels.WorkContracts
{
    public class WorkContractViewModel
    {
        public int ContractId { get; set; }

        public int Type { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Title { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string FirstName { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string LastName { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string CountryOfBirth { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Nationality { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Address { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string PostCode { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string City { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Country { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string SSN { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string VisaPermitNo { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ContractStartDate { get; set; }

        [CompareDate(RefProperty = "ContractStartDate", ErrorMessage = "Doit être supérieure à la date de début du contrat.",Operator =ComparisonOperator.GreaterThan)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ContractEndDate { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string ProjectDescription { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Skills { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string ClientNameByCandidate { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string ClientAddress { get; set; }

        public decimal? Allowances {
            get
            {
                if (string.IsNullOrEmpty(AllowancesDisplay))
                    return null;
                return decimal.Parse(AllowancesDisplay, new CultureInfo("de-DE"));
            }
        }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string AllowancesDisplay { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Currency { get; set; }

        [RequireIfTrue(ErrorMessage = "Ce champ est requis", FlagProperty = "IsSubmit")]
        public string Basic { get; set; }

        public string LastCommentDate { get;set;}

        public string FooterLastCommentDate { get; set; }

        public WorkContractStatusEnum? WCStatus { get; set; }

        [HiddenInput]
        public bool IsSubmit { get; set; }

        [HiddenInput]
        public bool IsRevision { get; set; }

        [HiddenInput]
        public bool IsValidation { get; set; }

        public bool IsInputDisabled { get; set; }

        public string HighlightedFields { get; set; }

        public IEnumerable<SelectListItem> LstTitles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstCountries { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstNationalities { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstSkills { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstCurrencies { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstBasic { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstCities { get; set; } = new List<SelectListItem>();

        // To display only
        public string Email { get; set; }
        public int ProposalId { get; set; }
        public int DocumentId { get; set; }
    }
}