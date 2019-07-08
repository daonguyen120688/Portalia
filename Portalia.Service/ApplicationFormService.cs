using Portalia.Repository;
using System;
using System.Linq;
using Portalia.Core.Enum;
using System.IO;
using System.Web;
using Portalia.Core.Dtos;
using Portalia.Core.Dtos.Message;
using Portalia.Core.Helpers;
using Portalia.Core.Interface.Service;

namespace Portalia.Service
{
    public class ApplicationFormService : IApplicationForm
    {
        private readonly WebsiteApplicationContext _dbContext;

        public ApplicationFormService(WebsiteApplicationContext websiteApplicationContext)
        {
            _dbContext = websiteApplicationContext;
        }

        public int AddApplyForm(ApplicationFormDto model, Exception error)
        {
            int applicationFormId;
            try
            {
                LoggerHelpers.Info("Start Apply Form Proc");
                var appformId = _dbContext.CreateApplicationForm(
                    email: model.Email,
                    applicationSourceId: (int)AmarisEnum.ApplicationSource.PortaliaWebsite,
                    jobOfferId: model.JobOfferId == 0 ? default(int?) : model.JobOfferId,
                    firstName: model.FirstName,
                    lastname: model.LastName,
                    titleId: null,
                    genderId: model.Gender,
                    birthDate: model.BirthDate,
                    preferredLanguageId: model.PreferredLanguage,
                    phoneNumber: model.PhoneNumber,
                    residenceCountryId: null,
                    residenceCityId: null,
                    residenceZipCode: null,
                    residenceAddress: model.Address,
                    citizenshipCountryId: null,
                    utmSource: null,
                    utmCampaign: null,
                    utmMedium: null
                    );

                applicationFormId = (appformId.FirstOrDefault() ?? 0);
                error = null;
                LoggerHelpers.Info($"DONE: Apply Form Proc. ApplicationFormId: {applicationFormId}");
            }
            catch (Exception e)
            {
                error = e;
                applicationFormId = 0;
                LoggerHelpers.Error(e);
            }
            return applicationFormId;
        }

        public AmarisEnum.SourcingStatus AddApplyDocument(int applyFormRequestId, ApplicationDocumentDto document, Exception error)
        {
            var finalStatus = AmarisEnum.SourcingStatus.Pending;
            error = null;
            try
            {
                LoggerHelpers.Info($"Start AppApplyDocument");
                int result = _dbContext.AddApplicationFormDocument(applyFormRequestId, document.Filename ?? "", document.Extension ?? "",
                    document.Binary, document.TypeId);
                LoggerHelpers.Info($"DONE AppApplyDocument. Result: {result}");
            }
            catch (Exception exception)
            {
                finalStatus = AmarisEnum.SourcingStatus.WebsiteError;
                //Log 
                error = exception;
                LoggerHelpers.Error(exception);
            }

            return finalStatus;
        }

        public int UpdateApplicationFormStatus(int applyFormRequestId, int applyFormStatus, Exception error)
        {
            error = null;
            //Log 
            int formId;
            try
            {
                LoggerHelpers.Info($"Start UpdateApplicationFormStatus: applyFormRequestId - {applyFormRequestId}, applyFormStatus - {applyFormStatus}");
                formId = _dbContext.UpdateApplicationFormStatus(applyFormRequestId, applyFormStatus);
                LoggerHelpers.Info($"DONE UpdateApplicationFormStatus: FormId - {formId}");
            }
            catch (Exception exception)
            {
                error = exception;
                formId = 0;
                LoggerHelpers.Error(exception);
            }

            return formId;
        }

        // Upload file & save temporary to TempData
        public ApplicationDocumentDto CreateApplicationDocument(HttpPostedFileBase uploadedCv, byte[] data, Exception error)
        {
            try
            {
                //default error is null
                error = null;
                
                var extension = "";
                if (!string.IsNullOrWhiteSpace(uploadedCv.FileName))
                {
                    try
                    {
                        extension = Path.GetExtension(uploadedCv.FileName);
                    }
                    catch (Exception exception)
                    {
                        //LOG
                        error = exception;
                        LoggerHelpers.Info($"Method CreateApplicationDocument: {exception}");
                        if (uploadedCv.FileName.Contains("."))
                        {
                            extension = uploadedCv.FileName.Substring(uploadedCv.FileName.LastIndexOf('.') + 1);
                        }
                    }
                }
                var websiteAppDocumentModel = new ApplicationDocumentDto
                {
                    Filename = uploadedCv.FileName,
                    Extension = extension,
                    Binary = data,
                    CreatedDate = DateTime.Today,
                    TypeId = (byte)AmarisEnum.DocumentType.Resume
                };

                return websiteAppDocumentModel;
            }
            catch (Exception exception)
            {
                error = exception;
                LoggerHelpers.Error(exception);

                return null;
            }
        }

        // Update data for application form after admin validates work contract in Portalia
        public MessageDto UpdateApplicationFormFromWorkContract(ApplicationFormDto applicationFormDto)
        {
            // If data is null, return error message
            if (applicationFormDto == null)
            {
                return MessageDto.GetErrorMessage();
            }

            // Find latest application form with email, status = pending, no job offer, and from PortaliaWebsite
            var latestApplicationForm = _dbContext.ApplicationForms.AsQueryable().Where(s =>
                    s.Email == applicationFormDto.Email &&
                    (s.StatusId == (int) AmarisEnum.SourcingStatus.Created &&
                     s.StatusId == (int) AmarisEnum.SourcingStatus.Pending) &&
                    s.JobOfferId == null && s.ApplicationSourceId == (int) AmarisEnum.ApplicationSource.PortaliaWebsite)
                .OrderByDescending(s => s.ApplicationFormId)
                .FirstOrDefault()?.ApplicationFormId;

            // If application not found, return error message
            if (!latestApplicationForm.HasValue)
            {
                var latestApplicationFormWhichIsInProgress = _dbContext.ApplicationForms
                    .AsQueryable()
                    .Where(s =>
                        s.Email == applicationFormDto.Email &&
                        s.StatusId != (int)AmarisEnum.SourcingStatus.Pending &&
                        s.JobOfferId == null &&
                        s.ApplicationSourceId == (int) AmarisEnum.ApplicationSource.PortaliaWebsite)
                    .OrderByDescending(s => s.ApplicationFormId)
                    .FirstOrDefault();

                if (latestApplicationFormWhichIsInProgress == null)
                {
                    return MessageDto.GetErrorMessage("Application form not found");
                }

                var latestApplicationFormDetail = _dbContext.ApplicationFormDetails
                    .AsQueryable()
                    .FirstOrDefault(s =>
                        s.ApplicationFormDetailId == latestApplicationFormWhichIsInProgress.ApplicationFormId);

                if (latestApplicationFormDetail == null)
                {
                    return MessageDto.GetErrorMessage("Application form not found");
                }

                AddApplyForm(new ApplicationFormDto
                {
                    Email = latestApplicationFormWhichIsInProgress.Email,
                    FirstName = applicationFormDto.FirstName,
                    LastName = applicationFormDto.LastName,
                    Gender = latestApplicationFormDetail.GenderId,
                    BirthDate = applicationFormDto.BirthDate,
                    PreferredLanguage = latestApplicationFormDetail.PreferredLanguageId,
                    PhoneNumber = latestApplicationFormDetail.PhoneNumber,
                    Address = latestApplicationFormDetail.ResidenceAddress
                }, new Exception());

                return MessageDto.GetSuccessMessage("Application form updated");
            }

            // Find application form detail
            var applicationFormDetail = _dbContext.ApplicationFormDetails.AsQueryable()
                .Where(s => s.ApplicationFormDetailId == latestApplicationForm.Value)
                .OrderByDescending(s => s.ApplicationFormDetailId)
                .FirstOrDefault(s => s.ApplicationFormDetailId == latestApplicationForm.Value);

            // If application form detail not found, return error message
            if (applicationFormDetail == null)
            {
                return MessageDto.GetErrorMessage("Application form not found");
            }

            // Update fields
            applicationFormDetail.Firstname = applicationFormDto.FirstName;
            applicationFormDetail.Lastname = applicationFormDto.LastName;
            applicationFormDetail.BirthDate = applicationFormDto.BirthDate;

            // Save changes
            _dbContext.SaveChanges();

            // Return success message
            return MessageDto.GetSuccessMessage("Application form updated");
        }
    }
}
