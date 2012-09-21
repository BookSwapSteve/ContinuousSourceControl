using ContinuousSourceControl.BL.Interfaces;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.BL
{
    public class WatcherFactory : IWatcherFactory
    {
        public IWatcher Create(IRepository repository, IFileChangeBL fileChangeBL, Project project)
        {
            return new Watcher(repository, fileChangeBL, project);
        }
    }
}