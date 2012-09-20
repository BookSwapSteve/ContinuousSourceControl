using ContinuousSourceControl.BL.Interfaces;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.BL
{
    public class WatcherFactory : IWatcherFactory
    {
        public IWatcher Create(IRepository repository, Project project)
        {
            return new Watcher(repository, project);
        }
    }
}