using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.BL.Interfaces
{
    public interface IWatcherFactory
    {
        IWatcher Create(IRepository repository, IFileChangeBL fileChangeBL, Project project);
    }
}