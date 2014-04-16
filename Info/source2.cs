 /*
        [TestMethod]
        public void FolderPollServiceFileInitWithoutImpersonationTest()
        {
            string polledPath = @"D:\FolderPoll";

            if (!Directory.Exists(polledPath))
            {
                Directory.CreateDirectory(polledPath);
            }

            string[] polledFolders = { polledPath };
            string[] copyFolders = { Path.Combine(polledPath, "FolderPollCopy") };
            string[] moveFolders = { Path.Combine(polledPath, "FolderPollMove") };
            string[] applications = { @"D:\Windows\notepad.exe" };
            string[] filters = { ".txt" };
            string domain = "domain";
            string username = "username";
            string password = "password";

            foreach (var copyFolder in copyFolders)
            {
                if (!Directory.Exists(copyFolder))
                {
                    Directory.CreateDirectory(copyFolder);
                }
            }

            foreach (var moveFolder in moveFolders)
            {
                if (!Directory.Exists(moveFolder))
                {
                    Directory.CreateDirectory(moveFolder);
                }
            }

            var folderPollXsd = AutoCreateFolderPollXsd(polledFolders, copyFolders, moveFolders, applications, filters,
                false, domain, username, password);

            XmlSerializer serializer = new XmlSerializer(typeof(FolderPollXsd));
            TextWriter writer = new StreamWriter(Path.Combine(polledPath, "testConfig.xml"));
            serializer.Serialize(writer, folderPollXsd);
            writer.Close();

            FolderPollService folderPollService = new FolderPollService(Path.Combine(polledPath, "testConfig.xml"), true);
            folderPollService.Run();

            
        }

        private FolderPollXsd AutoCreateFolderPollXsd(string[] polledFolders, string[] copyFolders, string[] moveFolders, string[] applications, string[] filters,
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
            string[] applications = { @"C:\Windows\notepad.exe" };
            string[] filters = { ".txt" };
            string domain = "domain";
            string username = "username";
            string password = "password";

            var actualFolderPollXsd = AutoCreateFolderPollXsd(polledFolders, copyFolders, moveFolders, applications, filters,
                true, domain, username, password);

            var polls = new List<Poll>
            {
                new Poll
                {
                  Folder  =  polledFolders[0],
                  Impersonation = true,
                  Domain = domain,
                  Username = username,
                  Password = password,
                  NewFile = new NewFile
                  {
                      Filter = filters[0],
                      Copy = new NewFileCopy
                      {
                          TargetFolder = copyFolders[0]
                      }
                  }
                },
                new Poll
                {
                  Folder  =  polledFolders[0],
                  Impersonation = true,
                  Domain = domain,
                  Username = username,
                  Password = password,
                  NewFile = new NewFile
                  {
                      Filter = filters[0],
                      Copy = new NewFileCopy
                      {
                          TargetFolder = copyFolders[0]
                      },
                      Launch = new NewFileLaunch
                      {
                          Application = applications[0],
                          Arguments = "{0}"
                      }
                  }
                },
                new Poll
                {
                  Folder  =  polledFolders[0],
                  Impersonation = true,
                  Domain = domain,
                  Username = username,
                  Password = password,
                  NewFile = new NewFile
                  {
                      Filter = filters[0],
                      Copy = new NewFileCopy
                      {
                          TargetFolder = copyFolders[0]
                      },
                      Launch = new NewFileLaunch
                      {
                          Application = applications[0],
                          Arguments = "{1}"
                      }
                  }
                },
                new Poll
                {
                  Folder  =  polledFolders[0],
                  Impersonation = true,
                  Domain = domain,
                  Username = username,
                  Password = password,
                  NewFile = new NewFile
                  {
                      Filter = filters[0],
                      Copy = new NewFileCopy
                      {
                          TargetFolder = copyFolders[0]
                      },
                      Launch = new NewFileLaunch
                      {
                          Application = applications[0],
                          Arguments = "{2}"
                      }
                  }
                },
                new Poll
                {
                  Folder  =  polledFolders[0],
                  Impersonation = true,
                  Domain = domain,
                  Username = username,
                  Password = password,
                  NewFile = new NewFile
                  {
                      Filter = filters[0],
                      Copy = new NewFileCopy
                      {
                          TargetFolder = copyFolders[0]
                      },
                      Launch = new NewFileLaunch
                      {
                          Application = applications[0],
                          Arguments = Path.Combine(copyFolders[0], "{3}")
                      }
                  }
                },
                new Poll
                {
                  Folder  =  polledFolders[0],
                  Impersonation = true,
                  Domain = domain,
                  Username = username,
                  Password = password,
                  NewFile = new NewFile
                  {
                      Filter = filters[0],
                      Copy = new NewFileCopy
                      {
                          TargetFolder = copyFolders[0]
                      },
                      Launch = new NewFileLaunch
                      {
                          Application = applications[0],
                          Arguments = Path.Combine(copyFolders[0], string.Concat("{4}", filters[0]))
                      }
                  }
                },
                new Poll
                {
                      Folder  =  polledFolders[0],
                      Impersonation = true,
                      Domain = domain,
                      Username = username,
                      Password = password,
                      NewFile = new NewFile
                      {
                          Filter = filters[0],
                          Move = new NewFileMove
                          {
                              TargetFolder = moveFolders[0]
                          }
                      }
                },
                new Poll
                {
                      Folder  =  polledFolders[0],
                      Impersonation = true,
                      Domain = domain,
                      Username = username,
                      Password = password,
                      NewFile = new NewFile
                      {
                          Filter = filters[0],
                          Move = new NewFileMove
                          {
                              TargetFolder = moveFolders[0]
                          },
                          Launch = new NewFileLaunch
                          {
                              Application = applications[0],
                              Arguments = "{0}"
                          }
                      }
                },
                new Poll
                {
                      Folder  =  polledFolders[0],
                      Impersonation = true,
                      Domain = domain,
                      Username = username,
                      Password = password,
                      NewFile = new NewFile
                      {
                          Filter = filters[0],
                          Move = new NewFileMove
                          {
                              TargetFolder = moveFolders[0]
                          },
                          Launch = new NewFileLaunch
                          {
                              Application = applications[0],
                              Arguments = "{1}"
                          }
                      }
                },
                new Poll
                {
                      Folder  =  polledFolders[0],
                      Impersonation = true,
                      Domain = domain,
                      Username = username,
                      Password = password,
                      NewFile = new NewFile
                      {
                          Filter = filters[0],
                          Move = new NewFileMove
                          {
                              TargetFolder = moveFolders[0]
                          },
                          Launch = new NewFileLaunch
                          {
                              Application = applications[0],
                              Arguments = "{2}"
                          }
                      }
                },
                new Poll
                {
                      Folder  =  polledFolders[0],
                      Impersonation = true,
                      Domain = domain,
                      Username = username,
                      Password = password,
                      NewFile = new NewFile
                      {
                          Filter = filters[0],
                          Move = new NewFileMove
                          {
                              TargetFolder = moveFolders[0]
                          },
                          Launch = new NewFileLaunch
                          {
                              Application = applications[0],
                              Arguments = Path.Combine(moveFolders[0], "{3}")
                          }
                      }
                },
                new Poll
                {
                      Folder  =  polledFolders[0],
                      Impersonation = true,
                      Domain = domain,
                      Username = username,
                      Password = password,
                      NewFile = new NewFile
                      {
                          Filter = filters[0],
                          Move = new NewFileMove
                          {
                              TargetFolder = moveFolders[0]
                          },
                          Launch = new NewFileLaunch
                          {
                              Application = applications[0],
                              Arguments = Path.Combine(moveFolders[0], string.Concat("{4}", filters[0]))
                          }
                      }
                }
            };

            var expectedFolderPollXsd = new FolderPollXsd
            {
                Poll = polls.ToArray()
            };

            Assert.AreEqual(true, FolderPollXsdComparator(actualFolderPollXsd, expectedFolderPollXsd));
            
            XmlSerializer serializer = new XmlSerializer(typeof(FolderPollXsd));
            TextWriter writer = new StreamWriter(@"D:\xsd1.xml");
            serializer.Serialize(writer, actualFolderPollXsd);
            writer.Close();

            writer = new StreamWriter(@"D:\xsd2.xml");
            serializer.Serialize(writer, expectedFolderPollXsd);
            writer.Close();

            
        }

        private bool FolderPollXsdComparator(FolderPollXsd first, FolderPollXsd second)
        {
            var pollsCount = 0;
            if (first.Poll.Count() != second.Poll.Count())
            {
                return false;
            }
            else
            {
                pollsCount = first.Poll.Count();
            }

            for (int i = 0; i < pollsCount; i++)
            {
                if (first.Poll[i].Domain != second.Poll[i].Domain || first.Poll[i].Folder != second.Poll[i].Folder ||
                    first.Poll[i].Impersonation != second.Poll[i].Impersonation || 
                    first.Poll[i].ImpersonationSpecified != second.Poll[i].ImpersonationSpecified ||
                    first.Poll[i].Password != second.Poll[i].Password || first.Poll[i].Username != second.Poll[i].Username ||
                    first.Poll[i].NewFile.Copy.TargetFolder != second.Poll[i].NewFile.Copy.TargetFolder ||
                    first.Poll[i].NewFile.Move.TargetFolder != second.Poll[i].NewFile.Move.TargetFolder ||
                    first.Poll[i].NewFile.Filter != second.Poll[i].NewFile.Filter ||
                    first.Poll[i].NewFile.Launch.Application != second.Poll[i].NewFile.Launch.Application ||
                    first.Poll[i].NewFile.Launch.Arguments != second.Poll[i].NewFile.Launch.Arguments)
                {
                    return false;
                }
            }

            return true;
        }*/