using System.Collections.Generic;

namespace Portalia.Core.Entity.WebApplication
{
    public class ApplicationSource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApplicationSource()
        {
            this.ApplicationForms = new HashSet<ApplicationForm>();
        }

        public byte ApplicationSourceId { get; set; }
        public string Label { get; set; }
        public int HoldingId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationForm> ApplicationForms { get; set; }
    }
}
