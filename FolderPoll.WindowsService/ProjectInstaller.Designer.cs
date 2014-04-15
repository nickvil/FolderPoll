namespace FolderPoll.WindowsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FolderPollWindowsServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FolderPollWindowsServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FolderPollWindowsServiceProcessInstaller
            // 
            this.FolderPollWindowsServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FolderPollWindowsServiceProcessInstaller.Password = null;
            this.FolderPollWindowsServiceProcessInstaller.Username = null;
            this.FolderPollWindowsServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.FolderPollWindowsServiceProcessInstaller_AfterInstall);
            // 
            // FolderPollWindowsServiceInstaller
            // 
            this.FolderPollWindowsServiceInstaller.ServiceName = "FolderPollWindowsService";
            this.FolderPollWindowsServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.FolderPollWindowsServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.FolderPollWindowsServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FolderPollWindowsServiceProcessInstaller,
            this.FolderPollWindowsServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FolderPollWindowsServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FolderPollWindowsServiceInstaller;
    }
}