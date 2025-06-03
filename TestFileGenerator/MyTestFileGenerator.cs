using FileProcessing;
using FileProcessing.DataStorage;
using FileProcessing.DataGeneration.Generators;

namespace TestFileGenerator
{
    public class MyTestFileGenerator
    {
        public async Task Generate(string fileName, long fileSize) 
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            IDataStorage storage = new LocalFileStorage();

            IChunkGenerator generator = new MyChunkGenerator();
            IFileGeneration fileGeneration = new MyFileGeneration(storage, generator, chunkSize);

            await fileGeneration.Generate($"{fileName}.txt", fileSize);
        }

        public async Task GenerateFiles(string fileName, int count, long fileSize)
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            IDataStorage storage = new LocalFileStorage();

            var tasks = new List<Task>();
            for (int i = 0; i < count; i++)
            {
                IChunkGenerator generator = new MyChunkGenerator();
                IFileGeneration fileGeneration = new MyFileGeneration(storage, generator, chunkSize);

                tasks.Add(fileGeneration.Generate($"{fileName}_{i + 1}.txt", fileSize)); // 1 GB
            }
            await Task.WhenAll(tasks);
        }
    }
}
