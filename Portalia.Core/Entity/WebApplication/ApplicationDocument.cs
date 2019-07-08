using System;

namespace Portalia.Core.Entity.WebApplication
{
    public class ApplicationDocument
    {
        public int ApplicationDocumentId { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public byte[] Binary { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte TypeId { get; set; }
        public int ApplicationFormId { get; set; }

        public virtual ApplicationForm ApplicationForm { get; set; }
        public virtual DocumentType DocumentType { get; set; }
    }
}
