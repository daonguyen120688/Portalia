using Portalia.Core.Dtos;
using Portalia.Core.Enum;
using System;
using System.Web;
using Portalia.Core.Dtos.Message;

namespace Portalia.Core.Interface.Service
{
    public interface IApplicationForm
    {
        int AddApplyForm(ApplicationFormDto model, Exception error);
        AmarisEnum.SourcingStatus AddApplyDocument(int applyFormRequestId, ApplicationDocumentDto document, Exception error);
        int UpdateApplicationFormStatus(int applyFormRequestId, int applyFormStatus, Exception error);
        ApplicationDocumentDto CreateApplicationDocument(HttpPostedFileBase uploadedCv, byte[] data, Exception error);
        MessageDto UpdateApplicationFormFromWorkContract(ApplicationFormDto applicationFormDto);
    }
}
