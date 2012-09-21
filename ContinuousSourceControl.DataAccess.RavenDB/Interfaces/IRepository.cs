using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.DataAccess.RavenDB.Interfaces
{
    public interface IRepository
    {
        Project LoadProject(string projectName);
        ProjectFile LoadFile(Project project, string path);
        void Save(Project project);
        void Save(ProjectFile file);
        void Save(FileContent fileContent);
    }
}