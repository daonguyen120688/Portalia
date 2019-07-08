using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class BirthPlaceMapping: EntityTypeConfiguration<BirthPlace>
    {
        public BirthPlaceMapping()
        {
            ToTable("dbo.BirthPlace");
            HasKey(c => c.ID);
        }
    }
}
