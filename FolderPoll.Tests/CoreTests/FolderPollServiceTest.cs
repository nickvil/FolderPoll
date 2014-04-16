using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using FolderPoll.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            File.Create(Path.Combine(firstPollPath, string.Concat(firstName, Path.GetExtension(firstFilter))));
            File.Create(Path.Combine(secondPollPath, string.Concat(secondName, Path.GetExtension(secondFilter))));

            Assert.AreEqual(true, File.Exists(Path.Combine(copyPath, string.Concat(firstName, Path.GetExtension(firstFilter)))));
            Assert.AreEqual(true, File.Exists(Path.Combine(movePath, string.Concat(secondName, Path.GetExtension(secondFilter)))));
        }
    }
}
