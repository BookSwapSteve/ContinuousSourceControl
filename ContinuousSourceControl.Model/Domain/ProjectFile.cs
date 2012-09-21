using System;
using System.IO;
using ContinuousSourceControl.Model.Domain.Changes;

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
        /// <param name="changeType"> </param>
        /// <returns>Returns a versioned FileContent object that should then be stored in the database.</returns>
        public FileContent Change(WatcherChangeTypes changeType)
        {
            FileContent fileContent;
            switch (changeType)
            {
                case WatcherChangeTypes.Deleted:
                    fileContent = new DeletedFileContent();
                    break;
                case WatcherChangeTypes.Renamed:
                    throw new NotSupportedException("Please use RenamedTo and RenamedFrom");
                default:
                    fileContent = GetChangedFileContent();
                    break;
            }

            UpdateVersionNumber(fileContent);
            fileContent.ChangeType = changeType;
            fileContent.FileId = Id;

            return fileContent;
        }

        /// <summary>
        /// File has been renamed to a new file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public RenamedFileContent RenamedTo(string fullPath)
        {
            var fileContent = new RenamedFileContent();
            fileContent.RenamedTo(fullPath);
            UpdateVersionNumber(fileContent);
            fileContent.FileId = Id;
            return fileContent;
        }

        /// <summary>
        /// File has come from a previous file.
        /// </summary>
        /// <param name="oldFullPath"></param>
        /// <returns></returns>
        public RenamedFileContent RenamedFrom(string oldFullPath)
        {
            var fileContent = new RenamedFileContent();
            fileContent.RenamedFrom(oldFullPath);
            UpdateVersionNumber(fileContent);
            fileContent.FileId = Id;
            return fileContent;
        }

        private void UpdateVersionNumber(FileContent fileContent)
        {
            CurrentVersionNumber++;
            fileContent.Version = CurrentVersionNumber;
        }

        private FileContent GetChangedFileContent()
        {
            if (File.Exists(FilePath))
            {
                var binaryFileContent = new BinaryFileContent();
                binaryFileContent.Load(FilePath);
                int fileSize = binaryFileContent.FileContents.Length;

                FileContent fileContent;
                if (binaryFileContent.IsTextFile())
                {
                    fileContent = new StringFileContent();
                    fileContent.Load(FilePath);
                }
                else
                {
                    fileContent = binaryFileContent;
                }

                fileContent.LastWriteTime = File.GetLastWriteTime(FilePath);
                fileContent.FileSize = fileSize;
                return fileContent;
            }
            return new DirectoryContent();
        }
    }
}