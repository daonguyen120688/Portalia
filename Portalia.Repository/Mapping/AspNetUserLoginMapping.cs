using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class AspNetUserLoginMapping: EntityTypeConfiguration<AspNetUserLogin>
    {
        public AspNetUserLoginMapping()
        {
            HasKey(c => c.LoginProvider);
            HasKey(c => c.ProviderKey);
            HasKey(c => c.UserId);
        }
    }
}
