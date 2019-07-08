using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class StatusMappingMapping : EntityTypeConfiguration<StatusMapping>
    {
        public StatusMappingMapping()
        {
            ToTable("dbo.StatusMapping");
            HasKey(x => x.Id);
            HasMany(x => x.WorkContractStatuses).WithRequired(x => x.StatusMapping).HasForeignKey(x => x.StatusId);
        }
    }
}
