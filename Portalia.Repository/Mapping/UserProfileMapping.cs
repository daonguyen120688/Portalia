using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class UserProfileMapping: EntityTypeConfiguration<UserProfile>
    {
        public UserProfileMapping()
        {
            ToTable("dbo.UserProfile");
            HasKey(x => x.UserProfileId);
        }
    }
}
