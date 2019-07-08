using System.Collections.Generic;
using Portalia.Core.Dtos.Document;
using Portalia.Core.Dtos.Message;
using Portalia.Core.Entity;
using FolderType = Portalia.Core.Enum.FolderType;

namespace Portalia.Core.Interface.Service
{
    public interface IDocumentService
    {
        Document Create(Document document);
        Document Delete(int documentId, string userId);
        List<Document> GetByProposalId(int proposalId,string userId);
        List<Document> GetAll(string userId, int proposalId, FolderType folderType);
        bool HaveDocument(string getUserId, int documentId);
        Document GetById(int documentId);
        List<Document> GetAll(string userId, FolderType folderType);
        List<Document> GetAllByUserId(string userId);
        Document GetByUserId(string userid, int documentId);
        void CreateDefault(Document document);
        List<Document> GetDefaultDocuments(FolderType? folderType);
        void DeleteDefaultDocument(int documentId);
        MessageDto UploadFile(UploadDocumentDto uploadDocumentDto);
    }
}
