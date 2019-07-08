using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class CityMapping : EntityTypeConfiguration<City>
    {
        public CityMapping()
        {
            ToTable("dbo.City");
            HasKey(x => x.Id);
        }
    }
}
