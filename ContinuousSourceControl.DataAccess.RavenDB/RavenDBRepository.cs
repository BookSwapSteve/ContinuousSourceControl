using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.DataAccess.RavenDB
{
    public class RavenDBRepository : IRepository
    {
        public Project LoadProject(string projectName)
        {
            throw new System.NotImplementedException();
        }

        public ProjectFile LoadFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void Save(Project project)
        {
            throw new System.NotImplementedException();
        }

        public void Save(ProjectFile file)
        {
            throw new System.NotImplementedException();
        }

        public void Save(FileContent fileContent)
        {
            throw new System.NotImplementedException();
        }
    }
}