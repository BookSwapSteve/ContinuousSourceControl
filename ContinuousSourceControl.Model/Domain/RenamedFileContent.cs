using System.IO;

namespace ContinuousSourceControl.Model.Domain
{
    public class RenamedFileContent : FileContent
    {
        public RenamedFileContent()
        {
            ChangeType = WatcherChangeTypes.Renamed;
        }

        public string NewFilePath { get; set; }
        public string OldFullPath { get; set; }

        public void RenamedTo(string fullPath)
        {
            NewFilePath = fullPath;
        }

        public void RenamedFrom(string oldFullPath)
        {
            OldFullPath = oldFullPath;
        }

        public override void Load(string fromFile)
        { }
    }
}