using System.Collections.Generic;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using FolderType = Portalia.Core.Enum.FolderType;

namespace Portalia.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            Documents = new List<Document>();
        }
        public FolderType FolderType { get; set; }
        public List<Document> Documents { get; set; }
        public string UserId { get; set; }
        public int? ProposalId { get; set; }
    }
}