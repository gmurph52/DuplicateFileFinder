using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuplicateFileFinder
{
    public class DuplicateFileFinder : IDuplicateFileFinder
    {
        public Task<List<DuplicateFileResult>> FindDuplicatesAsync(string basePath)
        {
            return Task.FromResult(new List<DuplicateFileResult>());
        }
    }
}