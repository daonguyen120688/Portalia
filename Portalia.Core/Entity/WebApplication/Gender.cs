using System.Collections.Generic;

namespace Portalia.Core.Entity.WebApplication
{
    public class Gender
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gender()
        {
            this.ApplicationFormDetails = new HashSet<ApplicationFormDetail>();
        }

        public byte GenderId { get; set; }
        public string Label { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationFormDetail> ApplicationFormDetails { get; set; }
    }
}
