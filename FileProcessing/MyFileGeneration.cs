using FileProcessing.DataGeneration;
using FileProcessing.DataGeneration.Generators;
using FileProcessing.DataStorage;

namespace FileProcessing
{
    /// <summary>
    /// Generates a file of specified size by creating chunks of data and writing them to the file.
    /// </summary>
    public class MyFileGeneration : IFileGeneration
    {
        private readonly IDataStorage _dataStorage;
        private readonly IChunkGeneration _chunkGeneration;

        /// <summary>
        /// Generates a file of specified size by creating chunks of data and writing them to the file.
        /// </summary>
        /// <param name="dataStorage">Storage for writing the generated file.</param>
        /// <param name="chunkGenerator">Generator for creating chunks of data.</param>
        /// <param name="chunkSize">Size of each chunk to be generated.</param>
        public MyFileGeneration(IDataStorage dataStorage, IChunkGenerator chunkGenerator, int chunkSize)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
            if (chunkGenerator == null)
                throw new ArgumentNullException(nameof(chunkGenerator));
            _chunkGeneration = new ChunkGeneration(chunkGenerator, chunkSize);
        }

        public async Task Generate(string fileName, long fileSize)
        {
            if (fileSize <= 0)
                throw new ArgumentException("File size must be greater than zero.", nameof(fileSize));
            
            _dataStorage.Delete(fileName);
            Console.WriteLine($"Generating file: {fileName}");

            // Generate chunks of data and write them to the file
            await foreach (var bytes in _chunkGeneration.GenerateChunks(fileSize))
            {
                using (var ms = new MemoryStream(bytes))
                {
                    await _dataStorage.WriteAsync(fileName, ms);
                }
            }
        }
    }
}
