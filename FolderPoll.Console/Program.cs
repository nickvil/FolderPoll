namespace FolderPoll.Console
{
    using System.Configuration;
    using System.IO;

    using FolderPoll.Core;

    class Program
    {
        private static IFolderPollService folderPollService;

        private static string ExecutingAssemblyFolder
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        static void Main()
        {
            var configurationFilePath = Path.Combine(ExecutingAssemblyFolder, ConfigurationManager.AppSettings["FolderPollConfigurationFileName"]);
            folderPollService = new FolderPollService(configurationFilePath, true);
            folderPollService.Run();

            System.Console.WriteLine("FolderPoll service started! Please press any key to stop service");
            System.Console.ReadKey(false);

            folderPollService.Stop();
        }
    }
}
