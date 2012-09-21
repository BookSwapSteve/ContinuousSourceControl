using System.IO;
using System.Linq;

namespace ContinuousSourceControl.Model.Domain
{
    public class BinaryFileContent : FileContent
    {
        public byte[] FileContents { get; set; }

        public override void Load(string fromFile)
        {
            FileContents = File.ReadAllBytes(fromFile);
        }

        public bool IsTextFile ()
        {
            int nullCount = 0;

            // Inspect the first 1000 bytes and see if the file contents has
            // a large number of 0's which may mean it's a binary file.
            foreach (var b in FileContents.Take(1000))
            {
                if (b == 0)
                {
                    nullCount ++;

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