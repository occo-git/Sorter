//#define benchmarks

//#define generation
#define process

//#define multi_generation
//#define multi_process


using BenchmarkDotNet.Running;
using FileDataSorter.Benchmark;
using FileProcessing;
using FileProcessing.DataGeneration.Generators;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataStorage;
using System.Diagnostics;


#if benchmarks

var summary1 = BenchmarkRunner.Run<FileGenerationBenchmark>();
//var summary2 = BenchmarkRunner.Run<FileProcessingBenchmark>();
//var summary3 = BenchmarkRunner.Run<FileCheckBenchmark>();

#else

var sw = Stopwatch.StartNew();



sw.Stop();
Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

#endif