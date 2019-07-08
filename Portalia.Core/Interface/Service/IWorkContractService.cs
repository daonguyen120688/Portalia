using Portalia.Core.Dtos;
using Portalia.Core.Dtos.Message;
using Portalia.Core.Entity;
using Portalia.Core.Enum;

namespace Portalia.Core.Interface.Service
{
    public interface IWorkContractService
    {
        WorkContractDto Get(int workContractId);
        WorkContractDto GetWCByUser(string userId, WorkContractType type);
        WorkContractDto GetValidatedWorkContract(string emailAddress);
        WorkContract Create(WorkContract workContract);
        bool Delete(int workContractId);
        int OpenNewWorkContract(string loggedUserId, string employeeUserId, WorkContractType type = WorkContractType.Candidate);
        int SetWorkContractStatus(int contractId, WorkContractStatusEnum contractStatus, string loggedUserId);
        WorkContractStatusEnum? GetWorkContractStatus(int workContractId);
        WorkContractStatusEnum? GetWorkContractStatus(string emailAddress);
        void NotifyWorkContractOwner(string candidateEmail, string candidateName, string contractFromUrl);
        MessageDto Update(WorkContractDto workContractDto);
        MessageDto GenerateFieldData(int workContractId);
        bool ToFieldData(WorkContract workContract);
        int CreateSkill(string label, int businessLineId = 2, bool validated = true);
        void SendEmail(object emailObj, string templateName);
        string GetUserEmailByWorkContractId(int workcontractId);
        WorkContractCommentDto GetMostRecentCommentOfWorkContract(int workcontractId);
        MessageDto UploadDocument(string userId, int documentId);
        void SendEmailToNotifyCandidateWorkContractIsReady(string userId, string requestUrl);
        string GetCurrentSkills(int workContractId);
        bool HasContract(string userId, WorkContractType type = WorkContractType.Candidate);
        void SendReminderToCandidate(string requestUrl);
    }
}
