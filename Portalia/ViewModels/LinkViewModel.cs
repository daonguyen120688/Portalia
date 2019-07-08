using System.Runtime.Serialization;

namespace Portalia.ViewModels
{
    public class LinkViewModel
    {
        public string Url { get; set; }
        public string Text { get; set; }
        public string Type {get; set;}
        [DataMember(Name = "MissingFieldCounting", EmitDefaultValue = false)]
        public string MissingFieldCounting { get; set; }
    }
}