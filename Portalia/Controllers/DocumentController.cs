using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using nClam;
using Newtonsoft.Json;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using Portalia.ViewModels;
using Repository.Pattern.Infrastructure;
using FolderType = Portalia.Core.Enum.FolderType;
using Portalia.Core.Dtos;
using Portalia.Core.Helpers;

namespace Portalia.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly IUserProfileAttributeService _profileAttributeService;
        private readonly IUserProfileService _userProfileService;
        private readonly IApplicationForm _applicationForm;
        private readonly IProposalService _proposalService;

        public DocumentController(
            IDocumentService documentService,
            IUserProfileAttributeService profileAttributeService,
            IUserProfileService userProfileService,
            IApplicationForm applicationForm,
            IProposalService proposalService)
        {
            _documentService = documentService;
            _profileAttributeService = profileAttributeService;
            _userProfileService = userProfileService;
            _applicationForm = applicationForm;
            _proposalService = proposalService;
        }

        public JsonResult DeleteDocument(Document document)
        {
            if (User.IsInRole(Roles.Administrator.ToString()) && !document.Proposal.UserId.IsNullOrWhiteSpace())
            {
                _documentService.Delete(document.DocumentId, document.Proposal.UserId);
            }
            else
            {
                _documentService.Delete(document.DocumentId, User.Identity.GetUserId());
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UploadFile(string userId, int proposalId, FolderType folderType)
        {

            var isSavedSuccessfully = true;
            var fName = "";
            var document = new Document();
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];
                    if (file != null)
                    {
                        fName = file.FileName;
                        if (file.ContentLength > 0)
                        {
                            var target = new MemoryStream();
                            file.InputStream.CopyTo(target);
                            var data = target.ToArray();
                            var resultScan = await VirusScanner(data);
                            if (resultScan != ClamScanResults.Clean)
                            {
                                return Json(isSavedSuccessfully = false, JsonRequestBehavior.AllowGet);
                            }
                            document.FileBinary = data;
                            document.ProposalId = proposalId;
                            document.Name = file.FileName.Replace(" ", string.Empty);
                            document.FolderType = folderType;
                            _documentService.Create(document);
                            document.Proposal = new Proposal()
                            {
                                UserId = userId
                            };
                            document.FileBinary = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            return
                Json(isSavedSuccessfully
                    ? new { Message = fName, File = document }
                    : new { Message = "Error in saving file", File = document }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UploadUserProfileFile(int userProfileId, int attributeDetailId,
            int userProfileAttributeId, string attributeName)
        {
            var isSavedSuccessfully = true;
            attributeName = attributeName.Replace(" ", String.Empty);
            var userProfileupload = new UploadUserProfileModel();
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];

                    //Save file content goes here
                    fName = file.FileName;
                    fName = fName.Replace(" ", String.Empty);

                    if (file != null && file.ContentLength > 0)
                    {
                        var target = new MemoryStream();

                        file.InputStream.CopyTo(target);

                        var data = target.ToArray();
                        var resultScan = await VirusScanner(data);

                        if (resultScan != ClamScanResults.Clean)
                        {
                            return Json(isSavedSuccessfully = false, JsonRequestBehavior.AllowGet);
                        }

                        var storedFileName = $"{userProfileId}_{attributeDetailId}_{attributeName}_{fName}";
                        _profileAttributeService.UpdateAttribute(userProfileAttributeId, String.Empty, storedFileName,
                            data);
                        userProfileupload.UserProfileId = userProfileId;
                        userProfileupload.AttributeDetailId = attributeDetailId;
                        userProfileupload.UserProfileAttributeId = userProfileAttributeId;
                        userProfileupload.Name = storedFileName;
                        userProfileupload.AttributeName = attributeName;

                        //Transfer CV file to TOAST
                        if (attributeName == "CV")
                        {
                            var ex = default(Exception);
                            var userExtraInfo = _userProfileService.GetUserById(userProfileId);
                            var email = string.Empty;

                            email = userExtraInfo == default(AspNetUserExtraInfo) ? string.Empty : userExtraInfo.Email;

                            int applicationFormId =
                                _applicationForm.AddApplyForm(new ApplicationFormDto() {Email = email}, ex);

                            LoggerHelpers.Info($"ApplicationFormId: {applicationFormId}");

                            if (applicationFormId > 0)
                            {
                                var applicationDocumentDto = _applicationForm.CreateApplicationDocument(file, data, ex);

                                LoggerHelpers.Info(
                                    $"Serialize document object: {JsonConvert.SerializeObject(applicationDocumentDto)}");

                                if (applicationDocumentDto != null)
                                {
                                    var status = _applicationForm.AddApplyDocument(applicationFormId,
                                        applicationDocumentDto, ex);

                                    LoggerHelpers.Info($"Document Status: {status.ToString()}");

                                    if (status != AmarisEnum.SourcingStatus.WebsiteError)
                                    {
                                        var result =
                                            _applicationForm.UpdateApplicationFormStatus(applicationFormId,
                                                (int) status, ex);

                                        LoggerHelpers.Info($"Update ApplicationFormStatus Result: {result}");
                                    }
                                }
                            }

                            if (ex != default(Exception))
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                LoggerHelpers.Error(ex);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                LoggerHelpers.Error(ex);
            }

            return
                Json(isSavedSuccessfully
                    ? new {Message = fName, File = userProfileupload}
                    : new {Message = "Error in saving file", File = userProfileupload}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUserProfileDocument(UploadUserProfileModel document)
        {
            _profileAttributeService.UpdateAttribute(document.UserProfileAttributeId, String.Empty, null);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UploadUserPicture(string userId)
        {
            var isSavedSuccessfully = false;
            var fName = string.Empty;
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];

                    //Save file content goes here
                    fName = file.FileName;
                    fName = fName.Replace(" ", String.Empty);
                    if (file != null && file.ContentLength > 0)
                    {
                        var target = new MemoryStream();
                        file.InputStream.CopyTo(target);
                        var data = target.ToArray();
                        var resultScan = await VirusScanner(data);
                        if (resultScan != ClamScanResults.Clean)
                        {
                            return Json(isSavedSuccessfully = false, JsonRequestBehavior.AllowGet);
                        }
                        var userProfile = _userProfileService.GetUserProfile(userId);
                        userProfile.PictureName = $"Picture_{userId}{Path.GetExtension(fName)}";
                        userProfile.PictureFileBinary = data;
                        userProfile.ObjectState = ObjectState.Modified;
                        _userProfileService.UpdateUserProfile(userProfile);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            return
                Json(isSavedSuccessfully = true, JsonRequestBehavior.AllowGet);
        }

        // Get My Tutorials documents
        [HttpGet]
        public ActionResult Tutorials()
        {
            string userId = User.Identity.GetUserId();
            int? proposalId = _proposalService.GetById(userId)?.ProposalId;
            var documentViewModel = GetDocumentsByProposal(proposalId ?? 0, FolderType.EndOfContractBonuses, userId);
            return View("Index", documentViewModel);
        }
        
        private DocumentViewModel GetDocumentsByProposal(int proposalId, FolderType folderType, string userId)
        {
            var documentViewModel = new DocumentViewModel
            {
                FolderType = folderType,
                Documents = _documentService.GetAll(userId, folderType),
                UserId = userId,
                ProposalId = proposalId
            };
            return documentViewModel;
        }

        private async Task<ClamScanResults> VirusScanner(byte[] fileBytes)
        {
            try
            {
                int portScan = int.Parse(ConfigurationManager.AppSettings["PortScan"]);
                var serverScan = ConfigurationManager.AppSettings["ServerScan"];
                var clam = new ClamClient(serverScan, portScan);
                var result = await clam.SendAndScanFileAsync(fileBytes);
                return result.Result;
            }
            catch (Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return ClamScanResults.Clean;
            }
        }
    }
}