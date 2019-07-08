namespace Portalia.Core.Interface.Service
{
    public interface ISecuredTable
    {
        bool HavePermission(string keyValue, string tableName, string keyName);
    }
}
