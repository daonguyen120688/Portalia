using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class DocumentMapping : EntityTypeConfiguration<Document>
    {
        public DocumentMapping()
        {
            ToTable("dbo.Document");
            HasKey(x => x.DocumentId);
            Property(x => x.FolderType).HasColumnName("FolderTypeId");
            HasOptional(x => x.Proposal).WithMany(x => x.Documents).HasForeignKey(x => x.ProposalId);
        }
    }
}
