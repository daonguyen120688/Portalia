using Portalia.Core.Interface.Service;
using Portalia.Core.Dtos;
using System.Web.Mvc;
using System.Collections.Generic;
using Portalia.Core.Infrastructure;

namespace Portalia.Controllers
{
    [Authorize]
    public class RefDataController : BaseController
    {
        private readonly IRefDataService _refDataService;

        public RefDataController(IRefDataService refDataService)
        {
            _refDataService = refDataService;
        }

        // GET: RefData
        public ActionResult GetDailyBasicData()
        {
            IEnumerable<RefDataDto> refData = Caching.GetCache<IEnumerable<RefDataDto>>(CacheKey.DailyBasic, () =>
            {
                return _refDataService.GetRefDataByCode(CacheKey.DailyBasic);
            });
            return Json(refData,JsonRequestBehavior.AllowGet);
        }
    }
}