using System;
using Portalia.Core.Dtos.Document;
using Portalia.Core.Entity;
using Repository.Pattern.Infrastructure;

namespace Portalia.Core.Extensions
{
    public static class DocumentExtension
    {
        public static Document ToCreateUploadDocumentDto(this UploadDocumentDto uploadDocumentDto)
        {
            if (uploadDocumentDto == null)
            {
                return null;
            }

            return new Document
            {
                FileBinary = uploadDocumentDto.FileBinary,
                ProposalId = uploadDocumentDto.ProposalId,
                Name = uploadDocumentDto.FileName.Replace(" ", string.Empty),
                FolderType = uploadDocumentDto.FolderType,
                CreatedDate = DateTime.Now,
                ObjectState = ObjectState.Added,
                IsActive = true
            };
        }
    }
}
