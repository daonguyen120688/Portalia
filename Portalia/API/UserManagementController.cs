using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using nClam;
using Portalia.API.Attributes;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using Portalia.Extentions;
using Portalia.ViewModels;

namespace Portalia.API
{
    [AllowAnonymous]
    [AuthorizeApi]
    public class UserManagementController : ApiController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IProposalService _proposalService;
        private readonly IDocumentService _documentService;
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserManagementController(IUserProfileService userProfileService, IProposalService proposalService, IDocumentService documentService)
        {
            _userProfileService = userProfileService;
            _proposalService = proposalService;
            _documentService = documentService;
        }

        [HttpGet]
        public IHttpActionResult GetPortaliaUserPaging(int page = 1 , int size = 15,bool? isEmployee = null)
        {
            try
            {
                var users = _userProfileService.GetUsersIdentity(page, size, isEmployee);
                var usersPaging = new AspNetUserExtraInfoPaging()
                {
                    Paging = new PagingHeader(users.TotalItemCount, users.PageNumber, users.PageSize, users.PageCount),
                    AspNetUserExtraInfos = users.ToList()
                };
                return Json(usersPaging);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetPortaliaUser()
        {
            try
            {
                var users = _userProfileService.GetUsersIdentity();
                return Json(users);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetPortaliaUserById(string userId)
        {
            try
            {
                var user = _userProfileService.GetUsersIdentityById(userId);

                if (user == null)
                {
                    return Json(new AspNetUserExtraInfo());
                }

                var aspNetUserExtraInfo = new AspNetUserExtraInfo
                {
                    Id = userId,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    IsEmployee = user.IsEmployee
                };

                return Json(aspNetUserExtraInfo);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetPortaliaUserByUserName(string userName)
        {
            try
            {
                var user = _userProfileService.GetUsersIdentityByUserName(userName);
                return Json(user);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetPortaliaUserByEmail(string email)
        {
            try
            {
                var user = _userProfileService.GetUsersIdentityByEmail(email);
                return Json(user);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetFolderTypes()
        {
            try
            {
                var folderTypes = _proposalService.GetFolderTypes().ToList();
                return Json(folderTypes);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadDocument(string userId, FolderType folderType)
        {
            var proposal = _proposalService.GetById(userId);

            var isSavedSuccessfully = true;
            var fName = "";
            var document = new Document();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string fileName in httpRequest.Files)
                {
                    var file = httpRequest.Files[fileName];
                    if (file != null)
                    {
                        fName = file.FileName;
                        if (file.ContentLength > 0)
                        {
                            var target = new MemoryStream();
                            file.InputStream.CopyTo(target);
                            var data = target.ToArray();
                            document.FileBinary = data;
                            document.ProposalId = proposal.ProposalId;
                            document.Name = file.FileName.Replace(" ", string.Empty);
                            document.FolderType = folderType;
                            _documentService.Create(document);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                logger.Error(ex);
            }

            return
                Json(isSavedSuccessfully
                    ? new { Message = fName, File = document }
                    : new { Message = "Error in saving file", File = document });
        }

        [HttpPost]
        public IHttpActionResult UploadDefaultDocument(FolderType folderType)
        {
            var isSavedSuccessfully = true;
            var fName = "";
            var document = new Document();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string fileName in httpRequest.Files)
                {
                    var file = httpRequest.Files[fileName];
                    if (file != null)
                    {
                        fName = file.FileName;
                        if (file.ContentLength > 0)
                        {
                            var target = new MemoryStream();
                            file.InputStream.CopyTo(target);
                            var data = target.ToArray();
                            document.FileBinary = data;
                            document.Name = file.FileName.Replace(" ", string.Empty);
                            document.FolderType = folderType;
                            _documentService.CreateDefault(document);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                logger.Error(ex);
            }

            return
                Json(isSavedSuccessfully
                    ? new { Message = fName, File = document }
                    : new { Message = "Error in saving file", File = document });
        }

        [HttpGet]
        public IHttpActionResult GetDefaultDocuments(FolderType? folderType)
        {
            try
            {
                var defaultDocuments = _documentService.GetDefaultDocuments(folderType);
                return Json(defaultDocuments);
            }
            catch (Exception ex) 
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteDefaultDocument(int documentId)
        {
            try
            {
                _documentService.DeleteDefaultDocument(documentId);
                return Json(Ok());
            }
            catch (Exception ex) 
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetUserDocumentList(string userId)
        {
            try
            {
                var document = _documentService.GetAllByUserId(userId);
                return Json(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult DownloadDocument(string userid, int documentId)
        {
            try
            {
                var document = _documentService.GetByUserId(userid, documentId);
                return Json(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult DownloadDefaultDocument(int documentId)
        {
            try
            {
                var document = _documentService.GetById(documentId);
                return Json(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteDocument(string userId, int documentId)
        {
            try
            {
                _documentService.Delete(documentId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult Test()
        {
            return Ok("OK");
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
            catch (Exception ex)
            {
                logger.Error(ex);
                return ClamScanResults.Clean;
            }
        }
    }
}
