using System.IO;
using ContinuousSourceControl.Model.Logging;

namespace ContinuousSourceControl.Model.Domain.Changes
{
    public class StringFileContent : FileContentsChangedEvent
    {
        public string FileContents { get; set; }

        protected override bool LoadContents(string fromFile)
        {
            try
            {
                FileContents = File.ReadAllText(fromFile);
                return true;
            }
            catch (IOException exception)
            {
                Logger.Error("IOException trying to load contents from {0}. {1}", fromFile, exception);
                return false;
            }
        }
    }
}