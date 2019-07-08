using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class CountryLanguageMapping: EntityTypeConfiguration<CountryLanguage>
    {
        public CountryLanguageMapping()
        {
            ToTable("dbo.CountryLanguage");
            HasKey(c => c.LanguageID);
            HasRequired(c => c.Language).WithMany(c => c.CountryLanguages).HasForeignKey(c => c.CountryID);
        }
    }
}
