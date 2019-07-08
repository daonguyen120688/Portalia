using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class ApplicationSourceMapping : EntityTypeConfiguration<ApplicationSource>
    {
        public ApplicationSourceMapping()
        {
            ToTable("dbo.ApplicationSource");
            HasKey(c => c.ApplicationSourceId);

            HasMany(s => s.ApplicationForms)
                .WithRequired(s => s.ApplicationSource)
                .HasForeignKey(s => s.ApplicationSourceId)
                .WillCascadeOnDelete(false);
        }
    }
}
