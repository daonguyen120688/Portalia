using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class LanguageMapping: EntityTypeConfiguration<Language>
    {
        public LanguageMapping()
        {
            ToTable("dbo.Language");
            HasKey(c => c.ID);
        }
    }
}
