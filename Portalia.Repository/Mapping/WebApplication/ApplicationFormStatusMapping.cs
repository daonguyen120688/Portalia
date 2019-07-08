using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class ApplicationFormStatusMapping : EntityTypeConfiguration<ApplicationFormStatus>
    {
        public ApplicationFormStatusMapping()
        {
            ToTable("dbo.ApplicationFormStatus");
            HasKey(c => c.ApplicationFormStatusId);

            HasMany(s => s.ApplicationForms)
                .WithRequired(s => s.ApplicationFormStatus)
                .HasForeignKey(s => s.StatusId)
                .WillCascadeOnDelete(false);
        }
    }
}
