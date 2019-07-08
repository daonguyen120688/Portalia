using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class DataFieldMapping : EntityTypeConfiguration<DataField>
    {
        public DataFieldMapping()
        {
            ToTable("dbo.DataFields");
            HasKey(x => x.DataFieldId);
            HasRequired(x => x.WorkContract).WithMany(x => x.DataFields).HasForeignKey(x => x.ContractId);
        }
    }
}
