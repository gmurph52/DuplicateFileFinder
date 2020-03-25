using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using NewsStyleUriParser = System.NewsStyleUriParser;

namespace DuplicateFileFinder
{
	public class DuplicateFileFinder : IDuplicateFileFinder
	{
		// If more than 5,000 files maybe use SortedList instead https://stackoverflow.com/questions/1089132/net-hashtable-vs-dictionary-can-the-dictionary-be-as-fast 
		private static readonly ConcurrentDictionary<string, DuplicateFileResult> FilesDictionary = new ConcurrentDictionary<string, DuplicateFileResult>();
		private static readonly Semaphore Pool = new Semaphore(10, 10);

		/**
		 * This will only work on files and directories that you have access to. If a file is in use or not available to read
		 * then it will crash.
		 */
		public Task<List<DuplicateFileResult>> FindDuplicatesAsync(string basePath)
		{
			var threads = new List<Thread>();

			//var searchOptions = new EnumerationOptions()
			//{
			//	IgnoreInaccessible = true,
			//	RecurseSubdirectories = true,
			//	AttributesToSkip = FileAttributes.System | FileAttributes.Temporary | FileAttributes.Offline
			//};
			//string[] allFiles = Directory.GetFiles(basePath, "*.*", searchOptions);
			string[] allFiles = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);

			foreach (var file in allFiles)
			{
				// Start different threads here to get through all the files faster
				var thread = new Thread(() => ProcessFileThread(file));
				threads.Add(thread);
				thread.Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

			return Task.FromResult(FilesDictionary.Values.ToList());
		}

		private static void ProcessFileThread(string file)
		{
			Pool.WaitOne();
			var base64Sha1Hash = GetBase64Sha1Hash(file);
			FileInfo info = new FileInfo(file);

			if (!FilesDictionary.ContainsKey(base64Sha1Hash))
			{
				if (!FilesDictionary.TryAdd(base64Sha1Hash, new DuplicateFileResult
				{
					Sha1HashBase64 = base64Sha1Hash,
					FileRelativePaths = new List<string> { string.Concat(info.DirectoryName, '\\', info.Name) },
					FileSize = info.Length
				}))
				{
					// If TryAddFails that means another thread already added it so we will update.
					FilesDictionary[base64Sha1Hash].FileRelativePaths.Add(string.Concat(info.DirectoryName, '\\', info.Name));
				}
			}
			else
			{
				FilesDictionary[base64Sha1Hash].FileRelativePaths.Add(string.Concat(info.DirectoryName, '\\', info.Name));
			}

			Pool.Release();
		}

		private static string GetBase64Sha1Hash(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open))
			using (BufferedStream bs = new BufferedStream(fs))
			{
				using (SHA1Managed sha1 = new SHA1Managed())
				{
					return Convert.ToBase64String(sha1.ComputeHash(bs));
				}
			}
		}
	}
}