using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class CurrencyMapping: EntityTypeConfiguration<Currency>
    {
        public CurrencyMapping()
        {
            ToTable("dbo.Currency");
            HasKey(c => c.ID);
        }
    }
}
