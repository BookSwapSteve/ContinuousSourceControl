namespace ContinuousSourceControl.Model.Domain
{
    /// <summary>
    /// Represents a file that is being monitored and store version history of
    /// </summary>
    public class ProjectFile : DomainEntityBase
    {
        /// <summary>
        /// Filename of the project
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Path of the file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Project this file belongs to.
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// The version number of the last saved version.
        /// </summary>
        /// <remarks>
        /// Every time a file is saved this version number is incremented and 
        /// stored in the meta data for the file
        /// </remarks>
        public long CurrentVersionNumber { get; protected set; }

        /// <summary>
        /// Push a copy of the file into 
        /// </summary>
        /// <returns>Returns a versioned FileContent object that should then be stored in the database.</returns>
        public FileContent Create()
        {
            CurrentVersionNumber++;
            var fileContent = new FileContent {Version = CurrentVersionNumber};
            // TODO: Get the contents of the file and store it in the FileContent object
            return fileContent;
        }
    }
}