using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FolderPoll.Core
{
    using System.Xml;
    using System.Xml.Serialization;

    public class FolderPollService : IFolderPollService
    {
        private FolderPollXsd folderPoll;

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
                var ser = new XmlSerializer(typeof(FolderPollXsd));
                using (var reader = XmlReader.Create(configurationFilePathOrString))
                {
                    this.folderPoll = (FolderPollXsd)ser.Deserialize(reader);
                }
            }
            else
            {
                var ser = new XmlSerializer(typeof(FolderPollXsd));
                using (var reader = new StringReader(configurationFilePathOrString))
                {
                    this.folderPoll = (FolderPollXsd)ser.Deserialize(reader);
                }
            }
        }

        public void Run()
        {
            watchers.Clear();
            foreach (var poll in this.folderPoll.Poll)
            {
                this.InitPoll(poll);
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

        private void InitPoll(Poll poll)
        {
            if (poll == null || poll.NewFile == null)
            {
                return;
            }

            var watcher = new FileSystemWatcher(poll.Folder, poll.NewFile.Filter);


            if (poll.NewFile.Copy != null)
            {
                AttachAction(poll, FolderPollAction.Copy, watcher);
            }

            if (poll.NewFile.Move != null)
            {
                AttachAction(poll, FolderPollAction.Move, watcher);
            }

            if (poll.NewFile.Launch != null)
            {
                AttachAction(poll, FolderPollAction.Launch, watcher);
            }

            // Add watcher to list and EnableRaisingEvents only if we have at least one action
            if (poll.NewFile.Copy != null || poll.NewFile.Move != null || poll.NewFile.Launch != null)
            {
                watcher.EnableRaisingEvents = true;
                this.watchers.Add(watcher);
            }
        }

        private static void AttachAction(Poll poll, FolderPollAction action, FileSystemWatcher watcher)
        {
            if (poll.Impersonation && poll.ImpersonationSpecified)
            {
                switch (action)
                {
                    case FolderPollAction.Copy:
                        watcher.Created += delegate(object sender, FileSystemEventArgs args)
                        {
                            using (new ImpersonatedUser(poll.Domain, poll.Username, poll.Password))
                            {
                                CopyFile(args.FullPath, Path.Combine(poll.NewFile.Copy.TargetFolder, args.Name));
                            }
                        };
                        break;
                    case FolderPollAction.Move:
                        watcher.Created += delegate(object sender, FileSystemEventArgs args)
                        {
                            using (new ImpersonatedUser(poll.Domain, poll.Username, poll.Password))
                            {
                                MoveFile(args.FullPath, Path.Combine(poll.NewFile.Move.TargetFolder, args.Name));
                            }
                        };
                        break;
                    case FolderPollAction.Launch:
                        watcher.Created += delegate(object sender, FileSystemEventArgs args)
                        {
                            using (new ImpersonatedUser(poll.Domain, poll.Username, poll.Password))
                            {
                                LaunchApplication(poll.NewFile.Launch.Application, PrepareLaunchArgs(poll, args.FullPath, args.Name));
                            }
                        };
                        break;
                }
            }
            else
            {
                switch (action)
                {
                    case FolderPollAction.Copy:
                        watcher.Created += (sender, args) => CopyFile(args.FullPath, Path.Combine(poll.NewFile.Copy.TargetFolder, args.Name));
                        break;
                    case FolderPollAction.Move:
                        watcher.Created += (sender, args) => MoveFile(args.FullPath, Path.Combine(poll.NewFile.Move.TargetFolder, args.Name));
                        break;
                    case FolderPollAction.Launch:
                        watcher.Created += (sender, args) => LaunchApplication(poll.NewFile.Launch.Application, PrepareLaunchArgs(poll, args.FullPath, args.Name));
                        break;
                }
            }
        }

        private static void CopyFile(string filePath, string destinationPath)
        {
            if (File.Exists(filePath))
            {
                File.Copy(filePath, destinationPath, true);
            }
        }

        private static void MoveFile(string filePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            if (File.Exists(filePath))
            {
                File.Move(filePath, destinationPath);
            }
        }

        private static void LaunchApplication(string applicationPath, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = applicationPath,
                    Arguments = arguments
                }
            };

            process.Start();
        }

        private static string PrepareLaunchArgs(Poll poll, string fullPath, string nameWithExt)
        {
            string copiedFileRef = Path.Combine(poll.NewFile.Copy.TargetFolder, nameWithExt);
            string movedFileRef = Path.Combine(poll.NewFile.Move.TargetFolder, nameWithExt);
            string nameWithoutExt = Path.GetFileName(nameWithExt);

            return string.Format(poll.NewFile.Launch.Arguments, fullPath, copiedFileRef, movedFileRef, nameWithExt, nameWithoutExt);
        }

        enum FolderPollAction
        {
            Copy,
            Move,
            Launch
        }
    }
}
