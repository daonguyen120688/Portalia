using System.Collections.Generic;
using Portalia.Core.Dtos;
using Portalia.Core.Entity;
using Repository.Pattern.Repositories;
using System.Linq;
using System.Data.Entity;
using Portalia.Core.Interface.Service;

namespace Portalia.Service
{
    public class RefDataService : IRefDataService
    {
        private readonly IRepositoryAsync<RefData> _refDataRepository;

        public RefDataService(IRepositoryAsync<RefData> refDataRepository)
        {
            _refDataRepository = refDataRepository;
        }

        public IEnumerable<RefDataDto> GetRefDataByCode(string code)
        {
            return _refDataRepository.Queryable().AsNoTracking().Where(x => x.Code == code).Select(x=>new RefDataDto() {
                Key=x.Key,
                Value=x.Value
            }).ToList();
        }
    }
}
