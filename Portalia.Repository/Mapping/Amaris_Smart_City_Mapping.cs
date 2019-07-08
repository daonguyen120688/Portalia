using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class Amaris_Smart_City_Mapping : EntityTypeConfiguration<Amaris_Smart_City>
    {
        public Amaris_Smart_City_Mapping()
        {
            ToTable("dbo.Amaris_Smart_City");
            HasKey(x => x.ID);
        }
    }
}
