using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MultiBranding.ApiClient;
using Newtonsoft.Json;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using Portalia.Extentions;
using Portalia.Models;
using Portalia.ViewModels;
using Portalia.ViewModels.Proposals;
using FolderType = Portalia.Core.Enum.FolderType;

namespace Portalia.Controllers
{
    [Authorize]
    public class ProposalController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly IProposalService _proposalService;
        private readonly IUserProfileAttributeService _userProfileAttributeService;
        private readonly IUserProfileService _userProfileService;
        private readonly IWorkContractService _workContractService;
        public ProposalController(
            IProposalService proposalService,
            IDocumentService documentService,
            IUserProfileAttributeService userProfileAttributeService, 
            IUserProfileService userProfileService,
            IWorkContractService workContractService)
        {
            _proposalService = proposalService;
            _documentService = documentService;
            _userProfileAttributeService = userProfileAttributeService;
            _userProfileService = userProfileService;
            _workContractService = workContractService;
        }
       
        public ActionResult Index(string userId)
        {
            if (User.Identity.IsEmployee())
            {
                _proposalService.UpdateStatus(User.Identity.GetUserId(), ProposalStatus.Approved);
            }

            if (!string.IsNullOrEmpty(userId) && User.IsInRole(Roles.Administrator.ToString()))
            {

                ViewBag.UserId = userId;
                _userProfileService.UpdateStatusForNewUser(userId,false);
                return View(_proposalService.GetByUser(userId));
            }
            ViewBag.UserId = User.Identity.GetUserId();
            return View(_proposalService.GetByUser(User.Identity.GetUserId()));
        }

        [HttpGet]
        public ActionResult Create(string userId)
        {
            var model = new CreateProposalViewModel();
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.Identity.GetUserId();
                model.UserId = userId;
            }
            else
            {
                model.UserId = userId;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateProposalViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                model.UserId = User.Identity.GetUserId();
            }
            _proposalService.Create(model.UserId, model.ProjectName, model.ClientName,
                model.Description);
            return RedirectToAction("Index", new { userId = model.UserId });
        }

        [HttpGet]
        public ActionResult SendProposal(int proposalId, string userId)
        {
            if (User.IsInRole(Roles.Administrator.ToString()) && !userId.IsNullOrWhiteSpace())
            {
                return PartialView("_CreateProposalWizard",
                    _proposalService.GetById(proposalId, userId));
            }
            return PartialView("_CreateProposalWizard", _proposalService.GetById(proposalId, User.Identity.GetUserId()));
        }


        public ActionResult CheckUserValidation(string userId)
        {
            var userProfileId = string.Empty;
            userProfileId = string.IsNullOrEmpty(userId) ? User.Identity.GetUserId() : userId;
            var userProfileResource = Portalia.Resources.UserProfile.ResourceManager;
            var isValidUserProfile = _userProfileAttributeService.IsValidUserProfile(userProfileId).Select(s => userProfileResource.GetString(s.RemoveSpace()));

            var validUserProfile = isValidUserProfile as string[] ?? isValidUserProfile.ToArray();
            if (validUserProfile.Any())
            {
                return Json(new { result = false, error = string.Join(", ", validUserProfile) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountMissingField()
        {
            string result = _userProfileAttributeService.CountMissingField(User, Portalia.Resources.UserProfile.ResourceManager);

            return Content(result);
        }

        public ActionResult CountMissingFieldByAttributeType(int userId, string attributeType)
        {
            var result = _userProfileAttributeService.CountMissingField(userId, attributeType);
            return Content(result == 0 ? string.Empty : result.ToString());
        }

        [HttpPost]
        public JsonResult UpdateProposalStatus(int proposalId, string userId)
        {

            List<Document> documents;
            if (!userId.IsNullOrWhiteSpace() && User.IsInRole(Roles.Administrator.ToString()))
            {
                documents = _documentService.GetByProposalId(proposalId, userId);
            }
            else
            {
                documents = _documentService.GetByProposalId(proposalId, User.Identity.GetUserId());
            }

            if (documents.Any())
            {
                var proposal = _proposalService.UpdateStatus(proposalId, ProposalStatus.SentProposal);
                var isSendMail = bool.Parse(ConfigurationManager.AppSettings["IsSendMail"]);
                if (isSendMail)
                {
                    MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
                    var resultSendMail = MultiBrandingHelper.SendEmail(new SendEmailParameter()
                    {
                        ApplicationName = "Multibranding",
                        Holding = "Portalia",
                        TemplateName = "ContactUs",
                        Variable = JsonConvert.SerializeObject(new
                        {
                            EmailContent = $"<p>Hello,</p>" +
                                   $"<p>You have a new project proposal, please connect to https://portalia.fr/Proposal?userId={userId}</p>" +
                                   $"<p>You can also find this candidate on https://arp.amaris.com/SMART/Search/Search/SimpleSearch?q={User.Identity.GetUserName()}</p>" +
                                   $"<p>Portalia Team</p>",
                            Subject = $"[Portalia] [{User.Identity.GetUserName()}] sent a project proposal",
                            SendFrom = ConfigurationManager.AppSettings["MailFrom"],
                            SendTo = ConfigurationManager.AppSettings["HrEmails"]
                        })
                    });
                }

                return Json(new { Message = "OK" });
            }
            if (!documents.Any())
            {
                return Json(new { Message = Resources.Proposal.PleaseUploadYourDocumentBeforeSend });
            }
            return Json("");
        }

        [HttpGet]
        public ActionResult Detail(int proposalId, string userId)
        {
            Proposal proposal;
            ProposalDetailViewModel proposalDetailViewModel;

            if (!userId.IsNullOrWhiteSpace() && User.IsInRole(Roles.Administrator.ToString()))
            {
                proposal = _proposalService.GetById(proposalId, userId);
                proposalDetailViewModel = GetProposalViewModel(proposal);

                return View(proposalDetailViewModel);
            }

            proposal = _proposalService.GetById(proposalId, User.Identity.GetUserId());

            if (proposal == null)
            {
                return RedirectToAction("Error", "Home");
            }

            proposalDetailViewModel = GetProposalViewModel(proposal);

            return View(proposalDetailViewModel);
        }

        public ActionResult Delete(int proposalId, string userId)
        {
            if (User.IsInRole(Roles.Administrator.ToString()) && !userId.IsNullOrWhiteSpace())
            {
                return PartialView("_DeleteProposal", _proposalService.GetById(proposalId, userId));
            }
            var model = _proposalService.GetById(proposalId, User.Identity.GetUserId());
            if (model == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return PartialView("_DeleteProposal", _proposalService.GetById(proposalId, User.Identity.GetUserId()));
        }
        [HttpPost]
        public ActionResult Delete(Proposal proposal)
        {
            if (User.IsInRole(Roles.Administrator.ToString()) && !proposal.UserId.IsNullOrWhiteSpace())
            {
                _proposalService.DeleteProposal(proposal.ProposalId, proposal.UserId);
            }
            else
            {
                _proposalService.DeleteProposal(proposal.ProposalId, User.Identity.GetUserId());
            }

            return RedirectToAction("Index", "Proposal");
        }

        [HttpGet]
        public ActionResult GetDocumentsByProposal(int proposalId, FolderType folderType, string userId)
        {
            if (proposalId < 1)
            {
                var proposal = _proposalService.GetById(User.Identity.GetUserId());
                proposalId = proposal.ProposalId;
            }

            var documentViewModel = new DocumentViewModel
            {
                FolderType = folderType,
                Documents = _documentService.GetAll(userId, folderType),
                UserId = userId,
                ProposalId = proposalId
            };
            return PartialView("_Documents", documentViewModel);
        }

        [HttpGet]
        public ActionResult UploadDocumentForUser(string userId, int proposalId, FolderType folderType)
        {
            var userDocumentViewModel = new UserDocumentViewModel()
            {
                FolderType = folderType,
                ProposalId = proposalId
            };
            if (User.IsInRole(Roles.Administrator.ToString()))
            {
                userDocumentViewModel.UserId = userId;
            }
            else
            {
                userDocumentViewModel.UserId = User.Identity.GetUserId();
            }

            return PartialView("_UploadDocumentForUser", userDocumentViewModel);
        }
        public ActionResult Download(int documentId, string userId)
        {
            bool haveDocument;
            if (User.IsInRole(Roles.Administrator.ToString()) && !userId.IsNullOrWhiteSpace())
            {
                haveDocument = _documentService.HaveDocument(userId, documentId);
            }
            else
            {
                haveDocument = _documentService.HaveDocument(User.Identity.GetUserId(), documentId);
            }

            if (!haveDocument)
            {
                return HttpNotFound();
            }
            var document = _documentService.GetById(documentId);
            return File(document.FileBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.Name);
        }
        public ActionResult DownloadDefault(int documentId)
        {
            var document = _documentService.GetById(documentId);
            return File(document.FileBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.Name);
        }

        public ActionResult MySpace(string userId = "")
        {
            Proposal proposal;
            ProposalDetailViewModel proposalDetailViewModel;

            if (!userId.IsNullOrWhiteSpace())
            {
                // if logged employee is admin, then he can see another employee proposal
                if (User.IsInRole(Roles.Administrator.ToString()))
                {
                    proposal = _proposalService.GetById(userId);
                    proposalDetailViewModel = GetProposalViewModel(proposal);

                    return View("Detail", proposalDetailViewModel);
                }

                // else, if logged employee access to their proposal, let them go.
                if (userId == User.Identity.GetUserId())
                {
                    return RedirectToAction("MySpace", new {userId = string.Empty});
                }

                // else, go to 404 page because they don't fill right param
                return RedirectToAction("Error", "Home");
            }

            // if userId is null, then go to their proposal
            var model = _proposalService.GetById(User.Identity.GetUserId());

            // if proposal is not found, redirect to 404 page.
            if (model == null)
            {
                return RedirectToAction("Error", "Home");
            }
            // Convert Proposal to View Model
            proposalDetailViewModel = GetProposalViewModel(model);

            // if found, go to their proposal
            return View("Detail", proposalDetailViewModel);
        }

        public ActionResult Documents(int proposalId, FolderType folderType, string userId = "")
        {
            // if userId is null, then go to their proposal
            var model = _proposalService.GetById(User.Identity.GetUserId());

            // if proposal is not found, redirect to 404 page.
            if (model == null)
            {
                return RedirectToAction("Error", "Home");
            }
            // Convert Proposal to View Model
            var proposalDetailViewModel = GetProposalViewModel(model);

            proposalDetailViewModel.Document = new DocumentViewModel
            {
                FolderType = folderType,
                Documents = _documentService.GetAll(userId, folderType),
                UserId = userId,
                ProposalId = proposalId
            };

            // if found, go to their proposal
            return View("Detail", proposalDetailViewModel);
        }

        private ProposalDetailViewModel GetProposalViewModel(Proposal proposal)
        {
            if (proposal == null)
            {
                return new ProposalDetailViewModel();
            }

            // Convert Proposal to View Model
            var proposalDetailViewModel = proposal.ToProposalDetailViewModel();
            var employeeStatus = new List<string>() { "In", "Recruited", "ExitConfirmed", "Out" };
            var canSeeMenu = User.Identity.IsEmployee() || User.IsInRole("Administrator") || employeeStatus.Contains(User.Identity.EmployeeStatus());
            var canSeeContract = _workContractService.HasContract(User.Identity.GetUserId());

            proposalDetailViewModel.CanSeeMenu = canSeeMenu;
            proposalDetailViewModel.CanSeeContract = canSeeContract;
            proposalDetailViewModel.EmployeeStatus = User.Identity.EmployeeStatus();
            proposalDetailViewModel.IsEmployee = User.Identity.IsEmployee();

            return proposalDetailViewModel;
        }
        
    }
}