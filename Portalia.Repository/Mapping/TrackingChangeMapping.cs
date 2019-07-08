using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class TrackingChangeMapping : EntityTypeConfiguration<TrackingChange>
    {
        public TrackingChangeMapping()
        {
            ToTable("dbo.TrackingChanges");
            HasKey(x => x.TrackingChangeId);
            HasRequired(x => x.WorkContract).WithMany(x => x.TrackingChanges).HasForeignKey(x => x.ContractId);
        }
    }
}
