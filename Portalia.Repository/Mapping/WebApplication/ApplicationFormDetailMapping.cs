using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class ApplicationFormDetailMapping : EntityTypeConfiguration<ApplicationFormDetail>
    {
        public ApplicationFormDetailMapping()
        {
            ToTable("dbo.ApplicationFormDetail");
            HasKey(c => c.ApplicationFormDetailId);

            HasRequired(s => s.ApplicationForm)
                .WithRequiredPrincipal();

            HasRequired(s => s.Gender)
                .WithRequiredPrincipal();

            HasRequired(s => s.Title)
                .WithRequiredPrincipal();
        }
    }
}
