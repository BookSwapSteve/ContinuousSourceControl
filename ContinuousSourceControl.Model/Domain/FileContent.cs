using System;
using System.IO;

namespace ContinuousSourceControl.Model.Domain
{
    /// <summary>
    /// Represents the actual file contents of the file being versioned and the meta data associated with the file.
    /// </summary>
    public abstract class FileContent : DomainEntityBase
    {
        /// <summary>
        /// Id of the parent file
        /// </summary>
        public int FileId { get; set; }

        public long Version { get; set; }
        public WatcherChangeTypes ChangeType { get; set; }
        public long FileSize { get; set; }

        // TODO: Make a specific FileContent type for this?


        public abstract void Load(string fromFile);

        public DateTime LastWriteTime { get; set; }

 

        


        
    }
}