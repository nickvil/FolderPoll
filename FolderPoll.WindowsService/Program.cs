using System.ServiceProcess;

namespace FolderPoll.WindowsService
{
    static class Program
    {
        static void Main()
        {
            var servicesToRun  = new ServiceBase[] 
            { 
                new FolderPollWindowsService() 
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
