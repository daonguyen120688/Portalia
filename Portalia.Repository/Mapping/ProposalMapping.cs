using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class ProposalMapping : EntityTypeConfiguration<Proposal>
    {
        public ProposalMapping()
        {
            ToTable("dbo.Proposal");
            HasKey(x => x.ProposalId);
            Property(x => x.ProposalStatus).HasColumnName("ProposalStatusId");
        }
    }
}
