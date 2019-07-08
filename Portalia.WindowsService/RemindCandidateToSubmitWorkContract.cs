using System;
using System.Configuration;
using System.Net.Http;
using System.ServiceProcess;

namespace Portalia.WindowsService
{
    public partial class RemindCandidateToSubmitWorkContract : ServiceBase
    {
        private static readonly string ApiKeyName = ConfigurationManager.AppSettings.Get("API.ApiKeyName");
        private static readonly string ApiKeyValue = ConfigurationManager.AppSettings.Get("API.ApiKeyValue");
        private static readonly string BaseAddress = ConfigurationManager.AppSettings.Get("BaseAPI");

        public RemindCandidateToSubmitWorkContract()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            // Create an HttpClient instance
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseAddress);
            //client.BaseAddress = new Uri("http://localhost:54501/");

            client.DefaultRequestHeaders.Add(ApiKeyName, ApiKeyValue);

            // Usage
            var response = client.GetAsync("api/workcontract/SendReminderToCandidate").Result;
            LogHelpers.WriteInfoLog($"Call {BaseAddress}api/workcontract/SendReminderToCandidate");
            if (response.IsSuccessStatusCode)
            {
                LogHelpers.WriteInfoLog("API called successfully");
            }
            else
            {
                LogHelpers.WriteErrorLog(
                    $"Cannot call API: Status Code: {response.StatusCode}, Message: {response.ReasonPhrase}");
            }
        }

        protected override void OnStop()
        {

        }
    }
}
