using System;

namespace Portalia.Core.Entity.WebApplication
{
    public class GetWebAppForm_Result
    {
        public int ApplicationFormId { get; set; }
        public byte ApplicationSourceId { get; set; }
        public int HoldingId { get; set; }
        public string ApplicationSourceLabel { get; set; }
        public string Email { get; set; }
        public Nullable<int> JobOfferId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Nullable<byte> GenderId { get; set; }
        public Nullable<byte> TitleId { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string CitizenshipCountryId { get; set; }
        public string ResidenceCountryId { get; set; }
        public Nullable<int> ResidenceCityId { get; set; }
        public string ResidenceAddress { get; set; }
        public string ResidenceZipCode { get; set; }
        public string PreferredLanguageId { get; set; }
    }
}
