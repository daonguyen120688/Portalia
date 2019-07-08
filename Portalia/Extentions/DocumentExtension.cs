using Portalia.Core.Dtos.Document;
using Portalia.Core.Enum;
using Portalia.ViewModels.WorkContracts;

namespace Portalia.Extentions
{
    public static class DocumentExtension
    {
        public static UploadDocumentDto ToUploadDocumentDto(
            this UploadWorkContractViewModel uploadWorkContractViewModel, int proposalId)
        {
            if (uploadWorkContractViewModel == null)
            {
                return null;
            }

            return new UploadDocumentDto
            {
                UserId = uploadWorkContractViewModel.UserId,
                FolderType = (FolderType)uploadWorkContractViewModel.FolderType,
                FileName = uploadWorkContractViewModel.Contract.GetFileNameWithoutSpaceBetweenWords(),
                FileBinary = uploadWorkContractViewModel.Contract.GetFileBinary(),
                ProposalId = proposalId
            };
        }
    }
}