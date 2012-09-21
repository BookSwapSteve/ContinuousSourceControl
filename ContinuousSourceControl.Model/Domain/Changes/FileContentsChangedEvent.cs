using System.Threading;

namespace ContinuousSourceControl.Model.Domain.Changes
{
    public abstract class FileContentsChangedEvent : FileContent
    {
        public override void Load(string fromFile)
        {
            for (int i = 0; i < 2; i++)
            {
                if (LoadContents(fromFile))
                {
                    return;
                }

                // If their was an io exception
                // sleep for a bit as this may help
                // the file being exclusivly locked.
                Thread.Sleep(250);
            }
        }

        protected abstract bool LoadContents(string fromFile);
    }
}