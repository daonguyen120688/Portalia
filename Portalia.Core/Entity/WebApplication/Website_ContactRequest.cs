using System;

namespace Portalia.Core.Entity.WebApplication
{
    public class Website_ContactRequest
    {
        public int ContactRequestId { get; set; }
        public string RequestSubject { get; set; }
        public string CountryId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Function { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Message { get; set; }
        public string ForwardedTo { get; set; }
    }
}
