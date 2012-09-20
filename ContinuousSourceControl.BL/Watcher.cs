using System;
using System.Collections.Generic;
using System.IO;
using ContinuousSourceControl.BL.Interfaces;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;
using ContinuousSourceControl.Model.Logging;

namespace ContinuousSourceControl.BL
{
    public class Watcher : IWatcher, IDisposable
    {
        private const string GitIgnoreFileName = ".gitIgnore";

        private readonly IRepository _repository;
        private readonly Project _project;
        private FileSystemWatcher _fileSystemWatcher;
        private bool _disposed = false;
        readonly List<string> _filters = new List<string>();

        public Watcher(IRepository repository, Project project)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (project == null) throw new ArgumentNullException("project");

            _repository = repository;
            _project = project;

            _fileSystemWatcher = new FileSystemWatcher(project.PathRoot) { EnableRaisingEvents = false };
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
            string filterFile = Path.Combine(_project.PathRoot, GitIgnoreFileName);
            if (File.Exists(filterFile))
            {
                Logger.Info("Using filter file {0}", _project.PathRoot);
                using (var stream = File.OpenRead(filterFile))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        _filters.Add(reader.ReadLine());
                    }
                }
            }
            else
            {
                Logger.Warn("No filter file found in {0}", _project.PathRoot);
            }
        }

        #region Event Handlers

        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logger.Info("Changed {0} - {1}", e.FullPath, e.ChangeType);

            ProjectFile file = LoadOrCreateFile(e.FullPath);

            _repository.Save(file);
        }

        void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Logger.Info("Created {0}", e.FullPath);
        }

        void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Logger.Info("Deleted {0}", e.FullPath);
        }

        void fileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            Logger.Error("Error: {0}", e.GetException().ToString());
        }

        void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Logger.Info("Renamed {0} to {1}", e.OldFullPath, e.FullPath);
            // Mark the old file as renamed.
            // Start a new file entry, at the version of the old one.
            // Point the two files at each other.
        }

        #endregion

        private ProjectFile LoadOrCreateFile(string path)
        {
            var file = _repository.LoadFile(path);

            if (file == null)
            {
                file = _project.CreateFile(path);
            }

            return file;
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