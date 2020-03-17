using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuplicateFileFinder
{
    public interface IDuplicateFileFinder
    {
        public Task<List<DuplicateFileResult>> FindDuplicatesAsync(string basePath);
    }
}