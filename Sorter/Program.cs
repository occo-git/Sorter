//#define benchmarks
using BenchmarkDotNet.Running;
using FileDataSorter.Benchmark;
using Sorter;
using System.Diagnostics;


#if benchmarks
var summary2 = BenchmarkRunner.Run<FileProcessingBenchmark>();
//var summary3 = BenchmarkRunner.Run<FileCheckBenchmark>();
#else

var sw = Stopwatch.StartNew();

var sorter = new MySorter();
//await sorter.Sort("d:/_data1", 1024 * 1024 * 1024); // 1 GB file
await sorter.SotrFiles("d:/multi_data", 5, 100 * 1024 * 1024); // 5 files of 100 MB each

sw.Stop();
Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

#endif