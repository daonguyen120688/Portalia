using System;
using System.Collections.Generic;
using System.Linq;
using Portalia.Core.Entity;

namespace Portalia.Extentions
{
    public static class AttributeDetailExtention
    {
        public static AttributeDetail GetByName(this List<AttributeDetail> attributeDetails, string name)
        {
            if (string.IsNullOrEmpty(name) || attributeDetails.All(c => c.Name != name))
            {
                throw new Exception($"not found {name}");
            }

            return attributeDetails.Single(c => c.Name == name);
        }

        public static List<AttributeDetail> SortByList(this List<AttributeDetail> attributeDetails, List<string> names)
        {
            return names.Select(name => attributeDetails.First(c => c.Name.RemoveSpace() == name)).ToList();
        }
    }
}