using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portalia.Core.Attributes
{
    public class DataTypeInfor : Attribute
    {
        public FieldType FieldType { get; set; }
        public string DatasourceProperty{ get; set; }
        public string DateTimeFormat { get; set; }
    }

    public enum FieldType
    {
        Text=1,
        DropDown=2,
        DateTime=3,
        Radio=4,
        Checkbox=5,
        MultiSelect=6
    }
}
