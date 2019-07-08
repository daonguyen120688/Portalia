using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class TitleMapping : EntityTypeConfiguration<Title>
    {
        public TitleMapping()
        {
            ToTable("dbo.Title");
            HasKey(c => c.TitleId);

            HasMany(s => s.ApplicationFormDetails)
                .WithRequired(s => s.Title)
                .HasForeignKey(s => s.TitleId)
                .WillCascadeOnDelete(false);
        }
    }
}
