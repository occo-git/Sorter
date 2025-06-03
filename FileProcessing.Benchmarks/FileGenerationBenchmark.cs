using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using FileProcessing;
using FileProcessing.DataGeneration.Generators;
using FileProcessing.DataStorage;


namespace FileDataSorter.Benchmark
{
    [MemoryDiagnoser]
    [Config(typeof(FileGenerationBenchmark.Config))]
    public class FileGenerationBenchmark
    {
        IDataStorage storage = new LocalFileStorage();
        IChunkGenerator generator = new MyChunkGenerator();

        [Benchmark]
        public async Task GenerateteTestFile_1()
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            string filePath = "d:/mark1GB_generated.txt";
            int fileSize = 1024 * 1024 * 1024; // 1 GB

            IFileGeneration fileGeneration = new MyFileGeneration(storage, generator, chunkSize);
            await fileGeneration.Generate(filePath, fileSize);

            storage.Delete(filePath);
        }

        public class Config : ManualConfig
        {
            public Config()
            {
                AddJob(Job.Default.WithWarmupCount(2).WithIterationCount(5));
            }
        }
    }
}
