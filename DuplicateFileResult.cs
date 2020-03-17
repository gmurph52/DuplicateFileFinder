using System.Collections.Generic;

namespace DuplicateFileFinder
{
    public class DuplicateFileResult
    {
        public string Sha1HashBase64 { get; set;  }
        public long FileSize { get; set; }
        public List<string> FileRelativePaths { get; set; }
    }
}