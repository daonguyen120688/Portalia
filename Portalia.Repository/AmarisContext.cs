using System.Data.Entity;
using Portalia.Core.Entity;
using Portalia.Repository.Mapping;

namespace Portalia.Repository
{
    public class AmarisContext:DbContext
    {
        public AmarisContext()
            : base("name=SMART_AmarisEntities")
        {
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<CountryLanguage> CountryLanguages { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LanguageMapping());
            modelBuilder.Configurations.Add(new CountryLanguageMapping());
        }
    }
}
