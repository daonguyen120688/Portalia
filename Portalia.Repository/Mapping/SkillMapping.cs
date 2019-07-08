using Portalia.Core.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping
{
    public class SkillMapping : EntityTypeConfiguration<Skill>
    {
        public SkillMapping()
        {
            ToTable("dbo.Skill");
            HasKey(x => x.SkillId);
        }
    }
}
