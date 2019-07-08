using Portalia.Core.Enum;

namespace Portalia.Core.Dtos.Document
{
    public class UploadDocumentDto
    {
        public string UserId { get; set; }
        public int ProposalId { get; set; }
        public FolderType FolderType { get; set; }
        public string FileName { get; set; }
        public byte[] FileBinary { get; set; }
    }
}
