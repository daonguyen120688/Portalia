using System.Data;
using System.Data.SqlClient;
using Portalia.Core.Entity;
using Portalia.Core.Interface.Service;
using Portalia.Repository;

namespace Portalia.Service
{
    public class MigrationService : IMigrationService
    {
        private readonly PortaliaContext _context;

        public MigrationService(PortaliaContext context)
        {
            _context = context;
        }

        public void Create(Migration migration)
        {
            _context.Database.ExecuteSqlCommand("Auth.CreateMigration @OldAspNetUserId, @NewAuth0Id",
                new SqlParameter("@OldAspNetUserId", migration.OldAspNetUserId),
                new SqlParameter("@NewAuth0Id", migration.NewAuth0Id));
        }
    }
}
