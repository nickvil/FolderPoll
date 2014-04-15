using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FolderPoll.Core
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class FolderPollService : IFolderPollService
    {
        private FolderPoll folderPoll;

        private readonly List<FileSystemWatcher> watchers;

        /// <summary>
        /// FolderPollService Constructor
        /// </summary>
        /// <param name="configurationFilePathOrString">Path to XML configuration file or just XML string</param>
        /// <param name="isFile">true - XML file, false - XML string</param>
        public FolderPollService(string configurationFilePathOrString, bool isFile)
        {
            this.watchers = new List<FileSystemWatcher>();
            this.Init(configurationFilePathOrString, isFile);
        }

        /// <summary>
        /// Initialization of service
        /// </summary>
        /// <param name="configurationFilePathOrString">Path to XML configuration file or just XML string</param>
        /// <param name="isFile">true - XML file, false - XML string</param>
        public void Init(string configurationFilePathOrString, bool isFile)
        {
            if (isFile)
            {
                var ser = new XmlSerializer(typeof(FolderPoll));
                using (var reader = XmlReader.Create(configurationFilePathOrString))
                {
                    this.folderPoll = (FolderPoll)ser.Deserialize(reader);
                }
            }
            else
            {
                var ser = new XmlSerializer(typeof(FolderPoll));
                using (var reader = new StringReader(configurationFilePathOrString))
                {
                    this.folderPoll = (FolderPoll)ser.Deserialize(reader);
                }
            }
        }

        public void Run()
        {
            watchers.Clear();
            foreach (var poll in this.folderPoll.Poll)
            {
                this.ProcessPoll(poll);

                try
                {
                    if (poll.ImpersonationSpecified && poll.Impersonation)
                    {
                        using (var user = new ImpersonatedUser(poll.Username, poll.Password))
                        {
                            this.ProcessPoll(poll);
                        }
                    }
                    else
                    {
                        this.ProcessPoll(poll);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void Stop()
        {
            foreach (var watcher in this.watchers)
            {
                if (watcher.EnableRaisingEvents)
                {
                    watcher.EnableRaisingEvents = false;
                }
            }
        }

        private void ProcessPoll(Poll poll)
        {
            if (poll == null ||  poll.NewFile == null)
            {
                return;
            }

            var watcher = new FileSystemWatcher(poll.Folder, poll.NewFile.Filter);
            
            if (poll.NewFile.Copy != null)
            {
                this.AttachAction(poll, FolderPollAction.Copy, watcher);
            }

            if (poll.NewFile.Move != null)
            {
                this.AttachAction(poll, FolderPollAction.Move, watcher);
            }

            if (poll.NewFile.Launch != null)
            {
                this.AttachAction(poll, FolderPollAction.Launch, watcher);
            }

            // Add wather to list and EnableRaisingEvents only if we have at least one action
            if (poll.NewFile.Copy != null || poll.NewFile.Move != null || poll.NewFile.Launch != null)
            {
                watcher.EnableRaisingEvents = true;
                this.watchers.Add(watcher);
            }
        }

        private void CopyFile(string filePath, string destinationPath)
        {
            File.Copy(filePath, destinationPath);
        }

        private void MoveFile(string filePath, string destinationPath)
        {
            File.Move(filePath, destinationPath);
        }

        private void LaunchApplication(string applicationPath, string filePath, string arguments)
        {
            var process = new Process();

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = applicationPath;
            startInfo.Arguments = string.Format("\"{0}\" {1}", filePath, arguments);
            process.StartInfo = startInfo;

            process.Start();
        }

        private void AttachAction(Poll poll, FolderPollAction action, FileSystemWatcher watcher)
        {
            if (poll.ImpersonationSpecified && poll.Impersonation)
            {
                using (var user = new ImpersonatedUser(poll.Username, poll.Password))
                {
                    switch (action)
                    {
                        case FolderPollAction.Copy:
                            watcher.Created += (sender, args) => this.CopyFile(args.FullPath, string.Format("{0}\\{1}", poll.NewFile.Move.TargetFolder, args.Name));
                            break;
                        case FolderPollAction.Move:
                            watcher.Created += (sender, args) => this.MoveFile(args.FullPath, string.Format("{0}\\{1}", poll.NewFile.Move.TargetFolder, args.Name));
                            break;
                        case FolderPollAction.Launch:
                            watcher.Created +=(sender, args) => this.LaunchApplication(poll.NewFile.Launch.Application, args.FullPath, poll.NewFile.Launch.Arguments);
                            break;
                    }
                }
            }
            else
            {
                
            }
        }
    }

    public enum FolderPollAction
    {
        Copy,
        Move,
        Launch
    }
}
