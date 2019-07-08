using System.Collections.Generic;

namespace Portalia.Core.Entity.WebApplication
{
    public class Title
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Title()
        {
            this.ApplicationFormDetails = new HashSet<ApplicationFormDetail>();
        }

        public byte TitleId { get; set; }
        public string Label { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationFormDetail> ApplicationFormDetails { get; set; }
    }
}
