using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class AttributeType: Repository.Pattern.Ef6.Entity
    {
        public AttributeType()
        {
            AttributeDetails = new List<AttributeDetail>();
        }
        public int AttributeTypeId { get; set; }
        public string Label { get; set; }
        public virtual ICollection<AttributeDetail> AttributeDetails { get; set; }
    }
}
