using System;
using ContinuousSourceControl.BL.Interfaces;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.BL
{
    /// <summary>
    /// Main application class for the project.
    /// </summary>
    public class Application
    {
        private readonly IRepository _repository;
        private readonly IWatcherFactory _fileWatcherFactory;

        private IWatcher _fileWatcher;

        public Application(IRepository repository, IWatcherFactory watcherFactory)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (watcherFactory == null) throw new ArgumentNullException("watcherFactory");

            _repository = repository;
            _fileWatcherFactory = watcherFactory;
        }

        public Project CreateProject(string name, string path)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (path == null) throw new ArgumentNullException("path");

            var project = new Project { Name = name, PathRoot = path };
            _repository.Save(project);
            return project;
        }

        public void Start(string projectName)
        {
            Project project = _repository.LoadProject(projectName);

            _fileWatcher = _fileWatcherFactory.Create(_repository, project);
            _fileWatcher.Start();
        }
    }
}