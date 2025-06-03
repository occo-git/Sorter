using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using FileProcessing;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataStorage;

namespace FileDataSorter.Benchmark
{
    [MemoryDiagnoser]
    [Config(typeof(FileCheckBenchmark.Config))]
    public class FileCheckBenchmark
    {
        IDataStorage storage = new LocalFileStorage();
        IChunkParser parser = new MyChunkParser();

        [Benchmark]
        public async Task CheckTestFile_1()
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            string filePath = "d:/mark1GB_sorted.txt";

            IFileCheck fileCheck = new MyFileCheck(storage, parser, chunkSize);
            await fileCheck.CheckData(filePath);
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
