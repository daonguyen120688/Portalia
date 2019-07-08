using System;

namespace Portalia.Core.Entity.WebApplication
{
    public class ApplicationFormDetail
    {
        public int ApplicationFormDetailId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Nullable<byte> TitleId { get; set; }
        public Nullable<byte> GenderId { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string PreferredLanguageId { get; set; }
        public string PhoneNumber { get; set; }
        public string ResidenceCountryId { get; set; }
        public Nullable<int> ResidenceCityId { get; set; }
        public string ResidenceZipCode { get; set; }
        public string ResidenceAddress { get; set; }
        public string CitizenshipCountryId { get; set; }
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }

        public virtual ApplicationForm ApplicationForm { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Title Title { get; set; }
    }
}
