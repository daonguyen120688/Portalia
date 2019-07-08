using System;
using System.Web.Http;
using Portalia.API.Attributes;
using Portalia.Core.Enum;
using Portalia.Core.Extensions;
using Portalia.Core.Interface.Service;
using Portalia.ViewModels.WorkContracts;

namespace Portalia.API
{
    [AuthorizeApi]
    public class WorkContractController : ApiController
    {
        private readonly IWorkContractService _workContractService;
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WorkContractController(IWorkContractService workContractService)
        {
            _workContractService = workContractService;
        }

        [HttpGet]
        public IHttpActionResult GetValidatedWorkContract(string emailAddress)
        {
            try
            {
                var contract = _workContractService.GetValidatedWorkContract(emailAddress);
                if (contract != null)
                {
                    return Json(new WorkContractApiModel(contract));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult IsWorkContractValidated(string emailAddress)
        {
            try
            {
                // Get current work contract status
                WorkContractStatusEnum? status = _workContractService.GetWorkContractStatus(emailAddress);                
                if (status.HasValue)
                {
                    bool isValidated = EnumExtensions.GetWorkContractValidatedStatuses().Contains((int)status);
                    return Json(isValidated);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult SendReminderToCandidate()
        {
            var requestUrl = Request?.RequestUri?.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port,
                UriFormat.UriEscaped);
            _workContractService.SendReminderToCandidate(requestUrl);

            return Json(true);
        }
    }
}