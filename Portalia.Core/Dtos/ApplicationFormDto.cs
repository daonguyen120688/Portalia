using System;

namespace Portalia.Core.Dtos
{
    public class ApplicationFormDto
    {
        public int JobOfferId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public byte? Gender { get; set; }
        public string Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Address { get; set; }
        public string PreferredLanguage { get; set; }
        public string SSN { get; set; }
        public string Situation { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
