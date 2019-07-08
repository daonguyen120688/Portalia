using System.Collections.Generic;
using Newtonsoft.Json;

namespace Portalia.Core.Entity
{
    public class AspNetRole
    {
        public AspNetRole()
        {
            AspNetUsers = new HashSet<AspNetUser>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
