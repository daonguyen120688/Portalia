using System.Collections.Generic;

namespace Portalia.Core.Entity.WebApplication
{
    public class ApplicationFormStatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApplicationFormStatus()
        {
            this.ApplicationForms = new HashSet<ApplicationForm>();
        }

        public byte ApplicationFormStatusId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationForm> ApplicationForms { get; set; }
    }
}
