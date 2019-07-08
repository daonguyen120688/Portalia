using System.ComponentModel.DataAnnotations;

namespace Portalia.Core.Entity
{
    public class City : Repository.Pattern.Ef6.Entity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameASCII { get; set; }
        public string Country { get; set; }
        public string CountryCode2 { get; set; }
        public string CountryCode3 { get; set; }
    }
}
