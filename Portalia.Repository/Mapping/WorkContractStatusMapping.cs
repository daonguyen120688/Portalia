using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class WorkContractStatusMapping: EntityTypeConfiguration<WorkContractStatus>
    {
        public WorkContractStatusMapping()
        {
            ToTable("dbo.WorkContractStatus");
        }
    }
}
