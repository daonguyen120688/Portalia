using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class WorkContractCommentMapping : EntityTypeConfiguration<WorkContractComment>
    {
        public WorkContractCommentMapping()
        {
            ToTable("dbo.WorkContractComment");
            HasKey(x => x.WCCommentId);
        }
        
    }
}
