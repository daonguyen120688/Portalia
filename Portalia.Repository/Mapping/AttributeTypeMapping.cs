using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class AttributeTypeMapping: EntityTypeConfiguration<AttributeType>
    {
        public AttributeTypeMapping()
        {
            ToTable("dbo.AttributeType");
            HasKey(x => x.AttributeTypeId);
        }
    }
}
