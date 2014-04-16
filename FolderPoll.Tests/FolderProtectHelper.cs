using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace FolderPoll.Tests.Helpers
{
    public static class FolderProtectHelper
    {
        #region Public methods

        public static void SetupSecurityGroupAndUser(PrincipalContext context, string groupName, string groupDescription, string userName, string userPassword, string userDescription)
        {
            var group = GetOrCreateGroup(context, groupName, groupDescription);

            var accountAlreadyExists = group.Members.FirstOrDefault(x => x.Name == groupName) != null;

            if (!accountAlreadyExists)
            {
                // Add account
                var userAccount = CreateNewUser(context, userName, userPassword, userDescription);

                if (userAccount != null) group.Members.Add(userAccount);

                group.Save();
            }
        }

        public static void ProtectFolder(string folderToProtect, string accountFullAccessAllowed, string accountDenied, bool allowDeniedToList)
        {
            EnsureFolderExists(folderToProtect);

            var directoryInfo = new DirectoryInfo(folderToProtect);

            // Get a DirectorySecurity object that represents the current security settings.
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            // Apply the account has FULL access to this folder
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(accountFullAccessAllowed,
                                         fileSystemRights: FileSystemRights.FullControl,
                                         inheritanceFlags:
                                             InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                         propagationFlags: PropagationFlags.None,
                                         type: AccessControlType.Allow)
                );

            if (!string.IsNullOrEmpty(accountDenied))
            {
                // Other users can only list files
                if (allowDeniedToList)
                {
                    directorySecurity.AddAccessRule(
                        new FileSystemAccessRule(accountDenied, // Instead of just AccountUserNameWizUser
                                                 fileSystemRights: FileSystemRights.ListDirectory,
                                                 inheritanceFlags:
                                                     InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                 propagationFlags: PropagationFlags.None,
                                                 type: AccessControlType.Allow)
                        );
                }

                directorySecurity.AddAccessRule(
                    new FileSystemAccessRule(accountDenied,
                                             fileSystemRights:
                                                 FileSystemRights.Delete |
                                                 FileSystemRights.CreateFiles |
                                                 FileSystemRights.CreateDirectories |
                                                 FileSystemRights.AppendData |
                                                 FileSystemRights.ReadExtendedAttributes |
                                                 FileSystemRights.WriteExtendedAttributes |
                                                 FileSystemRights.ExecuteFile |
                    //                                         FileSystemRights.Traverse |
                                                 FileSystemRights.DeleteSubdirectoriesAndFiles |
                                                 FileSystemRights.WriteAttributes |
                                                 FileSystemRights.ChangePermissions |
                                                 FileSystemRights.TakeOwnership,

                                             inheritanceFlags:
                                                 InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                             propagationFlags: PropagationFlags.None,
                                             type: AccessControlType.Deny)
                    );
            }


            // Set the new access settings.
            directoryInfo.SetAccessControl(directorySecurity);
        }

        #endregion

        #region Private metods

        private static GroupPrincipal GetOrCreateGroup(PrincipalContext context, string groupName, string description)
        {
            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);

            if (@group == null)
            {
                @group = new GroupPrincipal(context, groupName)
                {
                    // This setting does not work on earlier versions of .NET so it has been removed here
                    //                                Description = description
                };
            }
            return @group;
        }

        private static UserPrincipal CreateNewUser(PrincipalContext context, string username, string password, string description)
        {
            UserPrincipal existingUser = UserPrincipal.FindByIdentity(context,
                                                       IdentityType.SamAccountName,
                                                       username);

            if (existingUser != null)
            {
                return null;
            }

            var user = new UserPrincipal(context)
            {
                Name = username,
                DisplayName = username,
                Description = description,
                UserCannotChangePassword = true,
                PasswordNeverExpires = true
            };
            user.SetPassword(password);
            user.Save();
            return user;
        }

        private static void EnsureFolderExists(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        #endregion
    }
}
