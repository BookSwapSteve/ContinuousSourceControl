using System.IO;

namespace ContinuousSourceControl.Model.Domain.Changes
{
    public class StringFileContent : FileContent
    {
        public string FileContents { get; set; }

        public override void Load(string fromFile)
        {
            FileContents = File.ReadAllText(fromFile);
        }         
    }
}