using System.ServiceProcess;

namespace Portalia.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RemindCandidateToSubmitWorkContract()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
