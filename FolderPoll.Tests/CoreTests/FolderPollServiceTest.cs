using System.Collections.Generic;
using System.IO;
using FolderPoll.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolderPoll.Tests
{
    [TestClass]
    public class FolderPollServiceTest
    {
        [TestMethod]
        public void OperationsWithoutImpersonationTest()
        {
            string[] polledFolders = { "" };
            //var folderPoll = CreateFolderPoll()
        }

        private FolderPollXsd CreateFolderPollXsd(string[] polledFolders, string[] copyFolders, string[] moveFolders, string[] applications, string[] filters,
            bool isImpersonated, string domain = "", string username = "", string password = "")
        {
            var polls = new List<Poll>();
            foreach (var polledFolder in polledFolders)
            {
                foreach (var filter in filters)
                {
                    foreach (var copyFolder in copyFolders)
                    {
                        Poll pollCopy = new Poll
                        {
                            Folder = polledFolder,
                            NewFile = new NewFile
                            {
                                Copy = new NewFileCopy
                                {
                                    TargetFolder = copyFolder
                                },
                                Filter = filter
                            }
                        };
                        if (isImpersonated)
                        {
                            pollCopy.Impersonation = true;
                            pollCopy.Domain = domain;
                            pollCopy.Username = username;
                            pollCopy.Password = password;
                        }
                        polls.Add(pollCopy);

                        foreach (var application in applications)
                        {
                            List<Poll> pollsCopyApp = new List<Poll>
                            {
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Copy = new NewFileCopy
                                        {
                                            TargetFolder = copyFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = "{0}"
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Copy = new NewFileCopy
                                        {
                                            TargetFolder = copyFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = "{1}"
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Copy = new NewFileCopy
                                        {
                                            TargetFolder = copyFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = "{2}"
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Copy = new NewFileCopy
                                        {
                                            TargetFolder = copyFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = Path.Combine(copyFolder, "{3}")
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Copy = new NewFileCopy
                                        {
                                            TargetFolder = copyFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = Path.Combine(copyFolder, string.Concat("{4}", filter))
                                        },
                                        Filter = filter
                                    }
                                }
                            };
                            if (isImpersonated)
                            {
                                foreach (var pollCopyApp in pollsCopyApp)
                                {
                                    pollCopyApp.Impersonation = true;
                                    pollCopyApp.Domain = domain;
                                    pollCopyApp.Username = username;
                                    pollCopyApp.Password = password;
                                }
                            }
                            polls.AddRange(pollsCopyApp);
                        }
                    }

                    foreach (var moveFolder in moveFolders)
                    {
                        Poll pollMove = new Poll
                        {
                            Folder = polledFolder,
                            NewFile = new NewFile
                            {
                                Move = new NewFileMove
                                {
                                    TargetFolder = moveFolder
                                },
                                Filter = filter
                            }
                        };
                        if (isImpersonated)
                        {
                            pollMove.Impersonation = true;
                            pollMove.Domain = domain;
                            pollMove.Username = username;
                            pollMove.Password = password;
                        }
                        polls.Add(pollMove);

                        foreach (var application in applications)
                        {
                            List<Poll> pollsMoveApp = new List<Poll>
                            {
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Move = new NewFileMove
                                        {
                                            TargetFolder = moveFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = "{0}"
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Move = new NewFileMove
                                        {
                                            TargetFolder = moveFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = "{1}"
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Move = new NewFileMove
                                        {
                                            TargetFolder = moveFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = "{2}"
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Move = new NewFileMove
                                        {
                                            TargetFolder = moveFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = Path.Combine(moveFolder, "{3}")
                                        },
                                        Filter = filter
                                    }
                                },
                                new Poll
                                {
                                    Folder = polledFolder,
                                    NewFile = new NewFile
                                    {
                                        Move = new NewFileMove
                                        {
                                            TargetFolder = moveFolder
                                        },
                                        Launch = new NewFileLaunch
                                        {
                                            Application = application,
                                            Arguments = Path.Combine(moveFolder, string.Concat("{4}", filter))
                                        },
                                        Filter = filter
                                    }
                                }
                            };
                            if (isImpersonated)
                            {
                                foreach (var poll in pollsMoveApp)
                                {
                                    poll.Impersonation = true;
                                    poll.Domain = domain;
                                    poll.Username = username;
                                    poll.Password = password;
                                }
                            }
                            polls.AddRange(pollsMoveApp);
                        }
                    }
                }
            }

            return new FolderPollXsd
            {
                Poll = polls.ToArray()
            };
        }

        [TestMethod]
        public void TestAutoCreateFolderPoll()
        {
            string[] polledFolders = { @"C:\" };
            string[] copyFolders = { @"C:\" };
            string[] moveFolders = { @"C:\" };
        }
    }
}
