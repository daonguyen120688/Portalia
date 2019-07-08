using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class AttributeDetailMapping: EntityTypeConfiguration<AttributeDetail>
    {
        public AttributeDetailMapping()
        {
            ToTable("dbo.AttributeDetail");
            HasKey(x => x.AttributeDetailId);
            HasRequired(x => x.AttributeType)
                .WithMany(x => x.AttributeDetails)
                .HasForeignKey(x => x.AttributeTypeId);
        }
    }
}
