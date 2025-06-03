using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using FileProcessing;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataStorage;

namespace FileDataSorter.Benchmark
{
    [MemoryDiagnoser]
    [Config(typeof(FileProcessingBenchmark.Config))]
    public class FileProcessingBenchmark
    {
        IDataStorage storage = new LocalFileStorage();
        IChunkParser parser = new MyChunkParser();

        [Benchmark]
        public async Task ProcessTestFile_1()
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            string filePath = "d:/mark1GB.txt";

            IFileProcessing fileProcessing = new MyFileProcessing(storage, parser, chunkSize);
            string resultFilePath = await fileProcessing.ProcessFile(filePath);

            storage.Delete(resultFilePath);
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
