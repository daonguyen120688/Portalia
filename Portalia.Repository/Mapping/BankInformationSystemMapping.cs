using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class BankInformationSystemMapping:EntityTypeConfiguration<BankInformationSystem>
    {
        public BankInformationSystemMapping()
        {
            ToTable("dbo.BankInformationSystem");
            HasKey(c => c.BankInformationSystemId);
        }
    }
}
