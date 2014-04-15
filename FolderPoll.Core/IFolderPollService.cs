namespace FolderPoll.Core
{
    public interface IFolderPollService
    {
        void Init(string configurationFilePathOrString, bool isFile);
        void Run();
        void Stop();
    }
}
