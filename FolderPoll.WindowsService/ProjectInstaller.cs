using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace FolderPoll.WindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void FolderPollWindowsServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void FolderPollWindowsServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
