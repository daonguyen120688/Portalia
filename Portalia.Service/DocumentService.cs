using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Portalia.Core.Dtos.Document;
using Portalia.Core.Dtos.Message;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Extensions;
using Portalia.Core.Interface.Service;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace Portalia.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IRepositoryAsync<Document> _documentRepository;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public DocumentService(IRepositoryAsync<Document> documentRepository, IUnitOfWorkAsync unitOfWork)
        {
            _documentRepository = documentRepository;
            _unitOfWork = unitOfWork;
        }

        public Document Create(Document document)
        {
            document.CreatedDate = DateTime.Now;
            document.ObjectState = ObjectState.Added;
            document.IsActive = true;
            _documentRepository.Insert(document);
            _unitOfWork.SaveChanges();

            return document;
        }

        public Document Delete(int documentId, string userId)
        {
            var document = _documentRepository
                .Queryable()
                .FirstOrDefault(c => c.DocumentId == documentId && c.Proposal.UserId == userId);

            if (document == null)
            {
                return null;
            }

            document.IsActive = false;
            document.ObjectState = ObjectState.Modified;
            _documentRepository.Update(document);
            _unitOfWork.SaveChanges();

            return document;
        }

        public List<Document> GetByProposalId(int proposalId, string userId)
        {
            var documents = _documentRepository
                .Queryable()
                .Where(x => x.ProposalId == proposalId && x.Proposal.UserId == userId && x.IsActive)
                .ToList();
            documents.AddRange(GetDefaultDocuments());
            return documents;
        }


        public List<Document> GetAll(string userId, int proposalId, FolderType folderType)
        {
            var documents = _documentRepository.Queryable()
                .Include(c => c.Proposal)
                .Where(x => x.ProposalId == proposalId && 
                            x.FolderType == folderType && 
                            x.Proposal.UserId == userId &&
                            x.IsActive)
                .ToList();
            documents.AddRange(GetDefaultDocuments(folderType));
            return documents;
        }

        public bool HaveDocument(string userId, int documentId)
        {
            return _documentRepository.Queryable()
                .Any(c => c.Proposal.UserId == userId && c.DocumentId == documentId && c.IsActive);
        }

        public Document GetById(int documentId)
        {
            return _documentRepository.Find(documentId);
        }

        public List<Document> GetAll(string userId, FolderType folderType)
        {

            var documents = _documentRepository
                .Queryable()
                .Include(c => c.Proposal)
                .Where(x => x.FolderType == folderType && x.Proposal.UserId == userId && x.IsActive)
                .ToList();
            documents.AddRange(GetDefaultDocuments(folderType));
            return documents;
        }

        public List<Document> GetAllByUserId(string userId)
        {

            var documents = _documentRepository
                .Queryable()
                .AsNoTracking()
                .Where(c => c.Proposal.UserId == userId && c.IsActive)
                .ToList();
            documents.AddRange(GetDefaultDocuments());
            return documents;
        }

        public Document GetByUserId(string userid, int documentId)
        {
            return _documentRepository
                .Queryable()
                .AsNoTracking()
                .FirstOrDefault(c => c.DocumentId == documentId && c.Proposal.UserId == userid && c.IsActive);
        }

        public void CreateDefault(Document document)
        {
            //Default document
            document.ProposalId = null;
            document.CreatedDate = DateTime.Now;
            document.ObjectState = ObjectState.Added;
            _documentRepository.Insert(document);
            _unitOfWork.SaveChanges();
        }

        public List<Document> GetDefaultDocuments(FolderType? folderType)
        {
            if (folderType.HasValue)
            {
                return _documentRepository
                    .Queryable()
                    .Where(c => c.FolderType == folderType.Value && c.ProposalId == null && c.IsActive)
                    .ToList();
            }
            return _documentRepository
                .Queryable()
                .Where(c => c.ProposalId == null && c.IsActive)
                .ToList();
        }

        public void DeleteDefaultDocument(int documentId)
        {
            var document = _documentRepository.Queryable().FirstOrDefault(c => c.DocumentId == documentId);

            if (document == null)
            {
                return;
            }

            document.IsActive = false;
            document.ObjectState = ObjectState.Modified;
            _documentRepository.Update(document);
            _unitOfWork.SaveChanges();
        }

        public MessageDto UploadFile(UploadDocumentDto uploadDocumentDto)
        {
            var document = uploadDocumentDto.ToCreateUploadDocumentDto();

            if (document == null)
            {
                return MessageDto.GetErrorMessage("Invalid data");
            }

            _documentRepository.Insert(document);
            _unitOfWork.SaveChanges();

            return MessageDto.GetSuccessMessage("Upload file successfully", document.DocumentId);
        }

        private List<Document> GetDefaultDocuments()
        {
            return _documentRepository
                .Queryable()
                .Where(c => c.ProposalId == null && c.IsActive)
                .ToList();
        }
        private List<Document> GetDefaultDocuments(FolderType folderType)
        {
            return _documentRepository
                .Queryable()
                .Where(c => c.ProposalId == null && c.FolderType == folderType && c.IsActive)
                .ToList();
        }
    }
}
