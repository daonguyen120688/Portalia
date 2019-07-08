using System.Data.SqlClient;
using System.Linq;
using Portalia.Core.Interface.Service;
using Portalia.Repository;

namespace Portalia.Service
{
    public class SecuredTable:ISecuredTable
    {
        private PortaliaContext _portaliaContext;

        public SecuredTable(PortaliaContext portaliaContext)
        {
            _portaliaContext = portaliaContext;
        }

        public bool HavePermission(string keyValue, string tableName, string keyName)
        {
            return _portaliaContext.Database.SqlQuery<int>($"SELECT COUNT(*) FROM dbo.{tableName} WHERE {keyName}=@parameter",new SqlParameter("@parameter", keyValue)).First() > 0;
        }
    }
}
