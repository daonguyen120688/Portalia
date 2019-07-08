using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class AttributeDetail : Repository.Pattern.Ef6.Entity
    {
        public AttributeDetail()
        {
            UserProfileAttributes = new List<UserProfileAttribute>();
        }
        public int AttributeDetailId { get; set; }
        public string Name { get; set; }
        public int AttributeTypeId { get; set; }
        public virtual AttributeType AttributeType { get; set; }
        public virtual ICollection<UserProfileAttribute> UserProfileAttributes { get; set; }
        public string Type { get; set; }
        public string DataSourceUrl { get; set; }
        public bool IsRequired { get; set; }
    }
}
