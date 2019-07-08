using System.Collections.Generic;
using Portalia.Core.Dtos;
using Portalia.Core.Entity;

namespace Portalia.Core.Interface.Service
{
    public interface IDataSourceService
    {
        List<Country> GetCountry();
        List<Language> GetLanguages();
        List<BankInformationSystem> BankInformationSystems();
        List<Currency> GetCurrency();
        List<BirthPlace> GetBirthPlace();
        IEnumerable<City> GetCity();
        IEnumerable<Skill> GetSkills();
        IEnumerable<RefDataDto> GetCountryForWC();
        IEnumerable<RefDataDto> GetBirthPlaceForWC();
        IEnumerable<RefDataDto> GetCurrencyForWC();
        IEnumerable<Skill> GetSpecificSkills(List<int> arrSkillIds);
        IEnumerable<Amaris_Smart_City> GetSmartCity();
    }
}
