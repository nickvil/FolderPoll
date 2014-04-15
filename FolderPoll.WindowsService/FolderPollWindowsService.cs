using System.Configuration;
using System.ServiceProcess;
using System.IO;
using FolderPoll.Core;

namespace FolderPoll.WindowsService
{
    public partial class FolderPollWindowsService : ServiceBase
    {
        private IFolderPollService folderPollService;

        private string ExecutingAssemblyFolder
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        public FolderPollWindowsService()
        {
            InitializeComponent();
            string configurationFilePath = Path.Combine(this.ExecutingAssemblyFolder, ConfigurationManager.AppSettings["FolderPollConfigurationFileName"]);
            this.folderPollService = new FolderPollService(configurationFilePath, true);
        }

        protected override void OnStart(string[] args)
        {
            folderPollService.Run();
        }

        protected override void OnStop()
        {
            folderPollService.Stop();
        }
    }
}
