using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ContinuousSourceControl.Model.Logging;

namespace ContinuousSourceControl.Model.Helpers
{
    public class FilterHelper : IFileFilter
    {
        private const string GitIgnoreFileName = ".gitIgnore";
        private const string GitFolder = ".git";
        readonly List<string> _filters = new List<string>();

        public void Initialize(string path)
        {
            // By default ignore the .git folder.
            _filters.Add(GitFolder);

            string filterFile = Path.Combine(path, GitIgnoreFileName);
            if (!File.Exists(filterFile))
            {
                Logger.Warn("No filter file found in {0}", path);
                return;
            }

            Logger.Info("Using filter file {0}", filterFile);
            _filters.AddRange(File.ReadAllLines(filterFile));
        }

        public bool IsMatch(string filePathAndName)
        {
            string path = Path.GetDirectoryName(filePathAndName);
            string fileName = Path.GetFileName(filePathAndName);
            string fileExtension = Path.GetExtension(filePathAndName);

            foreach (var filter in _filters)
            {
                if (IsMatch(filter, path, fileName, fileExtension))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(string filter, string path, string fileName, string fileExtension)
        {
            if (IsPathMatch(filter, path)) return true;

            if (IsFileNameMatch(filter, fileName)) return true;

            if (IsFileExtensionMatch(filter, fileExtension)) return true;

            return false;
        }

        private static bool IsPathMatch(string filter, string path)
        {
            // TODO: Split up the directory's and check 
            // each one, rather than looking for a simple contains.
            if (path.Contains(filter))
            {
                Logger.Info("Ignoring file because path {0} matches filter {1}", path, filter);
                return true;
            }
            return false;
        }

        private static bool IsFileNameMatch(string filter, string fileName)
        {
            // Ignore for now as illegal characters may be present in the string.
            ////// TODO: Cache this.
            ////if (filter.Contains("*"))
            ////{
            ////    Regex regex = FindFilesPatternToRegex.Convert(filter);
            ////    if (regex.IsMatch(fileName))
            ////    {
            ////        Logger.Info("Ignoring file because filename {0} matches filter {1}", fileName, filter);
            ////        return true;
            ////    }
            ////} 
            ////else
            ////{
                if (fileName.ToLower().Contains(filter.ToLower()))
                {
                    Logger.Info("Ignoring file because filename {0} contains filter {1}", fileName, filter);
                    return true;
                }
            ////}

            return false;
        }

        private static bool IsFileExtensionMatch(string filter, string fileExtension)
        {
            if (fileExtension.Contains(filter))
            {
                Logger.Info("Ignoring file because file extension {0} matches filter {1}", fileExtension, filter);
                return true;
            }
            return false;
        }
    }
}