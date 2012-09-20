namespace ContinuousSourceControl.Model.Domain
{
    /// <summary>
    /// Represents a project to monitor
    /// </summary>
    public class Project : DomainEntityBase
    {
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Top level project folder.
        /// </summary>
        public string PathRoot { get; set; }

        /// <summary>
        /// If the project should be enabled
        /// </summary>
        public bool Enabled { get; set; }

        public ProjectFile CreateFile(string path)
        {
            var file = new ProjectFile
                {
                    ProjectId = Id,
                    FileName = System.IO.Path.GetFileName(path),
                    FilePath = path,
                };
            return file;
        }
    }
}