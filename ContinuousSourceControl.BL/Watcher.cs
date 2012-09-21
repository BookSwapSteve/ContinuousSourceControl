using System;
using System.IO;
using ContinuousSourceControl.BL.Interfaces;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;
using ContinuousSourceControl.Model.Domain.Changes;
using ContinuousSourceControl.Model.Helpers;
using ContinuousSourceControl.Model.Logging;

namespace ContinuousSourceControl.BL
{
    public class Watcher : IWatcher, IDisposable
    {
        private readonly IRepository _repository;
        private readonly IFileChangeBL _fileChangeBl;
        private readonly Project _project;
        private FileSystemWatcher _fileSystemWatcher;
        private bool _disposed = false;
        
        private FilterHelper _filterHelper;

        public Watcher(IRepository repository,IFileChangeBL fileChangeBl, Project project)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (fileChangeBl == null) throw new ArgumentNullException("fileChangeBl");
            if (project == null) throw new ArgumentNullException("project");

            _repository = repository;
            _fileChangeBl = fileChangeBl;
            _project = project;

            _fileSystemWatcher = new FileSystemWatcher(project.PathRoot)
                {
                    EnableRaisingEvents = false,
                    IncludeSubdirectories = true
                };

            _fileSystemWatcher.Changed += fileSystemWatcher_Changed;
            _fileSystemWatcher.Created += fileSystemWatcher_Created;
            _fileSystemWatcher.Deleted += fileSystemWatcher_Deleted;
            _fileSystemWatcher.Error += fileSystemWatcher_Error;
            _fileSystemWatcher.Renamed += fileSystemWatcher_Renamed;
        }

        public void Start()
        {
            // TODO: Load up the filters from .gitIgnore file
            LoadFilters();
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void LoadFilters()
        {
            _filterHelper = new FilterHelper();
            _filterHelper.Initialize(_project.PathRoot);
        }

        #region Event Handlers

        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logger.Info("Changed {0} - {1}", e.FullPath, e.ChangeType);

            UpdateFile(e);
        }

        private void UpdateFile(FileSystemEventArgs e)
        {
            if (!_filterHelper.IsMatch(e.FullPath))
            {
                ProjectFile file = LoadOrCreateFile(e.FullPath);
                FileContent fileContent = file.Change(e.ChangeType);

                _repository.Save(file);
                _repository.Save(fileContent);
            }
        }

        void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Logger.Info("Created {0}", e.FullPath);

            UpdateFile(e);
        }

        void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Logger.Info("Deleted {0}", e.FullPath);

            UpdateFile(e);
        }

        void fileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            Logger.Error("Error: {0}", e.GetException().ToString());
        }

        void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Logger.Info("Renamed {0} to {1}", e.OldFullPath, e.FullPath);
            // TODO: Handle folder name change, all files under it neeed to be updated.

            // Mark the old file as renamed.
            // Start a new file entry, at the version of the old one.
            // Point the two files at each other.

            if (!_filterHelper.IsMatch(e.OldFullPath))
            {
                ProjectFile oldFile = LoadOrCreateFile(e.OldFullPath);
                ProjectFile newFile = LoadOrCreateFile(e.FullPath);
                RenamedFileContent oldFileContent = oldFile.RenamedTo(e.FullPath);
                RenamedFileContent newFileContent = newFile.RenamedFrom(e.OldFullPath);

                _repository.Save(oldFile);
                _repository.Save(newFile);
                _repository.Save(oldFileContent);
                _repository.Save(newFileContent);
            }
        }

        #endregion

        private ProjectFile LoadOrCreateFile(string path)
        {
            return _repository.LoadFile(_project, path) ?? _project.CreateFile(path);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_fileSystemWatcher != null)
                {
                    try
                    {
                        _fileSystemWatcher.Changed -= fileSystemWatcher_Changed;
                        _fileSystemWatcher.Created -= fileSystemWatcher_Created;
                        _fileSystemWatcher.Deleted -= fileSystemWatcher_Deleted;
                        _fileSystemWatcher.Error -= fileSystemWatcher_Error;
                        _fileSystemWatcher.Renamed -= fileSystemWatcher_Renamed;
                        _fileSystemWatcher.Dispose();
                    }
                    finally
                    {
                        _fileSystemWatcher = null;
                        _disposed = true;
                    }
                }
            }
        }

        #endregion
    }
}