using System;
using System.Collections.Generic;
using System.Linq;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;
using ContinuousSourceControl.Model.Domain.Changes;

namespace ContinuousSourceControl.DataAccess.RavenDB
{
    /// <summary>
    /// Store the version history in memory.
    /// </summary>
    public class MemoryRepository : IRepository
    {
        private readonly IList<Project> _projects = new List<Project>();
        private readonly IDictionary<string, ProjectFile> _files = new Dictionary<string, ProjectFile>();
        private readonly IList<FileContent> _fileContents = new List<FileContent>();

        public Project LoadProject(string projectName)
        {
            return _projects.FirstOrDefault(x => x.Name.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
        }

        public ProjectFile LoadFile(Project project, string path)
        {
            string key = CreateProjectFileKey(project.Id, path);
            if (_files.ContainsKey(key))
            {
                return _files[key];
            }
            return null;
        }

        public IList<ProjectFile> LoadFiles(Project project)
        {
            return _files.Where(x => x.Value.ProjectId == project.Id).Select(x=>x.Value).ToList();
        }

        public IList<FileContent> LoadFileContents(ProjectFile file)
        {
            return _fileContents.Where(x => x.FileId == file.Id).ToList();
        }

        public void Save(Project project)
        {
            project.Id = _projects.Count + 1;
            _projects.Add(project);
        }

        public void Save(ProjectFile file)
        {
            string key = CreateProjectFileKey(file.ProjectId, file.FilePath);
            if (_files.ContainsKey(key))
            {
                // Update the existing file.
                _files[key] = file;
            }
            else
            {
                file.Id = _files.Count + 1;
                _files.Add(key, file);
            }
        }

        public void Save(FileContent fileContent)
        {
            fileContent.Id = _fileContents.Count + 1;
            _fileContents.Add(fileContent);
        }

        private static string CreateProjectFileKey(int projectId, string path)
        {
            return string.Format("{0}-{1}", projectId, path);
        }
    }
}