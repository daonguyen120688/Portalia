using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class DocumentTypeMapping : EntityTypeConfiguration<DocumentType>
    {
        public DocumentTypeMapping()
        {
            ToTable("dbo.DocumentType");
            HasKey(c => c.DocumentTypeId);

            HasMany(s => s.ApplicationDocuments)
                .WithRequired(s => s.DocumentType)
                .HasForeignKey(s => s.TypeId)
                .WillCascadeOnDelete(false);
        }
    }
}
