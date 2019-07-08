using Portalia.Core.Interface.Service;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Portalia.Core.Entity;
using Repository.Pattern.Infrastructure;
using Portalia.Core.Dtos;
using Portalia.Core.Enum;
using System;
using Portalia.Core.Extensions;
using Portalia.Repository;
using System.Linq;
using System.Data.Entity;
using Newtonsoft.Json;
using MultiBranding.ApiClient;
using Portalia.Core.Dtos.Message;
using Portalia.Core.Helpers;
using System.Reflection;
using Portalia.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;
using Portalia.Core.StaticContent;
using System.Configuration;

namespace Portalia.Service
{
    public class WorkContractService : IWorkContractService
    {
        private readonly IRepositoryAsync<WorkContract> _workContractRepository;
        private readonly IRepositoryAsync<Skill> _skillRepository;
        private readonly IRepositoryAsync<WorkContractStatus> _workContractStatusRepository;
        private readonly IRepositoryAsync<TrackingChange> _trackingChangeRepository;
        private readonly IRepositoryAsync<DataField> _dataFieldRepository;
        private readonly IRepositoryAsync<AspNetUser> _aspnetUserService;
        private readonly IRepositoryAsync<WorkContractComment> _workcontractCommentRepository;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IUserProfileService _userProfileService;
        private readonly IRepositoryAsync<Document> _documentRepository;
        private readonly IRepositoryAsync<Proposal> _proposalRepository;
        private readonly IRepositoryAsync<Amaris_Smart_City> _cityRepository;

        public WorkContractService(IRepositoryAsync<WorkContract> workContractRepository,
            IUnitOfWorkAsync unitOfWork,
            PortaliaContext portaliaContext,
            IRepositoryAsync<WorkContractStatus> workContractStatusRepository,
            IUserProfileService userProfileService,
            IRepositoryAsync<DataField> dataFieldRepository,
            IRepositoryAsync<Skill> skillRepository,
            IRepositoryAsync<TrackingChange> trackingChangeRepository,
            IRepositoryAsync<AspNetUser> aspnetUserService,
            IRepositoryAsync<WorkContractComment> workcontractCommentRepository,
            IRepositoryAsync<Document> documentRepository,
            IRepositoryAsync<Proposal> proposalRepository,
            IRepositoryAsync<Amaris_Smart_City> cityRepository)
        {
            _workContractRepository = workContractRepository;
            _unitOfWork = unitOfWork;
            _workContractStatusRepository = workContractStatusRepository;
            _userProfileService = userProfileService;
            _dataFieldRepository = dataFieldRepository;
            _skillRepository = skillRepository;
            _trackingChangeRepository = trackingChangeRepository;
            _aspnetUserService = aspnetUserService;
            _workcontractCommentRepository = workcontractCommentRepository;
            _documentRepository = documentRepository;
            _proposalRepository = proposalRepository;
            _cityRepository = cityRepository;
        }

        public string GetCurrentSkills(int workContractId)
        {
            WorkContract workContract = _workContractRepository.Queryable().AsNoTracking()
               .SingleOrDefault(x => x.ContractId == workContractId);

            return workContract?.Skills;
        }

        public bool HasContract(string userId, WorkContractType type = WorkContractType.Candidate)
        {
            var documentContractId = _workContractRepository
                .Queryable()
                .AsNoTracking()
                .FirstOrDefault(x => x.UserId == userId && x.Type == (int)type)?.DocumentId ?? 0;
            var hasContract = documentContractId > 0;
            return hasContract;
        }

        // Send reminder to candidate if they haven't save/submit work contract after 1 day since the last update
        public void SendReminderToCandidate(string requestUrl)
        {
            var pendingOnCandidateContracts = _workContractRepository
                .Queryable()
                .Where(s =>
                    s.WorkContractStatuses.OrderByDescending(x => x.WorkContractStatusId).FirstOrDefault().StatusId ==
                    (int) WorkContractStatusEnum.PendingOnCandidate 
                    && s.UpdatedDate.HasValue
                    && s.WorkContractStatuses.OrderByDescending(x => x.WorkContractStatusId).FirstOrDefault().HasSentReminder == false)
                .ToList();

            foreach (var contract in pendingOnCandidateContracts)
            {
                var hoursDiff = (DateTime.UtcNow - contract.UpdatedDate.Value).TotalHours;
                if (hoursDiff < 24)
                {
                    continue;
                }

                var user = _aspnetUserService.Queryable().FirstOrDefault(s => s.Id == contract.UserId);
                var emailParams = new
                {
                    FirstName = user?.FirstName,
                    WorkContractUrl = GetWorkContractUrl(requestUrl, contract.ContractId),
                    UserPersonalEmail = user?.Email
                };
                SendEmail(emailParams, EmailTemplateUtility.RemindCandidateToSubmitWorkContract);
                var workContractStatus = _workContractStatusRepository.Queryable()
                    .Where(s => s.ContractId == contract.ContractId)
                    .OrderByDescending(s => s.WorkContractStatusId)
                    .FirstOrDefault();

                if (workContractStatus != null &&
                    workContractStatus.StatusId == (int) WorkContractStatusEnum.PendingOnCandidate &&
                    !workContractStatus.HasSentReminder)
                {
                    workContractStatus.HasSentReminder = true;
                    workContractStatus.ObjectState = ObjectState.Modified;
                }
            }

            _unitOfWork.SaveChanges();
        }

        public WorkContractDto Get(int workContractId)
        {
            WorkContract workContract = _workContractRepository.Queryable().AsNoTracking()
                .Include(x => x.WorkContractStatuses)
                .SingleOrDefault(x => x.ContractId == workContractId);
            return workContract?.ToWorkContractDto();
        }

        public WorkContractCommentDto GetMostRecentCommentOfWorkContract(int workcontractId)
        {
            WorkContractComment recentComment = _workcontractCommentRepository.Queryable().AsNoTracking().OrderByDescending(x=>x.WCCommentId).FirstOrDefault(x => x.ContractId == workcontractId);

            if (recentComment == null)
                return null;
            return recentComment.ToWorkContractCommentDto();
        }

        public MessageDto UploadDocument(string userId, int documentId)
        {
            var workContract = _workContractRepository
                .Queryable()
                .FirstOrDefault(s => s.UserId == userId);

            // If work contract not found, return error message
            if (workContract == null)
            {
                return MessageDto.GetErrorMessage("Work contract not found");
            }

            // Find the old document
            var deletedDocument = _documentRepository.Queryable()
                .FirstOrDefault(s => s.DocumentId == workContract.DocumentId);

            // If old document found, deactivate it
            if (deletedDocument != null)
            {
                deletedDocument.IsActive = false;
                deletedDocument.ObjectState = ObjectState.Modified;
            }

            // Set new document to work contract
            workContract.DocumentId = documentId;
            workContract.ObjectState = ObjectState.Modified;
            // Save changes
            _unitOfWork.SaveChanges();

            // Set status of work contract to "Uploaded"
            SetWorkContractStatus(workContract.ContractId, WorkContractStatusEnum.Uploaded, userId);

            return MessageDto.GetSuccessMessage("Upload work contract successfully");
        }

        public void SendEmailToNotifyCandidateWorkContractIsReady(string userId, string requestUrl)
        {
            var user = _aspnetUserService
                .Queryable()
                .FirstOrDefault(s => s.Id == userId);

            if (user == null)
            {
                LoggerHelpers.Error("Count not send email to notify candidate that the work contract is ready. User not found");
                return;
            }
            
            var emailParameters = new
            {
                UserFirstName = user.FirstName,
                UserPersonalEmail = user.Email,
                MyPersonalSpaceUrl = requestUrl
            };

            SendEmail(emailParameters, EmailTemplateUtility.NotifyCandidateWorkContractIsReady);
        }

        public int CreateWCComment(WorkContractComment comment)
        {
            if (comment == null)
                return 0;

            _workcontractCommentRepository.Insert(comment);
            _unitOfWork.SaveChanges();

            return comment.WCCommentId;
        }

        public WorkContractDto GetWCByUser(string userId, WorkContractType type)
        {
            WorkContract workContract = _workContractRepository.Queryable().AsNoTracking()
                .Include(x => x.WorkContractStatuses)
                .FirstOrDefault(x => x.UserId == userId && x.Type == (int)type);
            var proposalId = _proposalRepository.Queryable().AsNoTracking()
                .FirstOrDefault(s => s.UserId == userId)?.ProposalId ?? 0;
            var workContractDto = workContract?.ToWorkContractDto();

            if (workContractDto != null)
            {
                workContractDto.ProposalId = proposalId;
            }

            return workContract?.ToWorkContractDto();
        }
        
        public WorkContractDto GetValidatedWorkContract(string emailAddress)
        {
            List<int> validatedStatuses = EnumExtensions.GetWorkContractValidatedStatuses();

            // Get validated work contract based on personal email address
            WorkContract validatedContract =
                (from wc in _workContractRepository.Queryable()
                 join u in _aspnetUserService.Queryable() on wc.UserId equals u.Id
                 join wcs in _workContractStatusRepository.Queryable() on wc.ContractId equals wcs.ContractId
                 where u.Email == emailAddress && validatedStatuses.Contains(wcs.StatusId)
                 select wc)
                .Include(x => x.DataFields)
                .FirstOrDefault();

            if (validatedContract != null)
            {
                WorkContractDto dto = validatedContract.ToWorkContractDto();

                // Get data fields
                if (validatedContract.DataFields != null && validatedContract.DataFields.Any())
                {
                    dto.DataFields = validatedContract.DataFields.Select(d => new DataFieldDto
                    {
                        FieldId = d.FieldId,
                        Value = d.Value
                    });
                }

                return dto;
            }

            return null;
        }

        public WorkContract Create(WorkContract workContract)
        {
            var workContractOwner = _userProfileService.GetUsersIdentityById(workContract.UserId);
            if (workContractOwner != null)
            {
                workContract.FirstName = workContractOwner.FirstName;
                workContract.LastName = workContractOwner.LastName;
            }
            workContract.ObjectState = ObjectState.Added;
            _workContractRepository.Insert(workContract);
            _unitOfWork.SaveChanges();

            return workContract;
        }

        public int CreateSkill(string label, int businessLineId = 2, bool validated = true)
        {
            Skill skill = new Skill()
            {
                Label = label,
                BusinessLineId = businessLineId,
                Validated = validated,
                Created_Date = DateTime.Now
            };

            _skillRepository.Insert(skill);
            _unitOfWork.SaveChanges();
            return skill.SkillId;
        }

        public bool Delete(int workContractId)
        {
            WorkContract workContract = _workContractRepository.Find(workContractId);
            if (workContract == null)
            {
                return false;
            }
            workContract.ObjectState = ObjectState.Deleted;
            _workContractRepository.Delete(workContract);
            _unitOfWork.SaveChanges();
            return true;
        }

        /// <summary>
        /// success -> return work contract id
        /// failed -> return 0
        /// </summary>
        public int OpenNewWorkContract(string loggedUserId, string employeeUserId, WorkContractType type)
        {
            try
            {
                WorkContract userWorkContract = _workContractRepository.Queryable().FirstOrDefault(wc => wc.UserId == employeeUserId);
                if (userWorkContract == null || userWorkContract == default(WorkContract))
                {
                    WorkContract newWorkContract = new WorkContract
                    {
                        UserId = employeeUserId,
                        Type = (int)type,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = employeeUserId,
                        UpdatedDate = DateTime.UtcNow
                    };
                    userWorkContract = Create(newWorkContract);
                }

                SetWorkContractStatus(userWorkContract.ContractId, WorkContractStatusEnum.PendingOnCandidate, loggedUserId);
                return userWorkContract.ContractId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// success -> return work contract status id
        /// failed -> return 0
        /// </summary>
        public int SetWorkContractStatus(int contractId, WorkContractStatusEnum contractStatus, string loggedUserId)
        {
            try
            {
                // Check if work contract exist
                var isWorkContractExist = _workContractRepository.Queryable().Any(wc => wc.ContractId == contractId);

                // If work contract not found, return 0
                if (!isWorkContractExist)
                {
                    return 0;
                }

                // Initialize work contract status object with new status
                WorkContractStatus workContractStatus = new WorkContractStatus()
                {
                    ContractId = contractId,
                    StatusId = (int)contractStatus,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = loggedUserId
                };

                // Add work contract status to database
                workContractStatus.ObjectState = ObjectState.Added;
                _workContractStatusRepository.Insert(workContractStatus);
                // Save changes
                _unitOfWork.SaveChanges();

                // Return work contract status
                return workContractStatus.StatusId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        // Get the latest status of work contract
        public WorkContractStatusEnum? GetWorkContractStatus(int workContractId)
        {
            WorkContractStatusEnum? workContractStatus = (WorkContractStatusEnum)_workContractStatusRepository.Queryable().Where(wcs => wcs.ContractId == workContractId).OrderByDescending(wcs => wcs.CreatedDate).FirstOrDefault()?.StatusId;
            return workContractStatus.HasValue ? workContractStatus : null;
        }
        
        public WorkContractStatusEnum? GetWorkContractStatus(string emailAddress)
        {
            // Get current work contract status by personal email address
            int? statusId =
                (from wc in _workContractRepository.Queryable()
                 join u in _aspnetUserService.Queryable() on wc.UserId equals u.Id
                 join wcs in _workContractStatusRepository.Queryable() on wc.ContractId equals wcs.ContractId
                 where u.Email == emailAddress
                 select wcs)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.StatusId)
                .FirstOrDefault();

            if (statusId > 0)
            {
                return (WorkContractStatusEnum)statusId;
            }
            else
            {
                return null;
            }
        }

        public void NotifyWorkContractOwner(string candidateEmail, string candidateName, string contractFromUrl)
        {
            // If user found Initialize email params
            var emailParams = new
            {
                UserPersonalEmail = candidateEmail,
                FirstName = candidateName,
                WorkContractUrl = contractFromUrl
            };

            // Call MultiBranding API to send email to notify employee about the contract
            SendEmail(emailParams, EmailTemplateUtility.OpenWorkContractNotification);
        }

        public void SendEmail(object emailObj, string templateName)
        {
            // If user found Initialize email params
            string emailParams = null;

            if (emailObj != null)
            {
                emailParams = JsonConvert.SerializeObject(emailObj);
            }

            MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];

            // Call MultiBranding API to send email
            var resultSendMail = MultiBrandingHelper.SendEmail(new SendEmailParameter()
            {
                ApplicationName = "Portalia",
                Holding = "Portalia",
                TemplateName = templateName,
                Variable = emailParams
            });

            // Log the result of calling API
            if (resultSendMail == null)
            {
                LoggerHelpers.Error($"Cannot call API to send {templateName}");
            }
            else
            {
                LoggerHelpers.Info(
                    $"send {templateName} email:{JsonConvert.SerializeObject(resultSendMail)}");
            }
        }

        public MessageDto Update(WorkContractDto workContractDto)
        {
            string hightlightFields = string.Empty;
            // Find work contract by work contract ID
            var workContract = _workContractRepository.Queryable().FirstOrDefault(s => s.ContractId == workContractDto.ContractId);

            // If work contract not found, return error message
            if (workContract == null)
            {
                return MessageDto.GetErrorMessage("Work contract not found");
            }

            //Tracking data changes
            //Only perform tracking data when current status >= PendingOnManager
            if ((int)workContractDto.WorkContractStatus.StatusId > (int)WorkContractStatusEnum.PendingOnCandidate)
            {
                PerformTrackingWCChanges(workContract.ToWorkContractDto(),workContractDto,ref hightlightFields);
            }

            // Map fields from DTO object to DOMAIN object
            workContract.ToUpdateWorkContract(workContractDto);
            workContract.ObjectState = ObjectState.Modified;
            // Save changes to database
            _unitOfWork.SaveChanges();

            // Each time we update Work Contract, we have to add a record to Work Contract Status table to keep track with the status history
            // Only track Status != PendingOnCandidate
            if (workContractDto.WorkContractStatus.StatusId != WorkContractStatusEnum.PendingOnCandidate)
            {
                SetWorkContractStatus(workContract.ContractId, workContractDto.WorkContractStatus.StatusId, workContractDto.UpdatedBy);
            }

            // Return success message
            return MessageDto.GetSuccessMessage("Update work contract successfully");
        }

        public MessageDto GenerateFieldData(int workContractId)
        {
            var workContract = _workContractRepository.Queryable().FirstOrDefault(s => s.ContractId == workContractId);

            // If work contract not found, return error message
            if (workContract == null)
            {
                return MessageDto.GetErrorMessage("Work contract not found");
            }

            // Convert work contract obj to list of data fields for Work Contract app to get data to adapt their needs
            var saveWorkContractDataFields = ToFieldData(workContract);

            if (!saveWorkContractDataFields)
            {
                LoggerHelpers.Error("Cannot convert work contract obj to list of data fields");
            }

            return MessageDto.GetSuccessMessage("Work contract validated");
        }

        public string GetUserEmailByWorkContractId(int workcontractId)
        {
            AspNetUser user = _workContractRepository.Queryable().AsNoTracking().Where(x => x.ContractId == workcontractId).Select(x => x.AspNetUser).FirstOrDefault();
            if (user == null)
                return null;
            return user.Email;
        }

        /// <summary>
        /// transfer work contract obj to list of data fields and saved to db
        /// return true if saved successfully
        /// </summary>
        public bool ToFieldData(WorkContract workContract)
        {
            try
            {
                // If work contract == null, return false
                if (workContract == null || workContract == default(WorkContract))
                {
                    return false;
                }

                // Get latest work contract status
                var wcStatus = GetWorkContractStatus(workContract.ContractId);

                // If status not found or status != Validated, return false
                if (!wcStatus.HasValue || wcStatus.Value != WorkContractStatusEnum.Validated)
                {
                    return false;
                }

                // START: use reflection to get Type of WorkContract object
                var type = workContract.GetType();
                // Only get properties with custom attribute 'WorkContractMappingDataFieldAttribute'
                var propertiesWithCustomAttribute = type.GetProperties().Where(s =>
                    Attribute.IsDefined(s, typeof(WorkContractMappingDataFieldAttribute)));

                // Loop through each property of WorkContract object
                foreach (PropertyInfo propertyInfo in propertiesWithCustomAttribute)
                {
                    // If property cannot be read, skip this
                    if (!propertyInfo.CanRead)
                    {
                        continue;
                    }

                    // Get specific "WorkContractMappingDataFieldAttribute" of this property
                    // This attribute is used to map the Name and the DataField
                    var customPropertyNameAttribute =
                        (WorkContractMappingDataFieldAttribute)propertyInfo.GetCustomAttribute(typeof(WorkContractMappingDataFieldAttribute));

                    // If the attribute not found, skip this property because we don't need it
                    if (customPropertyNameAttribute == null)
                    {
                        continue;
                    }

                    // Initialize DataField enum
                    DataFieldEnum dataField;
                    // Parse attribute value to DataField enum
                    Enum.TryParse(customPropertyNameAttribute.Name, out dataField);

                    // If cannot be converted, skip this property
                    if (dataField == default(DataFieldEnum))
                    {
                        continue;
                    }

                    string value=string.Empty;

                    switch(dataField)
                    {
                        case DataFieldEnum.Address:
                            // Address field - serialize as JSON
                            int cityId = Convert.ToInt32(workContract.City);

                            var address = new
                            {
                                workContract.Address,
                                ZipCode = workContract.PostCode,
                                CityID = _cityRepository.Queryable().Where(x => x.ID == cityId).FirstOrDefault()?.Name,
                                CountryID = workContract.Country
                            };
                            value = JsonConvert.SerializeObject(address);
                            break;
                        case DataFieldEnum.Skills:
                            //Split and convert skills to int array
                            int[] skills = Array.ConvertAll(workContract.Skills.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                            value = string.Join(",", _skillRepository.Queryable().Where(x => skills.Contains(x.SkillId)).Select(x=>x.Label).ToList());
                            break;
                        default:
                            // Get value of the property of WorkContract object
                            value = propertyInfo.GetValue(workContract, null) + string.Empty;
                            break;
                    }

                    // If value is null or empty, skip this property
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    // Insert DataField to database
                    _dataFieldRepository.Insert(new DataField
                    {
                        ContractId = workContract.ContractId,
                        CreatedDate = DateTime.UtcNow,
                        FieldId = (int)dataField,
                        ObjectState = ObjectState.Added,
                        Value = value
                    });

                }

                // Save all changes
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Catch error and log
                LoggerHelpers.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// A method that help to tracking entity change
        /// </summary>
        /// <param name="oldWC">Current work contract</param>
        /// <param name="newWC">New work contract</param>
        private void PerformTrackingWCChanges(WorkContractDto oldWC, WorkContractDto newWC,ref string highlightFields)
        {
            if (oldWC == null || newWC == null)
                return;

            foreach (PropertyInfo property in newWC.GetType().GetProperties())
            {
                bool isChange = false;
                //Ignore field that has JsonIgnoreAttribute attribute
                if (property.GetCustomAttribute<IgnoreTrackingChange>() != null)
                {
                    continue;
                }

                DataTypeInfor dataTypeInfor = property.GetCustomAttribute<DataTypeInfor>();

                if (dataTypeInfor == null)
                    dataTypeInfor = new DataTypeInfor() { FieldType = FieldType.Text };

                DisplayAttribute display = property.GetCustomAttribute<DisplayAttribute>();
                string displayName = display != null ? display.Name : property.Name;
                object oldRawValue = oldWC.GetType().GetProperty(property.Name).GetValue(oldWC);
                object newRawValue = property.GetValue(newWC);
                string oldValue = null, newValue = null;

                switch (dataTypeInfor.FieldType)
                {
                    case FieldType.MultiSelect:
                        object objDatasourceForMulti = newWC.GetType().GetProperty(dataTypeInfor.DatasourceProperty).GetValue(newWC);
                        List<SelectListItem> datasourceForMulti = objDatasourceForMulti != null ? (List<SelectListItem>)objDatasourceForMulti : null;
                        oldValue = oldRawValue == null ? string.Empty : oldRawValue.ToString();
                        newValue = newRawValue == null ? string.Empty : newRawValue.ToString();
                        CompareAndTrackMultiDrowDownFields(displayName, datasourceForMulti, oldValue, newValue, newWC.ContractId, newWC.UpdatedBy, ref isChange);
                        break;
                    case FieldType.DateTime:
                        CompareAndTrackDateTimeFields(displayName, dataTypeInfor.DateTimeFormat, oldRawValue, newRawValue, newWC.ContractId, newWC.UpdatedBy,ref isChange);
                        break;
                    case FieldType.DropDown:

                        object objDatasource = newWC.GetType().GetProperty(dataTypeInfor.DatasourceProperty).GetValue(newWC);
                        List<SelectListItem> datasource = objDatasource!=null?(List<SelectListItem>)objDatasource:null;
                        oldValue = oldRawValue == null ? string.Empty : oldRawValue.ToString();
                        newValue = newRawValue == null ? string.Empty : newRawValue.ToString();
                        CompareAndTrackDrowDownFields(displayName, datasource, oldValue, newValue, newWC.ContractId, newWC.UpdatedBy,ref isChange);
                        break;
                    case FieldType.Text:
                    default:

                        oldValue = oldRawValue == null ? string.Empty : oldRawValue.ToString();
                        newValue = newRawValue == null ? string.Empty : newRawValue.ToString();
                        CompareAndTrackTextFields(displayName, oldValue, newValue, newWC.ContractId, newWC.UpdatedBy,ref isChange);
                        break;
                }

                if (isChange)
                    highlightFields += $",#{property.Name}";
            }

            highlightFields = highlightFields.Replace("#Allowances", "#AllowancesDisplay");
        }

        /// <summary>
        /// Compare and track data change between 2 text fields
        /// </summary>
        /// <param name="fieldName">Display name of field</param>
        /// <param name="oldValue">Current value</param>
        /// <param name="newValue">New value</param>
        /// <param name="contractId">Work contract id</param>
        /// <param name="userId">User Id</param>
        private void CompareAndTrackTextFields(string fieldName, string oldValue, string newValue, int contractId, string userId, ref bool isChange)
        {
            if (oldValue != newValue)
            {
                isChange = true;
                TrackingChange entity = new TrackingChange()
                {
                    ContractId = contractId,
                    FieldName = fieldName,
                    OldValue = oldValue,
                    NewValue = newValue,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _trackingChangeRepository.Insert(entity);
            }
        }

        /// <summary>
        /// Compare and track data change between 2 datetime fields
        /// </summary>
        /// <param name="fieldName">Display name of field</param>
        /// <param name="format">Datetime format</param>
        /// <param name="oldRawValue">Current value</param>
        /// <param name="newRawValue">New value</param>
        /// <param name="contractId">Work contract id</param>
        /// <param name="userId">User Id</param>
        private void CompareAndTrackDateTimeFields(string fieldName, string format, object oldRawValue, object newRawValue, int contractId, string userId,ref bool isChange)
        {
            DateTime? oldDateTimeValue = (DateTime?)oldRawValue;
            DateTime? newDateTimeValue = (DateTime?)newRawValue;

            string oldValue = oldDateTimeValue.HasValue ? oldDateTimeValue.Value.ToString(format) : string.Empty;
            string newValue = newDateTimeValue.HasValue ? newDateTimeValue.Value.ToString(format) : string.Empty;

            CompareAndTrackTextFields(fieldName, oldValue, newValue, contractId, userId,ref isChange);
        }

        /// <summary>
        /// Compare and track data change between 2 dropdown fields
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="datasource"></param>
        /// <param name="oldRawValue"></param>
        /// <param name="newRawValue"></param>
        /// <param name="contractId"></param>
        /// <param name="userId"></param>
        private void CompareAndTrackDrowDownFields(string fieldName, List<SelectListItem> datasource, string oldRawValue, string newRawValue, int contractId, string userId, ref bool isChange)
        {
            if(datasource==null)
            {
                CompareAndTrackTextFields(fieldName, oldRawValue, newRawValue, contractId, userId,ref isChange);
                return;
            }
            SelectListItem refDataDto = datasource.FirstOrDefault(x => x.Value == oldRawValue);
            string oldValue = refDataDto == null ? oldRawValue : refDataDto.Text;

            refDataDto = datasource.FirstOrDefault(x => x.Value == newRawValue);
            string newValue= refDataDto == null ? newRawValue : refDataDto.Text;

            CompareAndTrackTextFields(fieldName, oldValue, newValue, contractId, userId, ref isChange);
        }

        /// <summary>
        /// Compare and track data change between 2 multi-select fields
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="datasource"></param>
        /// <param name="oldRawValue"></param>
        /// <param name="newRawValue"></param>
        /// <param name="contractId"></param>
        /// <param name="userId"></param>
        private void CompareAndTrackMultiDrowDownFields(string fieldName, List<SelectListItem> datasource, string oldRawValue, string newRawValue, int contractId, string userId, ref bool isChange)
        {
            if (datasource == null)
            {
                CompareAndTrackTextFields(fieldName, oldRawValue, newRawValue, contractId, userId, ref isChange);
                return;
            }

            string oldValue=string.Empty, newValue=string.Empty;
            SelectListItem refDataDto = null;

            if (string.IsNullOrEmpty(oldRawValue))
                oldValue = string.Empty;
            else//Split values by comma(,) character
            {
                string[] arrOldValue = oldRawValue.Split(new char[] { ',' });
                foreach(string val in arrOldValue)
                {
                    refDataDto = datasource.FirstOrDefault(x => x.Value == val);
                    oldValue +=","+(refDataDto == null ? val : refDataDto.Text);
                }

                if (oldValue.Length > 1)
                    oldValue = oldValue.Remove(0, 1);
            }

            if (string.IsNullOrEmpty(newRawValue))
                newValue = string.Empty;
            else//Split values by comma(,) character
            {
                string[] arrNewValue = newRawValue.Split(new char[] { ',' });
                foreach (string val in arrNewValue)
                {
                    refDataDto = datasource.FirstOrDefault(x => x.Value == val);
                    newValue += "," + (refDataDto == null ? val : refDataDto.Text);
                }

                if (newValue.Length > 1)
                    newValue = newValue.Remove(0, 1);
            }

            CompareAndTrackTextFields(fieldName, oldValue, newValue, contractId, userId, ref isChange);
        }

        private string GetMySpaceUrl(string requestUrl)
        {
            return StringHelpers.GetEnvironmentUrl(requestUrl) + "/Portalia/Proposal/MySpace";
        }

        private string GetWorkContractUrl(string requestUrl, int workContractId)
        {
            return StringHelpers.GetEnvironmentUrl(requestUrl) + "/Portalia/WorkContract/Index?workContractId=" +
                   workContractId;
        }
    }
}
