using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataStorage
{
    /// <summary>
    /// Interface for data storage operations.
    /// </summary>
    public class LocalFileStorage : IDataStorage
    {
        private const int DefaultBufferSize = 4 * 1024; // 4 KB
        public int BufferSize { get; private set; }

        public LocalFileStorage(int bufferSize = DefaultBufferSize)
        {
            BufferSize = bufferSize;
        }

        /// <summary>
        /// Checks file name and file existence in the storage.
        /// </summary>
        /// <returns>True if file exists and is not empty, otherwise false.</returns>
        public bool CheckFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("File name is null or empty.");
                return false;
            }                
            if(!File.Exists(fileName))
            {
                Console.WriteLine($"File not found: {fileName}");
                return false;
            }
            if (new FileInfo(fileName).Length == 0)
            {
                Console.WriteLine($"File is empty: {fileName}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes data to a file asynchronously.
        /// </summary>
        /// <param name="fileName">File name to write to.</param>
        /// <param name="contentStream">Stream containing the data to write.</param>
        public async Task WriteAsync(string fileName, Stream contentStream)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None, BufferSize))
            {
                Console.WriteLine($"\tWrite data: Size = {contentStream.Length}");
                await contentStream.CopyToAsync(fileStream);
            }
        }

        /// <summary>
        /// Reads data from a file asynchronously.
        /// </summary>
        /// <param name="fileName">File name to read from.</param>
        public async Task<Stream> ReadAsync(string fileName)
        {
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await Task.FromResult(stream);
        }

        /// <summary>
        /// Reads multiple files and returns their streams as an enumerable synchronously.
        /// </summary>
        /// <param name="fileNames">Collection of file names to read.</param>
        /// <returns>An enumerable of streams for each file.</returns>
        public IEnumerable<Stream> ReadMultiple(IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                yield return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        /// <summary>
        /// Deletes a file from the storage.
        /// </summary>
        /// <param name="fileName">File name to delete.</param>
        public void Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("File name is null or empty.");
                return;
            }

            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                    Console.WriteLine($"File deleted: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {fileName}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"File not found: {fileName}");
            }
        }
    }
}
