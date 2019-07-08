using Microsoft.AspNet.Identity;
using Portalia.Core.Dtos;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using System;
using System.Linq;
using System.Web.Mvc;
using Portalia.Core.Dtos.Message;
using Portalia.ViewModels;
using Portalia.Extentions;
using Portalia.Core.Helpers;
using Portalia.Core.Infrastructure;
using System.Collections.Generic;
using System.Net;
using Portalia.ViewModels.WorkContracts;
using System.Threading.Tasks;
using Portalia.Core.Entity;
using Portalia.Core.StaticContent;
using System.Configuration;

namespace Portalia.Controllers
{
    [Authorize]
    public class WorkContractController : BaseController
    {
        private readonly IWorkContractService _workContactService;
        private readonly IRefDataService _refDataService;
        private readonly IUserProfileService _userProfileService;
        private readonly IDataSourceService _dataSourceService;
        private readonly IApplicationForm _applicationForm;
        private readonly IDocumentService _documentService;
        private readonly IProposalService _proposalService;

        public WorkContractController(IWorkContractService workContactService, 
            IRefDataService refDataService,
            IUserProfileService userProfileService, 
            IDataSourceService dataSourceService, IApplicationForm applicationForm,
            IDocumentService documentService, IProposalService proposalService)
        {
            _workContactService = workContactService;
            _refDataService = refDataService;
            _userProfileService = userProfileService;
            _dataSourceService = dataSourceService;
            _applicationForm = applicationForm;
            _documentService = documentService;
            _proposalService = proposalService;
        }

        public ActionResult OpenWorkContract(string employeeId)
        {
            try
            {
                // Find work contract by employeeId
                var workContractId = _workContactService.OpenNewWorkContract(User.Identity.GetUserId(), employeeId);
                // If work contract not found, return error message
                if (workContractId == 0)
                {
                    return Json(new
                    {
                        Message = "Failed to open work contract form.",
                        IsSuccess = false
                    }, JsonRequestBehavior.AllowGet);
                }

                // Find contract owner
                var contractOwner = _userProfileService.GetUsersIdentityById(employeeId);

                // If contract owner found
                if (contractOwner != null)
                {
                    // Generate Url which links to the work contract owner
                    var workContractUrl = Url.Action("Index", "WorkContract",
                        new {workContractId = workContractId}, "https");

                    // Send email to work contract owner
                    Task task = new Task(() => {
                        //Send email to director in order to notify the submission
                        _workContactService.NotifyWorkContractOwner(contractOwner.Email, contractOwner.FirstName,
                        workContractUrl);
                    });
                    task.Start();

                    // Sync data of candidate to TOAST
                    var employeePhoneNumber = _userProfileService.GetUsersIdentityById(employeeId)?.PhoneNumber;
                    var exception = new Exception();
                    var applicationFormId =
                        _applicationForm.AddApplyForm(new ApplicationFormDto
                        {
                            Email = contractOwner.Email,
                            LastName = contractOwner.LastName,
                            FirstName = contractOwner.FirstName,
                            PhoneNumber = employeePhoneNumber
                        }, exception);
                }
                else // if not found, log error
                {
                    LoggerHelpers.Error("User not found. Cannot send notification for open contract");
                }

                // Return success message
                return Json(new
                {
                    Message = "Opened form successfully. The user can access the form now.",
                    IsSuccess = true,
                    Data = GetUserInfoRowAsHtml(employeeId)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Catch error, log error
                LoggerHelpers.Error(ex.Message);
                // Return error message
                return Json(new
                {
                    Message = ex.Message,
                    IsSuccess = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CloseWorkContract(int workContractId, string employeeId)
        {
            try
            {
                // Close work contract and get the work contract status ID
                var workContractStatusId = _workContactService.SetWorkContractStatus(workContractId,
                    WorkContractStatusEnum.Disabled, User.Identity.GetUserId());

                // If there is no status returned, return error message
                if (workContractStatusId == 0)
                {
                    return Json(new
                    {
                        Message = "Failed to close work contract form.",
                        IsSuccess = false
                    }, JsonRequestBehavior.AllowGet);
                }

                // Find employee info (DTO object)
                var pagingUserItemDto = _userProfileService.GetPagingUserItem(employeeId);
                // Convert DTO object to View Model
                var pagingUserItemViewModel = pagingUserItemDto.ToPagingUserItemViewModel();

                // Return success message
                return Json(new
                {
                    Message = "Closed form successfully. The user cannot access the form anymore.",
                    IsSuccess = true,
                    Data = RenderRazorViewToString("~/Views/Administrator/_User.cshtml", pagingUserItemViewModel)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Catch and log error
                LoggerHelpers.Error(ex.Message);
                // Return error message
                return Json(new
                {
                    Message = ex.Message,
                    IsSuccess = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Index(int workContractId)
        {
            try
            {
                bool isAdmin = User.IsInRole("Administrator");
                bool isInputDisabled = false;

                // Get work contract by work contract ID
                WorkContractDto workContractDto = _workContactService.Get(workContractId);

                //Get last comment of WC
                WorkContractCommentDto contractCommentDto = _workContactService.GetMostRecentCommentOfWorkContract(workContractId);
                workContractDto.LastCommentDate = contractCommentDto?.CreatedDate;

                // Only the owner or admin can open
                if (workContractDto == null
                    || (workContractDto.UserId != User.Identity.GetUserId() && !isAdmin))
                {
                    return HttpNotFound();
                }

                switch (workContractDto.WorkContractStatusId)
                {
                    case WorkContractStatusEnum.Disabled:
                        // Work contract must not be Disabled
                        return View("Error", WorkContractStatusEnum.Disabled);
                    case WorkContractStatusEnum.Validated:
                        if (!isAdmin)
                        {
                            // Work contract must not be Validated
                            return View("Error", WorkContractStatusEnum.Validated);
                        }
                        else
                        {
                            isInputDisabled = true;
                            break;
                        }
                    case WorkContractStatusEnum.PendingOnCandidate:
                    case WorkContractStatusEnum.PendingOnCandidateRevision:
                        // Pending on Candidate - show form to admin as readonly
                        if (isAdmin)
                        {
                            isInputDisabled = true;
                        }
                        break;
                }

                // Convert DTO object to View Model
                WorkContractViewModel model = workContractDto.ToWorkContractViewModel();

                BindDatasourceForModel(model);
                model.IsInputDisabled = isInputDisabled;

                // Return view with view model
                return View(model);
            }
            catch (Exception ex)
            {
                // Catch and log error
                LoggerHelpers.Error(ex.Message);
                // Return HttpNotFound()
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult SaveWorkContract(WorkContractViewModel viewModel)
        {
            // Get message after saving work contract
            var message =
                HandleWorkContractActionAndReturnMessage(viewModel, WorkContractStatusEnum.PendingOnCandidate);

            // Return JSON message
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitWorkContract(WorkContractViewModel viewModel)
        {
            try
            {
                // Get message after submitting work contract
                MessageViewModel message =
                    HandleWorkContractActionAndReturnMessage(viewModel, WorkContractStatusEnum.Validated);
                if (!message.HasError)
                {
                    _applicationForm.UpdateApplicationFormFromWorkContract(new ApplicationFormDto
                    {
                        Email = viewModel.Email,
                        LastName = viewModel.LastName,
                        FirstName = viewModel.FirstName,
                        BirthDate = viewModel.DateOfBirth
                    });

                    // Generate Url which links to the work contract owner
                    string workContractUrl = Url.Action("Index", "WorkContract",
                        new { workContractId = viewModel.ContractId }, "https");

                    Task task = new Task(() => {
                        //Send email to director in order to notify the submission
                        _workContactService.SendEmail(new
                        {
                            Username = viewModel.FirstName + " " + viewModel.LastName,
                            Url = workContractUrl
                        }, EmailTemplateUtility.NotifyDirectorForReview);
                    });
                    task.Start(); 
                }
                // Return JSON message
                return Json(message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult RequestCandidateToRevise(WorkContractViewModel viewModel)
        {
            // Get message after sending request candidate to revise work contract
            MessageViewModel message =
                HandleWorkContractActionAndReturnMessage(viewModel, WorkContractStatusEnum.PendingOnCandidateRevision);

            //Send email to candidate in order to notify the revision
            if (!message.HasError)
            {
                // Find contract owner
                string candidateEmail = _workContactService.GetUserEmailByWorkContractId(viewModel.ContractId);

                if(string.IsNullOrEmpty(candidateEmail))
                {
                    LoggerHelpers.Error($"Can not find email of work contract:{viewModel.ContractId}");
                    message.HasError = true;
                    message.Message = "Can not find candidate email";
                }
                else
                {
                    // Generate Url which links to the work contract owner
                    string workContractUrl = Url.Action("Index", "WorkContract",
                        new { workContractId = viewModel.ContractId }, "https");
                    AspNetUserExtraInfo candidateProfile = _userProfileService.GetUsersIdentityByEmail(candidateEmail);
                    string candidateFullname = candidateProfile != null ? candidateProfile.FullName : $"{viewModel.FirstName} {viewModel.LastName}";
                    Task notifyDirectorTask = new Task(() =>
                    {
                        //Send email to director in order to notify the submission
                        _workContactService.SendEmail(new
                        {
                            Username = candidateFullname,
                            WCUrl = workContractUrl,
                            UserEmail = candidateEmail

                        }, EmailTemplateUtility.ReviseCandidateToUpdateWorkContract);
                    });

                    notifyDirectorTask.Start();
                }
            }

            // Return JSON message
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateWorkContract(WorkContractViewModel viewModel)
        {
            // Save work contract data, update status to Validated
            var message =
                HandleWorkContractActionAndReturnMessage(viewModel, WorkContractStatusEnum.Validated);

            // If error, return JSON error message
            if (message.HasError)
            {
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            // Generate field data for Work Contract app, run in background
            _workContactService.GenerateFieldData(viewModel.ContractId);
            // Update data for Application Form
            var userId = _workContactService.Get(viewModel.ContractId)?.UserId;
            var email = _userProfileService.GetUsersIdentityById(userId)?.Email;
            _applicationForm.UpdateApplicationFormFromWorkContract(new ApplicationFormDto
            {
                Email = email,
                LastName = viewModel.LastName,
                FirstName = viewModel.FirstName,
                BirthDate = viewModel.DateOfBirth
            });
            //update candidate profile first name, last name
            _userProfileService.UpdateProfileIdentity(new Core.Entity.AspNetUser()
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Id = userId
            });
            
            //update candidate profile attributes 
            var candidateProfile = _userProfileService.GetUserProfile(userId);
            _userProfileService.UpdateProfileAttribute(candidateProfile, viewModel.FirstName, viewModel.LastName);

            // Generate Url which links to the work contract owner
            var workContractUrl = Url.Action("Index", "WorkContract",
                new { workContractId = viewModel.ContractId }, "https");

            Task task = new Task(() => {
                //Send email to director in order to notify the submission
                _workContactService.SendEmail(new
                {
                    Username = viewModel.FirstName + " " + viewModel.LastName,
                    Email = email,
                    ARPUrl=ConfigurationManager.AppSettings["ARPWC_Url"],
                    WCUrl= workContractUrl
                }, EmailTemplateUtility.NotifyDirectorForReview);
            });
            task.Start();

            // Return JSON success message
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadWorkContract(UploadWorkContractViewModel uploadWorkContractViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid data", JsonRequestBehavior.AllowGet);
            }

            // Get proposal of user
            var proposalId = _proposalService.GetById(uploadWorkContractViewModel.UserId)?.ProposalId ?? 0;
            // Convert view model to upload document DTO
            var uploadDocumentDto = uploadWorkContractViewModel.ToUploadDocumentDto(proposalId);
            // Add document to database
            var uploadFileMessageDto = _documentService.UploadFile(uploadDocumentDto);

            // If upload failed, return error message
            if (uploadFileMessageDto.HasError)
            {
                return Json("Cannot upload contract", JsonRequestBehavior.AllowGet);
            }

            // else, get just-added document ID
            var documentId = (int)uploadFileMessageDto.Data;
            // Reference the contract to work contract
            var setNewDocumentToWorkContractMessageViewModel =
                _workContactService.UploadDocument(uploadWorkContractViewModel.UserId, documentId)
                .ToMessageViewModel();

            // If reference failed, return error message
            if (setNewDocumentToWorkContractMessageViewModel.HasError)
            {
                return Json("Cannot upload contract", JsonRequestBehavior.AllowGet);
            }

            // else, send email to notify candidate about the contract
            var requestUrl = Url.Action("MySpace", "Proposal",
                new {  }, "https"); ;

            LoggerHelpers.Debug($"Request URL:{requestUrl}");

            _workContactService.SendEmailToNotifyCandidateWorkContractIsReady(uploadWorkContractViewModel.UserId, requestUrl);

            setNewDocumentToWorkContractMessageViewModel.Data =
                GetUserInfoRowAsHtml(uploadWorkContractViewModel.UserId);

            // return success message
            return Json(setNewDocumentToWorkContractMessageViewModel, JsonRequestBehavior.AllowGet);
        }

        // This method is used for (Save/Submit/Request Candidate To Vise) Work Contract
        // We update Work Contract with the desired Work Contract Status
        private MessageViewModel HandleWorkContractActionAndReturnMessage(WorkContractViewModel viewModel,
            WorkContractStatusEnum workContractStatus)
        {
            //Remove validation for Visa Permit No if Country is not France
            if (viewModel.Country == "FR" && viewModel.IsSubmit)
                ModelState.Remove("VisaPermitNo");

            if((int)workContractStatus!=(int)WorkContractStatusEnum.PendingOnCandidateRevision)
                ModelState.Remove("Comment");

            // Only validate data when users submit form
            // If POST data is not valid, return JSON error message
            if (viewModel.IsSubmit && !ModelState.IsValid)
            {
                return MessageDto.GetErrorMessage("Invalid data").ToMessageViewModel();
            }

            //Create new skill if there is any new item
            CreateNewSkills(viewModel); 

            // Convert ViewModel to DTO
            var workContractDto = viewModel.ToWorkContractDto();
            // Set work contract status
            workContractDto.WorkContractStatus.StatusId = workContractStatus;
            // Set updated person
            workContractDto.UpdatedBy = User.Identity.GetUserId();
            // Set updated date
            workContractDto.UpdatedDate = DateTime.UtcNow;
            // Update work contract and get returned message

            BindDatasourceForDTOModel(workContractDto);

            var message = _workContactService.Update(workContractDto);

            return message.HasError // If the returned message is an error message
                ? MessageDto.GetErrorMessage().ToMessageViewModel() // then, Convert DTO to error message view model
                : MessageDto.GetSuccessMessage().ToMessageViewModel(); // else, Convert DTO to success message view model
        }

        private void CreateNewSkills(WorkContractViewModel model)
        {
            if(!string.IsNullOrEmpty(model.Skills))
            {
                List<int> lstSkills = new List<int>();
                string[] arrStrSkills = model.Skills.Split(new char[] { ',' });
                int temp;

                foreach (string skill in arrStrSkills)
                {
                    if(int.TryParse(skill,out temp))
                    {
                        lstSkills.Add(temp);
                    }
                    else
                    {
                        lstSkills.Add(_workContactService.CreateSkill(skill));
                    }
                }

                model.Skills = String.Join(",", lstSkills);
            }
        }

        /// <summary>
        /// Bind static datasource for dropdown fields
        /// </summary>
        /// <param name="model">Work contract view model</param>
        private void BindDatasourceForModel(WorkContractViewModel model)
        {
            SelectListItem emptyValue = new SelectListItem()
            {
                Value="",
                Text= "Veuillez choisir",
                Selected=true
            };

            //Country list must get from BirthPlace table
            model.LstCountries = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Country, () =>
            {

                List<SelectListItem> lstDataSource= _dataSourceService.GetBirthPlace().OrderBy(x=>x.Label).Select(x => new SelectListItem()
                {
                    Value = x.ID,
                    Text = x.Label,
                    Selected=false
                }).ToList();

                lstDataSource.Insert(0, emptyValue);

                return lstDataSource;
            });

            model.LstTitles = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Title, () =>
            {
                List<SelectListItem> lstDataSource = _refDataService.GetRefDataByCode(CacheKey.Title).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();

                lstDataSource.Insert(0, emptyValue);

                return lstDataSource;
            });

            //Nationalities list must get from Country table
            model.LstNationalities = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Country, () =>
            {
                List<SelectListItem> lstDataSource = _dataSourceService.GetCountryForWC().OrderBy(x => x.Value).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();

                lstDataSource.Insert(0, emptyValue);

                return lstDataSource;
            });

            model.LstCurrencies = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Currency, () =>
            {
                return _dataSourceService.GetCurrencyForWC().OrderBy(x => x.Value).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();
            });

            model.LstBasic = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.DailyBasic, () =>
            {
                return _refDataService.GetRefDataByCode(CacheKey.DailyBasic).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();
            });

            if (!string.IsNullOrEmpty(model.Skills))
            {
                string[] arrStrSkills = model.Skills.Split(new char[] { ',' });
                List<int> arrSkills = new List<int>();
                int temp;

                foreach (string item in arrStrSkills)
                {
                    if (int.TryParse(item, out temp))
                    {
                        arrSkills.Add(temp);
                    }
                }

                model.LstSkills = _dataSourceService.GetSpecificSkills(arrSkills).Select(x => new SelectListItem()
                {
                    Value = x.SkillId + "",
                    Text = x.Label
                }).ToList();
            }

            if (!string.IsNullOrEmpty(model.City))
            {
                model.LstCities = _dataSourceService.GetSmartCity().Where(x => x.ID == Convert.ToInt32(model.City)).Select(x => new SelectListItem()
                {
                    Value = x.ID + "",
                    Text = x.Name,
                    Selected = false
                }).ToList();
            }
        }

        /// <summary>
        /// Bind static datasource for dropdown fields
        /// </summary>
        /// <param name="model">Work contract view model</param>
        private void BindDatasourceForDTOModel(WorkContractDto model)
        {
            SelectListItem emptyValue = new SelectListItem()
            {
                Value = "",
                Text = "Veuillez choisir",
                Selected = true
            };

            //Country list must get from BirthPlace table
            model.LstCountries = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Country, () =>
            {

                List<SelectListItem> lstDataSource = _dataSourceService.GetBirthPlace().OrderBy(x => x.Label).Select(x => new SelectListItem()
                {
                    Value = x.ID,
                    Text = x.Label,
                    Selected = false
                }).ToList();

                lstDataSource.Insert(0, emptyValue);

                return lstDataSource;
            });

            model.LstTitles = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Title, () =>
            {
                List<SelectListItem> lstDataSource = _refDataService.GetRefDataByCode(CacheKey.Title).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();

                lstDataSource.Insert(0, emptyValue);

                return lstDataSource;
            });

            //Nationalities list must get from Country table
            model.LstNationalities = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Country, () =>
            {
                List<SelectListItem> lstDataSource = _dataSourceService.GetCountryForWC().OrderBy(x => x.Value).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();

                lstDataSource.Insert(0, emptyValue);

                return lstDataSource;
            });

            model.LstCurrencies = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.Currency, () =>
            {
                return _dataSourceService.GetCurrencyForWC().OrderBy(x => x.Value).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();
            });

            model.LstBasic = Caching.GetCache<IEnumerable<SelectListItem>>(CacheKey.DailyBasic, () =>
            {
                return _refDataService.GetRefDataByCode(CacheKey.DailyBasic).Select(x => new SelectListItem()
                {
                    Value = x.Key,
                    Text = x.Value,
                    Selected = false
                }).ToList();
            });

            if (!string.IsNullOrEmpty(model.Skills))
            {
                string[] arrStrSkills = model.Skills.Split(new char[] { ',' });
                string[] arrOldSkills = _workContactService.GetCurrentSkills(model.ContractId)?.Split(new char[] { ','});
                List<int> arrSkills = new List<int>();
                int temp;

                //Get list of items from new vlaue
                foreach (string item in arrStrSkills)
                {
                    if (int.TryParse(item, out temp))
                    {
                        arrSkills.Add(temp);
                    }
                }

                //Combine current list items with new list item
                if(arrOldSkills!=null)
                {
                    foreach(string item in arrOldSkills)
                    {
                        if (int.TryParse(item, out temp) && !arrSkills.Contains(temp))
                        {
                            arrSkills.Add(temp);
                        }
                    }
                }


                model.LstSkills = _dataSourceService.GetSpecificSkills(arrSkills).Select(x => new SelectListItem()
                {
                    Value = x.SkillId + "",
                    Text = x.Label,
                    Selected = false
                }).ToList();
            }

            if (!string.IsNullOrEmpty(model.City))
            {
                model.LstCities = _dataSourceService.GetSmartCity().Where(x => x.CountryId==model.Country).Select(x => new SelectListItem()
                {
                    Value = x.ID + "",
                    Text = x.Name,
                    Selected = false
                }).ToList();
            }

        }

        private string GetUserInfoRowAsHtml(string userId)
        {
            // Find employee info (DTO object)
            var pagingUserItemDto = _userProfileService.GetPagingUserItem(userId);
            // Convert DTO object to View Model
            var pagingUserItemViewModel = pagingUserItemDto.ToPagingUserItemViewModel();

            return RenderRazorViewToString("~/Views/Administrator/_User.cshtml", pagingUserItemViewModel);
        }

    }
}