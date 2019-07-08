using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using Portalia.Extentions;
using Portalia.ViewModels.WorkContracts;

namespace Portalia.Controllers
{
    [Authorize]
    public class LayoutController : BaseController
    {
        private readonly IWorkContractService _workContactService;
        private readonly IProposalService _proposalService;

        public LayoutController(IWorkContractService workContactService,
            IProposalService proposalService)
        {
            _workContactService = workContactService;
            _proposalService = proposalService;
        }

        [ChildActionOnly]
        public ActionResult WorkContractButton()
        {
            WorkContractViewModel model = null;
            
            // Get current user work contract
            var dto = _workContactService.GetWCByUser(User.Identity.GetUserId(), WorkContractType.Candidate);
            if (dto != null)
            {
                model = dto.ToWorkContractViewModel();
            }

            return PartialView("_WorkContractButton", model);
        }
    }
}