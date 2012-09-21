using System.Collections.Generic;
using ContinuousSourceControl.Model.Domain;
using ContinuousSourceControl.Model.Domain.Changes;

namespace ContinuousSourceControl.DataAccess.RavenDB.Interfaces
{
    public interface IRepository
    {
        Project LoadProject(string projectName);
        ProjectFile LoadFile(Project project, string path);
        IList<ProjectFile> LoadFiles(Project project);
        IList<FileContent> LoadFileContents(ProjectFile file);
        void Save(Project project);
        void Save(ProjectFile file);
        void Save(FileContent fileContent);
        
    }
}