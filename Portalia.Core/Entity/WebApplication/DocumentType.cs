using System.Collections.Generic;

namespace Portalia.Core.Entity.WebApplication
{
    public class DocumentType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentType()
        {
            this.ApplicationDocuments = new HashSet<ApplicationDocument>();
        }

        public byte DocumentTypeId { get; set; }
        public string Label { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationDocument> ApplicationDocuments { get; set; }
    }
}
