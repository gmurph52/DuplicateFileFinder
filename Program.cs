using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DuplicateFileFinder
{
    class Program
    {
        static async Task Main()
        {
            // TODO: While testing, point to some sample data
            const string basePath = "c:\\git\\";

            IDuplicateFileFinder implementation = new DuplicateFileFinder();  

            var sw = Stopwatch.StartNew();
            var dupes = await implementation.FindDuplicatesAsync(basePath);
            sw.Stop();

            PrintResults(dupes, sw.Elapsed);
        }

        static void PrintResults(List<DuplicateFileResult> dupes, TimeSpan elapsedTime)
        {
            foreach (var dupe in dupes.OrderBy(d => d.Sha1HashBase64))
            {
                Console.WriteLine($"{dupe.Sha1HashBase64}: ({FormatSpace(dupe.FileSize)})");
                foreach (var file in dupe.FileRelativePaths.OrderBy(f => f))
                {
                    Console.WriteLine($"  {file}");
                }

                Console.WriteLine();
            }

            var wastedSpace = dupes.Sum(d => d.FileSize * d.FileRelativePaths.Count - 1);
            var redundantFileCount = dupes.Sum(d => d.FileRelativePaths.Count - 1);

            Console.WriteLine($"Wasted Space: {FormatSpace(wastedSpace)} in {redundantFileCount:N0} redundant files");
            Console.WriteLine($"Completed in {elapsedTime}");
        }

        static string FormatSpace(long amount)
        {
            if (amount == 0)
            {
                return "0 B";
            }

            string result;
            string suffix;
            if (amount >= 1_099_511_627_776)  
            {
                result = $"{(amount / 1_099_511_627_776.0):N2}";
                suffix = "TB";
            } 
            else if (amount >= 1_073_741_824)  
            {
                result = $"{(amount / 1_073_741_824.0):N2}";
                suffix = "GB";
            } 
            else if (amount >= 1_048_576)  
            {
                result = $"{(amount / 1_048_576.0):N2}";
                suffix = "MB";
            }
            else if (amount >= 1_024)
            {
                result = $"{(amount / 1_024.0):N2}";
                suffix = "KB";
            }
            else
            {
                result = $"{amount}";
                suffix = "B";
            }

            return $"{result.TrimEnd('0').TrimEnd('.')} {suffix}";
        }
    }
}
