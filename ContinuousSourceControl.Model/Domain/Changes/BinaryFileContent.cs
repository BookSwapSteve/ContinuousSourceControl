using System.IO;
using System.Linq;
using ContinuousSourceControl.Model.Logging;

namespace ContinuousSourceControl.Model.Domain.Changes
{
    public class BinaryFileContent : FileContentsChangedEvent
    {
        public byte[] FileContents { get; set; }

        protected override bool LoadContents(string fromFile)
        {
            try
            {
                FileContents = File.ReadAllBytes(fromFile);
                return true;
            }
            catch (IOException exception)
            {
                Logger.Error("IOException trying to load contents from {0}. {1}", fromFile, exception);
                return false;
            }
        }

        public bool IsTextFile()
        {
            int nullCount = 0;

            // Inspect the first 1000 bytes and see if the file contents has
            // a large number of 0's which may mean it's a binary file.
            foreach (var b in FileContents.Take(1000))
            {
                if (b == 0)
                {
                    nullCount++;

                    if (nullCount > 20)
                    {
                        return false;
                    }
                }
            }
            // May still be a binary file!
            return true;
        }
    }
}