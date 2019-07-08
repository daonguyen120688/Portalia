using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class UserProfileAttributeMapping : EntityTypeConfiguration<UserProfileAttribute>
    {
        public UserProfileAttributeMapping()
        {
            ToTable("dbo.UserProfileAttribute");
            HasKey(x => x.UserProfileAttributeId);
            Property(x => x.UserProfileAttributeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasRequired(c => c.AttributeDetail).WithMany(c => c.UserProfileAttributes).HasForeignKey(c => c.AttributeDetailId);
            HasRequired(c => c.UserProfile).WithMany(c => c.UserProfileAttributes).HasForeignKey(c => c.UserProfileId);
        }
    }
}
