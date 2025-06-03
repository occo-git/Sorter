using FileProcessing.DataModels;
using FileProcessing.DataProcessing;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataStorage;
using System;
using System.Data;

namespace FileProcessing
{
    /// <summary>
    /// Processes a file
    /// <para> 1. Reads file in chunks of records, where each record is a data model with a Hash.</para>
    /// <para> 2. Orders records within each chunk by Hash</para>
    /// <para> 3. Saves each ordered chunk to a temporary file.</para>
    /// <para> 4. Merges all temporary files into a single result file.</para>
    /// </summary>
    public class MyFileProcessing : IFileProcessing
    {
        private readonly IDataStorage _dataStorage;
        private readonly IChunkProcessing _chunkProcessing;
        private readonly IDataStorage _tempDataStorage;

        public MyFileProcessing(IDataStorage dataStorage, IChunkParser chunkParser, int chunkSize)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
            if (chunkParser == null)
                throw new ArgumentNullException(nameof(chunkParser));

            _chunkProcessing = new MyChunkProcessing(chunkParser, chunkSize);
            _tempDataStorage = new LocalFileStorage();
        }

        public async Task<string> ProcessFile(string fileName)
        {
            if (!_dataStorage.CheckFile(fileName))
            {
                Console.WriteLine("No files created.");
                return string.Empty;
            }
            Console.WriteLine($"Processing file: {fileName}");
            List<string> chunkTempFileNames = new List<string>();

            // 1. Read file in chunks and process each chunk
            using (var stream = await _dataStorage.ReadAsync(fileName))
            {
                await foreach (var chunk in _chunkProcessing.GetChunks(stream))
                {
                    string chunkFileName = await ProcessChunk(chunk);
                    if (!string.IsNullOrEmpty(chunkFileName))
                        chunkTempFileNames.Add(chunkFileName);
                }
            }

            string resultFileName = GetResultFileName(fileName);
            await MergeFiles(resultFileName, chunkTempFileNames);
            return resultFileName;
        }

        /// <summary>
        /// Processes a chunk of data by ordering it and saving it to a temporary file.
        /// </summary>
        /// <param name="chunk">Chunk of data to process.</param>
        /// <returns>Path to the temporary file containing the ordered chunk data.</returns>
        private async Task<string> ProcessChunk(IChunkData<IDataModel> chunk)
        {
            if (chunk.Data == null || chunk.Data.Count == 0)
            {
                Console.WriteLine("Empty chunk data.");
                return string.Empty;
            }

            // 2. Orders records within each chunk by Hash.
            var result = chunk.Data.OrderBy(l => l.Hash).ToList();
            Console.WriteLine($"\tOrdered chunk data by Hash: Lines = {result.Count}");

            using (var combinedStream = new MemoryStream())
            {
                foreach (var record in result)
                    combinedStream.Write(record.Bytes, 0, record.Bytes.Length);

                // 3.Saves each ordered chunk to a temporary file.
                combinedStream.Position = 0;
                return await SaveChunkTempFile(chunk.Id, combinedStream);
            }
        }

        /// <summary>
        /// Saves a chunk of data to a temporary file.
        /// </summary>
        /// <param name="chunkId">Unique identifier for the chunk.</param>
        /// <param name="contentStream">Stream containing the chunk data.</param>
        /// <returns>Path to the temporary file where the chunk data is saved.</returns>
        private async Task<string> SaveChunkTempFile(Guid chunkId, Stream contentStream)
        {
            if (contentStream == null || contentStream.Length == 0)
            {
                Console.WriteLine("Empty content stream.");
                return string.Empty;
            }
            string tempDir = Path.GetTempPath();
            string tempFileName = Path.Combine(tempDir, $"chunk_{chunkId:N}.tmp");
            await _tempDataStorage.WriteAsync(tempFileName, contentStream);
            Console.WriteLine($"\tTemp file saved: {tempFileName}");
            return tempFileName;
        }

        /// <summary>
        /// Merges multiple temporary files into a single result file.
        /// </summary>
        /// <param name="resultFileName">Name of the resulting merged file.</param>
        /// <param name="fileNames">List of temporary file names to merge.</param>
        private async Task MergeFiles(string resultFileName, List<string> fileNames)
        {
            if (fileNames == null || fileNames.Count == 0)
            {
                Console.WriteLine("No files created.");
                return;
            }

            if (fileNames.Count == 1)
            {
                File.Copy(fileNames[0], resultFileName, true); // FIX: use _dataStorage.WriteAsync instead of File.Copy
                Console.WriteLine($">> File created: {resultFileName}");
            }
            else
            {
                Console.WriteLine("Merging files");

                // 4. Merges all temporary files into a single result file.
                var streams = _tempDataStorage.ReadMultiple(fileNames);
                await foreach (var chunkStream in _chunkProcessing.MergeStreams(streams))
                {
                    await _dataStorage.WriteAsync(resultFileName, chunkStream);
                }
                foreach (var stream in streams)
                    stream.Dispose();

                Console.WriteLine($">> File created: {resultFileName}");
            }
            DeleteTempFiles(fileNames);
        }

        /// <summary>
        /// Generates a unique file name for the result file based on the original file name.
        /// </summary>
        /// <param name="fileName">Original file name to base the result file name on.</param>
        /// <returns>Unique file name for the result file.</returns>
        private string GetResultFileName(string fileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string directory = Path.GetDirectoryName(fileName) ?? string.Empty;
            Guid id = Guid.NewGuid();
            return Path.Combine(directory, $"{fileNameWithoutExtension}_sorted_{id:N}.txt");
        }

        /// <summary>
        /// Deletes temporary files.
        /// </summary>
        /// <param name="fileNames">List of temporary file names to delete.</param>
        private void DeleteTempFiles(List<string> fileNames)
        {
            Console.WriteLine("Deleting temp files");
            foreach (var fileName in fileNames)
                _tempDataStorage.Delete(fileName);
        }
    }
}
