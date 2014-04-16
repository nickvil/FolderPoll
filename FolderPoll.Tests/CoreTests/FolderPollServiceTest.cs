using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using FolderPoll.Core;
using FolderPoll.Tests.CoreTests;
using FolderPoll.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolderPoll.Tests
{
    [TestClass]
    public class FolderPollServiceTest
    {
        private const string TestFolderPath = @"C:\FolderPoll";
        private const string ProtectedTestFolderPath = @"C:\FolderPollProtected";
        private readonly TestUser ProtectedUser = new TestUser
        {
           GroupName = "TestGroup",
           GroupDescription = "Group for FolderPoll's unit tests",
           UserName = "FolderPoll Test User",
           UserDescription = "User for FolderPoll's unit tests",
           UserPassword = "password"
        };
        private readonly string xmlConfigPath = Path.Combine(TestFolderPath, "TestConfiguration.xml");
        private readonly Random rnd = new Random();

        private readonly List<Poll> polls = new List<Poll>();
        private readonly List<TestFile> testFiles = new List<TestFile>();

        private string WithoutImpersonationMessage;
        private string WithImpersonationMessage;

        [TestMethod]
        public void WithoutImpersonationTest()
        {
            WithoutImpersonationMessage = string.Empty;

            this.PrepareTestEnviroment(false);  // prepare polls, creation of all test folders

            var folderPollConfig = new FolderPollConfig
            {
                Poll = polls.ToArray()
            };

            var serializer = new XmlSerializer(typeof(FolderPollConfig));
            TextWriter writer = new StreamWriter(xmlConfigPath);
            serializer.Serialize(writer, folderPollConfig);
            writer.Close();

            var folderPollService = new FolderPollService(xmlConfigPath, true);
            folderPollService.Run();

            EmulateService(false);

            Thread.Sleep(7000);

            FilesExistsTest();

            folderPollService.Stop();

            Assert.AreEqual("passed", WithoutImpersonationMessage);

        }

        [TestMethod]
        public void WithImpersonationTest()
        {
            WithImpersonationMessage = string.Empty;

            this.PrepareTestEnviroment(true);  // prepare polls, creation of all test folders

            var folderPollConfig = new FolderPollConfig
            {
                Poll = polls.ToArray()
            };

            var serializer = new XmlSerializer(typeof(FolderPollConfig));
            TextWriter writer = new StreamWriter(xmlConfigPath);
            serializer.Serialize(writer, folderPollConfig);
            writer.Close();

            var folderPollService = new FolderPollService(xmlConfigPath, true);
            folderPollService.Run();

            EmulateService(true);

            Thread.Sleep(7000);

            FilesExistsTest();

            folderPollService.Stop();

            Assert.AreEqual("passed", WithImpersonationMessage);

        }

        [TestMethod]
        public void FilesExistsTest()
        {
            const string errorMessage = "Action: {0}. Target Folder: {1}. Not Exist File: {2}";
            const string successMessage = "{0} FILE {1} TO {2}. SUCCESS!";
            foreach (var testFile in testFiles)
            {
                bool isSuccess;
                if (testFile.HasCopyAction)
                {
                    isSuccess = File.Exists(Path.Combine(testFile.PollCopyTargetFolder, testFile.FileName + testFile.FileExtension));
                    Assert.IsTrue(isSuccess, string.Format(errorMessage, "COPY", testFile.FileName + testFile.FileExtension, testFile.PollCopyTargetFolderName));

                    if (isSuccess)
                    {
                        Console.WriteLine(successMessage, "COPY", testFile.FileName + testFile.FileExtension, testFile.PollCopyTargetFolderName);
                    }
                }

                if (testFile.HasMoveAction)
                {
                    isSuccess = File.Exists(Path.Combine(testFile.PollMoveTargetFolder, testFile.FileName + testFile.FileExtension));
                    Assert.IsTrue(isSuccess, string.Format(errorMessage, "MOVE", testFile.FileName + testFile.FileExtension, testFile.PollMoveTargetFolderName));

                    if (isSuccess)
                    {
                        Console.WriteLine(successMessage, "MOVE", testFile.FileName + testFile.FileExtension, testFile.PollMoveTargetFolderName);
                    }
                }
            }

            WithoutImpersonationMessage = "passed";
            WithImpersonationMessage = "passed";
        }

        private void EmulateService(bool isImpersonated)
        {
            // Random files creation for all polls (for each pool - random from 3 to 10 files)
            foreach (var poll in polls)
            {
                var filesCount = rnd.Next(3, 11);
                for (int i = 0; i < filesCount; i++)
                {
                    var testFile = new TestFile();
                    testFile.PollFolder = poll.Folder;
                    testFile.FileName = Guid.NewGuid().ToString();
                    testFile.FileFilter = poll.NewFile.Filter;

                    if (poll.NewFile.Copy != null)
                    {
                        testFile.PollCopyTargetFolder = poll.NewFile.Copy.TargetFolder;
                    }

                    if (poll.NewFile.Move != null)
                    {
                        testFile.PollMoveTargetFolder = poll.NewFile.Move.TargetFolder;
                    }

                    testFiles.Add(testFile);

                    if (isImpersonated)
                    {
                        using (new ImpersonatedUser(poll.Domain, poll.Username, poll.Password))
                        {
                            File.Create(Path.Combine(testFile.PollFolder, testFile.FileName + testFile.FileExtension)).Dispose();
                        }
                    }
                    else
                    {
                        File.Create(Path.Combine(testFile.PollFolder, testFile.FileName + testFile.FileExtension)).Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Generation of Polls, test directiories creation
        /// </summary>
        /// <param name="impersonation">Is use impersonation or not</param>
        private void PrepareTestEnviroment(bool impersonation)
        {
            // fill random input folders
            var rndInputFoldersCount = rnd.Next(10, 21);    // from 10 to 20 Polls

            CreateRootFolders();

            for (int i = 0; i < rndInputFoldersCount; i++)
            {
                string folderName = Guid.NewGuid().ToString();

                string targetFolder;

                var poll = new Poll
                {
                    Impersonation = impersonation
                };

                if (impersonation)
                {
                    poll.Domain = "";
                    poll.Username = this.ProtectedUser.UserName;
                    poll.Password = this.ProtectedUser.UserPassword;
                    targetFolder = ProtectedTestFolderPath;
                }
                else
                {
                    targetFolder = TestFolderPath;
                }

                poll.Folder = Path.Combine(targetFolder, folderName);

                poll.NewFile = new NewFile
                {
                    Filter = "*." + TestHelper.GetRandomAlphaNumericFileExtension(3)
                };

                // Random Action: 0 - CopyFile, 1 - MoveFile, 2 - Copy And Move
                var variant = rnd.Next(0, 3);
                if (variant == 0 || variant == 2)
                {
                    poll.NewFile.Copy = new NewFileCopy
                    {
                        TargetFolder = Path.Combine(targetFolder, "out_copy_" + folderName),
                    };
                }
                if (variant == 1 || variant == 2)
                {
                    poll.NewFile.Move = new NewFileMove
                    {
                        TargetFolder = Path.Combine(targetFolder, "out_move_" + folderName)
                    };
                }

                polls.Add(poll);

                CreatePollFolders(poll, impersonation);
            }
        }

        private void CreateRootFolders()
        {
            if (Directory.Exists(TestFolderPath))
            {
                Directory.Delete(TestFolderPath, true);
            }

            Directory.CreateDirectory(TestFolderPath);

            if (Directory.Exists(ProtectedTestFolderPath))
            {
                using (new ImpersonatedUser(string.Empty, ProtectedUser.UserName, ProtectedUser.UserPassword))
                {
                    Directory.Delete(ProtectedTestFolderPath, true);
                }
            }

            Directory.CreateDirectory(ProtectedTestFolderPath);
        }

        private void CreatePollFolders(Poll poll, bool isProtected)
        {
            if (!isProtected)
            {
                Directory.CreateDirectory(poll.Folder);

                if (poll.NewFile.Copy != null)
                {
                    Directory.CreateDirectory(poll.NewFile.Copy.TargetFolder);
                }

                if (poll.NewFile.Move != null)
                {
                    Directory.CreateDirectory(poll.NewFile.Move.TargetFolder);
                } 
            }
            else
            {
                var context = new PrincipalContext(ContextType.Machine);
                FolderProtectHelper.SetupSecurityGroupAndUser(context, ProtectedUser.GroupName,
                    ProtectedUser.GroupDescription, ProtectedUser.UserName, ProtectedUser.UserPassword,
                    ProtectedUser.UserDescription);

                string currentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                FolderProtectHelper.ProtectFolder(poll.Folder, ProtectedUser.UserName, currentUserName, true);

                if (poll.NewFile.Copy != null)
                {
                    FolderProtectHelper.ProtectFolder(poll.NewFile.Copy.TargetFolder, ProtectedUser.UserName, currentUserName, true);
                }

                if (poll.NewFile.Move != null)
                {
                    FolderProtectHelper.ProtectFolder(poll.NewFile.Move.TargetFolder, ProtectedUser.UserName, currentUserName, true);
                }
            }
        }
    }

    public class TestUser
    {
        public string GroupName { get; set; }
        
        public string GroupDescription { get; set; }
        
        public string UserName { get; set; }
        
        public string UserPassword { get; set; }

        public string UserDescription { get; set; }
    }

    public class TestFile
    {
        public string PollFolder { get; set; }

        public string PollCopyTargetFolder { get; set; }

        public string PollMoveTargetFolder { get; set; }

        public string FileName { get; set; }

        public string FileFilter { get; set; }

        public string FileExtension
        {
            get { return Path.GetExtension(this.FileFilter); }
        }

        public bool HasCopyAction
        {
            get { return !string.IsNullOrEmpty(this.PollCopyTargetFolder); }
        }

        public bool HasMoveAction
        {
            get { return !string.IsNullOrEmpty(this.PollMoveTargetFolder); }
        }

        public string PollCopyTargetFolderName
        {
            get { return Path.GetFileName(this.PollCopyTargetFolder); }
        }

        public string PollMoveTargetFolderName
        {
            get { return Path.GetFileName(this.PollMoveTargetFolder); }
        }
    }
}
