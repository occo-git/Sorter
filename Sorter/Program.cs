//#define benchmarks
using BenchmarkDotNet.Running;
using FileDataSorter.Benchmark;
using Sorter;
using System.Diagnostics;


#if benchmarks
var summary2 = BenchmarkRunner.Run<FileProcessingBenchmark>();
//var summary3 = BenchmarkRunner.Run<FileCheckBenchmark>();
#else

string fileName = "d:/_data1";
int fileCount = 1; // Number of files to sort   

while (true)
{
    Console.Clear();
    Console.WriteLine("Sorter");
    Console.WriteLine($"File name: {fileName}");
    Console.WriteLine($"File count: {fileCount}");

    Console.WriteLine("\nChoose an option:");
    Console.WriteLine("N - New parameters");
    Console.WriteLine("S - Sort");
    Console.WriteLine("Q - Quit");

    var key = Console.ReadKey(true).Key;

    if (key == ConsoleKey.N) // New parameters
    {
        Console.WriteLine("N");
        Console.WriteLine("Enter new file name: ");
        fileName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter new file count: ");
        if (int.TryParse(Console.ReadLine(), out int newCount) && fileCount > 0)
            fileCount = newCount;
        else
        {
            Console.WriteLine("Wrong input!");
            Console.WriteLine($"FileCount (same value): {fileCount}");
        }
    }
    else if (key == ConsoleKey.S) // Sort files
    {
        Console.WriteLine("S");
        var sw = Stopwatch.StartNew();
        var sorter = new MySorter();

        if (fileCount == 1)
        {
            Console.WriteLine($"\nSort 1 file: {fileName}");
            await sorter.Sort("d:/_data1"); // sort 1 file
        }
        else
        {
            Console.WriteLine($"\nSort {fileCount} files: {fileName}");
            await sorter.SortFiles(fileName, fileCount); // sort multiple files
        }
        sw.Stop();
        Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
    else if (key == ConsoleKey.Q) // Quit
    {
        Console.WriteLine("Q");
        break;
    }
}

#endif