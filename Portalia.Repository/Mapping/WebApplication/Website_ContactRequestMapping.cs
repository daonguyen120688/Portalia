using Portalia.Core.Entity.WebApplication;
using System.Data.Entity.ModelConfiguration;

namespace Portalia.Repository.Mapping.WebApplication
{
    public class Website_ContactRequestMapping : EntityTypeConfiguration<Website_ContactRequest>
    {
        public Website_ContactRequestMapping()
        {
            ToTable("dbo.Website_ContactRequest");
            HasKey(c => c.ContactRequestId);
        }
    }
}
