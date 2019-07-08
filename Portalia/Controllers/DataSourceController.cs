using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Portalia.Core.Interface.Service;
using Portalia.ViewModels;
using Portalia.Core.Infrastructure;
using System.Threading.Tasks;
using System.Net.Http;
using Portalia.Core.Helpers;
using Portalia.ViewModels.WorkContract;
using System;
using Newtonsoft.Json;

namespace Portalia.Controllers
{
    public class DataSourceController : BaseController
    {
        private IDataSourceService _dataSourceService;

        public DataSourceController(IDataSourceService dataSourceService)
        {
            _dataSourceService = dataSourceService;
        }

        public JsonResult Gender()
        {
            var listItem = new List<SelectViewModel>()
            {
                new SelectViewModel()
                {
                    value = 1.ToString(),
                    text = "Homme"
                },
                new SelectViewModel()
                {
                    value = 2.ToString(),
                    text = "Femme"
                }
            };

            return Json(listItem, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult Country()
        {
            var countryResource = Resources.Nationality.ResourceManager;
            var listItems = _dataSourceService.GetCountry().OrderBy(c=>c.Label).Select(c => new SelectViewModel()
            {
                value = c.ID,
                text = countryResource.GetString(c.ID)
            }).Where(c => c.text != null).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Language()
        {
            var listItems = _dataSourceService.GetLanguages().Select(c => new SelectViewModel()
            {
                value = c.ID,
                text = c.Label
            }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BankSystems()
        {
            var listItems = _dataSourceService.BankInformationSystems().Select(c => new SelectViewModel()
            {
                value = c.BankInformationSystemId.ToString(),
                text = c.Label
            }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Currency()
        {
            var listItems = _dataSourceService.GetCurrency().Select(c => new SelectViewModel()
            {
                value = c.ID.ToString(),
                text = c.Label
            }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BirthPlace()
        {
            var listItems = _dataSourceService.GetBirthPlace().OrderBy(p => p.Label).Select(c => new SelectViewModel
            {
                value = c.ID.ToString(),
                text = c.Label
            }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Situation()
        {
            var listItem = new List<SelectViewModel>()
            {
                new SelectViewModel()
                {
                    value = 1.ToString(),
                    text = "Celibataire"
                },
                new SelectViewModel()
                {
                    value = 2.ToString(),
                    text = "marié"
                }
            };

            return Json(listItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult City(string countryCode,int? id, string name = "")
        {
            if (id.HasValue)
            {
                var city = _dataSourceService.GetSmartCity().FirstOrDefault(c => c.ID == id.Value && c.CountryId==countryCode && c.Valid);
                if (city == null) { return HttpNotFound(); }
                return Json(new
                    {
                        id = city.ID+"",
                        text = city.Name
                    }
                , JsonRequestBehavior.AllowGet);
            }
            var cityQuery = _dataSourceService.GetSmartCity().OrderBy(c => c.Name).Where(x=>x.CountryId==countryCode && x.Valid);
            if (!string.IsNullOrEmpty(name))
            {
                cityQuery = cityQuery.Where(c => c.Name.ToLower().Contains(name));
            }
            else
            {
                cityQuery = cityQuery.Where(c => !string.IsNullOrEmpty(c.Name.Trim()));
            }
            var cities = cityQuery.Take(10)
                .Select(c => new {
                    id = c.ID + "",
                    text = c.Name
                }).ToList();
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Skill(int? id, string name = "")
        {
            if (id.HasValue)
            {
                var skill = _dataSourceService.GetSkills().FirstOrDefault(c => c.SkillId == id.Value);
                if (skill == null) { return HttpNotFound(); }
                return Json(
                    new {
                        id = skill.SkillId+"",
                        text = skill.Label
                    }
                , JsonRequestBehavior.AllowGet);
            }
            var skillQuery = _dataSourceService.GetSkills().OrderBy(c => c.Label).Where(c => c.SkillId > 0);
            if (!string.IsNullOrEmpty(name))
            {
                skillQuery = skillQuery.Where(c => c.Label.ToLower().Contains(name.ToLower()));
            }
            var skills = skillQuery.Take(10)
                .Select(c => new                 {
                    id = c.SkillId+"",
                    text = c.Label
                });
            return Json(skills.ToList(), JsonRequestBehavior.AllowGet);
        }

        //public async Task<string> GetClientAddress(int clientId)
        //{
        //    if (clientId == 0)
        //        return null;

        //    try
        //    {
        //        List<ClientViewModel> result = Caching.GetCache<List<ClientViewModel>>(CacheKey.ClientList, () =>
        //        {
        //            return null;
        //        });

        //        if (result == null)
        //        {
        //            int[] subsidiaryIds = new[] { 170 };

        //            Dictionary<string, string> values = new Dictionary<string, string>
        //            {
        //                { "SubsidiaryID", JsonConvert.SerializeObject(subsidiaryIds) }
        //            };

        //            const string getClientListApi = "/ABCApi/api/Account/GetAccountsBySubsidiaryIds";

        //            FormUrlEncodedContent content = new FormUrlEncodedContent(values);

        //            result = await ApiRequestHelper.PostAsync<List<ClientViewModel>>(Request.Url.Host, getClientListApi, content);
        //            Caching.SetCache(CacheKey.ClientList, result, 3);
        //        }

        //        ClientViewModel model = result.Where(x => x.EntityRelationshipId == clientId).FirstOrDefault();

        //        return model?.Offices?.FirstOrDefault()?.Value;
        //    }
        //    catch(Exception ex)
        //    {
        //        LoggerHelpers.Error(ex);
        //        return null;
        //    }
        //}
    }
}