using System;

namespace Portalia.Core.Entity
{
    public class Document : Repository.Pattern.Ef6.Entity
    {
        public int DocumentId { get; set; }
        public int? ProposalId { get; set; }
        public string Name { get; set; }
        public virtual Proposal Proposal { get; set; }
        public Enum.FolderType FolderType { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] FileBinary { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
