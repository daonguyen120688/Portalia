
using System.ComponentModel.DataAnnotations;

namespace Portalia.Core.Entity
{
    public class Amaris_Smart_City : Repository.Pattern.Ef6.Entity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }
        public int? LocationId { get; set; }
        public int? RegionId { get; set; }
        public bool? IsAmarisCity { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string GooglePlaceId { get; set; }
        public int? TimezoneId { get; set; }
        public string HelpPlaceId { get; set; }
        public string GeocodePlaceId { get; set; }
        public bool Valid { get; set; }
    }
}
