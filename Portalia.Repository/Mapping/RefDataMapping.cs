using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class RefDataMapping : EntityTypeConfiguration<RefData>
    {
        public RefDataMapping()
        {
            ToTable("dbo.RefData");
            HasKey(x => x.Id);
        }
    }
}
