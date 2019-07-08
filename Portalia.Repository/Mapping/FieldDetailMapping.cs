using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class FieldDetailMapping : EntityTypeConfiguration<FieldDetail>
    {
        public FieldDetailMapping()
        {
            ToTable("dbo.FieldDetails");
            HasKey(x => x.FieldDetailId);
        }
    }
}
