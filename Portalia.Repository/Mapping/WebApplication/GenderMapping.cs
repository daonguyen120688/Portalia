using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class GenderMapping : EntityTypeConfiguration<Gender>
    {
        public GenderMapping()
        {
            ToTable("dbo.Gender");
            HasKey(c => c.GenderId);

            HasMany(s => s.ApplicationFormDetails)
                .WithRequired(s => s.Gender)
                .HasForeignKey(s => s.GenderId)
                .WillCascadeOnDelete(false);
        }
    }
}
