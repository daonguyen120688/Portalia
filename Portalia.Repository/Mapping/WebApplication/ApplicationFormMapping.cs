using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class ApplicationFormMapping : EntityTypeConfiguration<ApplicationForm>
    {
        public ApplicationFormMapping()
        {
            ToTable("dbo.ApplicationForm");
            HasKey(c => c.ApplicationFormId);

            HasRequired(s => s.ApplicationFormDetail)
                .WithRequiredDependent();

            HasRequired(s => s.ApplicationFormStatus)
                .WithRequiredPrincipal();

            HasRequired(s => s.ApplicationSource)
                .WithRequiredPrincipal();

            HasMany(s => s.ApplicationDocuments)
                .WithRequired(s=>s.ApplicationForm)
                .HasForeignKey(s=>s.ApplicationFormId)
                .WillCascadeOnDelete(false);
        }
    }
}
