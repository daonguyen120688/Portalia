using System;
using System.Collections.Generic;

namespace Portalia.Core.Entity.WebApplication
{
    public class ApplicationForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApplicationForm()
        {
            this.ApplicationDocuments = new HashSet<ApplicationDocument>();
        }

        public int ApplicationFormId { get; set; }
        public byte StatusId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Email { get; set; }
        public byte ApplicationSourceId { get; set; }
        public Nullable<int> JobOfferId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationDocument> ApplicationDocuments { get; set; }
        public virtual ApplicationFormStatus ApplicationFormStatus { get; set; }
        public virtual ApplicationSource ApplicationSource { get; set; }
        public virtual ApplicationFormDetail ApplicationFormDetail { get; set; }
    }
}
