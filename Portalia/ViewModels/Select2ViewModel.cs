using Newtonsoft.Json;

namespace Portalia.ViewModels
{
    public class Select2ViewModel
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}