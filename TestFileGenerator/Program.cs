//#define benchmarks
using BenchmarkDotNet.Running;
using FileDataSorter.Benchmark;
using System.Diagnostics;
using TestFileGenerator;


#if benchmarks
var summary1 = BenchmarkRunner.Run<FileGenerationBenchmark>();
#else

var sw = Stopwatch.StartNew();

var generator = new MyTestFileGenerator();
await generator.Generate("d:/_data1", 1024 * 1024 * 1024); // 1 GB file
//await generator.GenerateFiles("d:/multi_data", 5, 100 * 1024 * 1024); // 5 files of 100 MB each

sw.Stop();
Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

#endif