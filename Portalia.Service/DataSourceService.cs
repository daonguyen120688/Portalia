using System.Collections.Generic;
using System.Linq;
using Portalia.Core.Dtos;
using Portalia.Core.Entity;
using Portalia.Core.Interface.Service;
using Portalia.Repository;

namespace Portalia.Service
{
    public class DataSourceService : IDataSourceService
    {
        private readonly PortaliaContext _portaliaContext;

        public DataSourceService(PortaliaContext portaliaContext)
        {
            _portaliaContext = portaliaContext;
        }

        public List<Country> GetCountry()
        {
            return _portaliaContext.Countries.AsNoTracking().ToList();
        }

        public IEnumerable<RefDataDto> GetCountryForWC()
        {
            return _portaliaContext.Countries.AsNoTracking().Select(x=>new RefDataDto() {
                Key=x.ID,
                Value=x.Label
            });
        }

        public List<Language> GetLanguages()
        {
            return _portaliaContext.Languages.AsNoTracking().ToList();
        }

        public List<BankInformationSystem> BankInformationSystems()
        {
            return _portaliaContext.BankInformationSystems.AsNoTracking().ToList();
        }

        public List<Currency> GetCurrency()
        {
            return _portaliaContext.Currencies.AsNoTracking().ToList();
        }

        public List<BirthPlace> GetBirthPlace()
        {
            return _portaliaContext.BirthPlaces.AsNoTracking().ToList();
        }

        public IEnumerable<RefDataDto> GetBirthPlaceForWC()
        {
            return _portaliaContext.BirthPlaces.AsNoTracking().Select(x => new RefDataDto()
            {
                Key = x.ID,
                Value = x.Label
            });
        }

        public IEnumerable<RefDataDto> GetCurrencyForWC()
        {
            return _portaliaContext.Currencies.AsNoTracking().Select(x => new RefDataDto()
            {
                Key = x.ID,
                Value = x.Label
            });
        }

        public IEnumerable<City> GetCity()
        {
            return _portaliaContext.Cities.AsEnumerable();
        }

        public IEnumerable<Amaris_Smart_City> GetSmartCity()
        {
            return _portaliaContext.Amaris_Smart_Cities.AsEnumerable();
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _portaliaContext.Skills.AsEnumerable(); 
        }

        public IEnumerable<Skill> GetSpecificSkills(List<int> arrSkillIds)
        {
            return GetSkills().Where(x => arrSkillIds.Contains(x.SkillId)).AsEnumerable();
        }
    }
}
