using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class CountryMapping:EntityTypeConfiguration<Country>
    {
        public CountryMapping()
        {
            ToTable("dbo.Country");
            HasKey(c => c.ID);
        }
    }
}
