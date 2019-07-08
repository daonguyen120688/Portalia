using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class WorkContractMapping : EntityTypeConfiguration<WorkContract>
    {
        public WorkContractMapping()
        {
            ToTable("dbo.WorkContracts");
            HasKey(x => x.ContractId);
            HasMany(x => x.TrackingChanges).WithRequired(x => x.WorkContract).HasForeignKey(x => x.ContractId);
            HasMany(x => x.WorkContractStatuses).WithRequired(x => x.WorkContract).HasForeignKey(x => x.ContractId);
            HasMany(x => x.DataFields).WithRequired(x => x.WorkContract).HasForeignKey(x => x.ContractId);
        }
    }
}
