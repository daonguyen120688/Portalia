using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class ApplicationDocumentMapping : EntityTypeConfiguration<ApplicationDocument>
    {
        public ApplicationDocumentMapping()
        {
            ToTable("dbo.ApplicationDocument");
            HasKey(c => c.ApplicationDocumentId);

            HasRequired(s => s.ApplicationForm)
                .WithRequiredPrincipal();

            HasRequired(s => s.DocumentType)
                .WithRequiredPrincipal();
        }
    }
}
