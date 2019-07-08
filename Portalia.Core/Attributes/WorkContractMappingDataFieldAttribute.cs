using System;

namespace Portalia.Core.Attributes
{
    // This attribute provides the custom name for specific property
    // Mark property with this attribute to check property name and data field value
    public class WorkContractMappingDataFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public WorkContractMappingDataFieldAttribute(string name)
        {
            Name = name;
        }
    }
}
