using System;

namespace ContinuousSourceControl.Model.Domain
{
    /// <summary>
    /// Represents the actual file contents of the file being versioned and the meta data associated with the file.
    /// </summary>
    public class FileContent : DomainEntityBase
    {
        /// <summary>
        /// Id of the parent file
        /// </summary>
        public int FileId { get; set; }

        public long Version { get; set; }
        public DateTime LastUpated { get; set; }
        public byte[] FileContents { get; set; }
    }
}