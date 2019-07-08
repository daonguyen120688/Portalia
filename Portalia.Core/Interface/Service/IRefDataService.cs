using Portalia.Core.Dtos;
using System.Collections.Generic;

namespace Portalia.Core.Interface.Service
{
    public interface IRefDataService
    {
        IEnumerable<RefDataDto> GetRefDataByCode(string code);
    }
}
