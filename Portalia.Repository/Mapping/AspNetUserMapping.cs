using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class AspNetUserMapping : EntityTypeConfiguration<AspNetUser>
    {
        public AspNetUserMapping()
        {
            HasKey(x => x.Id);
            HasMany(x => x.WorkContracts).WithRequired(x => x.AspNetUser).HasForeignKey(x => x.UserId);
        }
    }
}
