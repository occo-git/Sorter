using FileProcessing.DataModels;
using FileProcessing.DataProcessing;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataStorage;
using System;

namespace FileProcessing
{
    /// <summary>
    /// Checks a file.
    /// </summary>
    public class MyFileCheck : IFileCheck
    {
        private readonly IDataStorage _dataStorage;
        private readonly IChunkProcessing _chunkProcessing;

        public MyFileCheck(IDataStorage dataStorage, IChunkParser chunkParser, int chunkSize)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
            if (chunkParser == null)
                throw new ArgumentNullException(nameof(chunkParser));
            _chunkProcessing = new MyChunkProcessing(chunkParser, chunkSize);
        }

        public async Task CheckData(string fileName)
        {
            if (!_dataStorage.CheckFile(fileName))
            {
                Console.WriteLine("No file checked.");
                return;
            }
            
            Console.WriteLine($"Cheking file data: {fileName}");

            using (var stream = await _dataStorage.ReadAsync(fileName)) 
            {
                await foreach (var chunk in _chunkProcessing.GetChunks(stream))
                {
                    await CheckData(chunk);
                }
            }
        }

        /// <summary>
        /// Checks the data in a chunk for order and integrity.
        /// </summary>
        /// <param name="chunk">Chunk of data to check.</param>
        /// <returns>Task with the number of records checked.</returns>
        private Task<int> CheckData(IChunkData<IDataModel> chunk)
        {
            return Task.Run(() =>
            {
                int recordCount = chunk.Data.Count;
                for (int i = 0; i < chunk.Data.Count - 1; i++)
                {
                    var record = chunk.Data[i];
                    var nextRecord = chunk.Data[i + 1];

                    if (string.Compare(record.Hash, nextRecord.Hash) > 0)
                        Console.WriteLine($"\tOut of order record [{i}]: Hash = {record.Hash}");
                }
                return recordCount;
            });
        }
    }
}
