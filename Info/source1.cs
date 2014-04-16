using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FolderPoll.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.AccountManagement;


namespace FolderPoll.Tests
{
    [TestClass]
    public class FolderPollServiceTest
    {
        [TestMethod]
        public void FolderPollServiceFileInitWithoutImpersonationTest()
        {
            string testPath = @"C:\FolderPoll";
            string configPath = Path.Combine(testPath, "ExampleConfiguration.xml");
            string firstPollPath = Path.Combine(testPath, "FirstPoll");
            string secondPollPath = Path.Combine(testPath, "SecondPoll");
            string firstFilter = "*.txt";
            string secondFilter = "*.xml";
            string firstName = "firstNewFile";
            string secondName = "secondNewFile";
            string copyPath = Path.Combine(testPath, "PollCopy");
            string movePath = Path.Combine(testPath, "PollMove");

            if (Directory.Exists(testPath))
            {
                Directory.Delete(testPath, true);
            }

            if (!Directory.Exists(testPath))
            {
                Directory.CreateDirectory(testPath);
            }
            if (!Directory.Exists(firstPollPath))
            {
                Directory.CreateDirectory(firstPollPath);
            }
            if (!Directory.Exists(secondPollPath))
            {
                Directory.CreateDirectory(secondPollPath);
            }
            if (!Directory.Exists(copyPath))
            {
                Directory.CreateDirectory(copyPath);
            }
            if (!Directory.Exists(movePath))
            {
                Directory.CreateDirectory(movePath);
            }

            var polls = new List<Poll>
            {
                new Poll
                {
                    Folder = firstPollPath,
                    NewFile = new NewFile
                    {
                        Filter = firstFilter,
                        Copy = new NewFileCopy
                        {
                            TargetFolder = copyPath
                        }
                    }
                },
                new Poll
                {
                    Folder = secondPollPath,
                    NewFile = new NewFile
                    {
                        Filter = secondFilter,
                        Move = new NewFileMove
                        {
                            TargetFolder = movePath
                        }
                    }
                }
            };

            var folderPollConfig = new FolderPollConfig
            {
                Poll = polls.ToArray()
            };

            var serializer = new XmlSerializer(typeof(FolderPollConfig));
            TextWriter writer = new StreamWriter(configPath);
            serializer.Serialize(writer, folderPollConfig);
            writer.Close();

            var folderPollService = new FolderPollService(configPath, true);

            folderPollService.Run();

            File.Create(Path.Combine(firstPollPath, string.Concat(firstName, Path.GetExtension(firstFilter)))).Dispose();
            File.Create(Path.Combine(secondPollPath, string.Concat(secondName, Path.GetExtension(secondFilter)))).Dispose();

            folderPollService.Stop();

            Assert.AreEqual(true, File.Exists(Path.Combine(copyPath, string.Concat(firstName, Path.GetExtension(firstFilter)))));
            Assert.AreEqual(true, File.Exists(Path.Combine(movePath, string.Concat(secondName, Path.GetExtension(secondFilter)))));

            if (Directory.Exists(testPath))
            {
                Directory.Delete(testPath, true); 
            }
        }

       /* [TestMethod]
        public void FolderPollServiceWithImpersonationTest()
        {
            var context = new PrincipalContext(ContextType.Machine);
            Helpers.FolderProtectHelper.SetupSecurityGroupAndUser(context, "TestGroup", "Test group for unit test",
                "TestUser", "Password12!@", "Test user for unit test");

            string currentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Helpers.FolderProtectHelper.ProtectFolder(@"c:\FolderPoll\Protected", "TestUser", currentUserName, true);

            var str = string.Empty;
        }*/

        [TestMethod]
        public void FolderPollInitTest()
        {
            FolderPollConfig testConfig = new FolderPollConfig
            {
                Poll = new Poll[]
                {
                    new Poll
                    {
                        Domain = "domain",
                        Folder = @"C:\FolderPoll",
                        Impersonation = true,
                        Username = "username",
                        Password = "password",
                        NewFile = new NewFile
                        {
                            Filter = "*.txt",
                            Copy = new NewFileCopy
                            {
                                TargetFolder = @"C:\FolderPoll"
                            },
                            Move = new NewFileMove
                            {
                                TargetFolder = @"C:\FolderPoll"
                            },
                            Launch = new NewFileLaunch
                            {
                                Application = @"C:\Windows\notepad.exe",
                                Arguments = "{0}"
                            }
                        }
                    } 
                }
            };

            var serializer = new XmlSerializer(typeof(FolderPollConfig));
            TextWriter streamWriter = new StreamWriter(@"C:\FolderPoll\testConfig.xml");
            serializer.Serialize(streamWriter, testConfig);
            streamWriter.Close();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, testConfig, ns);
            stringWriter.GetStringBuilder().ToString();

            var fileInitObject = new FolderPollService();
            fileInitObject.Init(@"C:\FolderPoll\testConfig.xml", true);
            var stringInitObject = new FolderPollService();
            stringInitObject.Init(stringWriter.GetStringBuilder().ToString(), false);

            var privateFileInit = new PrivateObject(fileInitObject);
            var privateStringInit = new PrivateObject(stringInitObject);

            Assert.IsNotNull(privateFileInit.GetField("folderPoll"));
            Assert.IsNotNull(privateStringInit.GetField("folderPoll"));
        }
    }
}
