using System;

namespace Portalia.Core.Dtos
{
    public class ApplicationDocumentDto
    {
        public int ApplicationDocumentId { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public byte[] Binary { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte TypeId { get; set; }
        public int ApplicationFormId { get; set; }
    }
}
