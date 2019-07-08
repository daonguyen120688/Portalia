using System;

namespace Portalia.ViewModels
{
    public class RequestInfoModel
    {
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string Url { get; set; }
        public string IpAddress { get; set; }
        public BrowserModel Browser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TrackingActionType { get; set; }
    }

    public class BrowserModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
    }
}