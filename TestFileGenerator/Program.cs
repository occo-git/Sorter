//#define benchmarks
using BenchmarkDotNet.Running;
using Bogus.DataSets;
using FileDataSorter.Benchmark;
using System.Diagnostics;
using TestFileGenerator;


#if benchmarks
var summary1 = BenchmarkRunner.Run<FileGenerationBenchmark>();
#else

string fileName = "d:/_data1";
long fileSize = 1024 * 1024 * 1024;
int fileCount = 1; // Number of files to generate   

while (true)
{
    Console.Clear();
    Console.WriteLine("Test file generator");
    Console.WriteLine($"File name: {fileName}");
    Console.WriteLine($"File size: {fileSize} B");
    Console.WriteLine($"File count: {fileCount}");

    Console.WriteLine("\nChoose an option:");
    Console.WriteLine("N - New parameters");
    Console.WriteLine("G - Generate");
    Console.WriteLine("Q - Quit");

    var key = Console.ReadKey(true).Key;

    if (key == ConsoleKey.N) // New parameters
    {
        Console.WriteLine("N");
        Console.WriteLine("Enter new file name: ");
        fileName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter new file size (in bytes): ");
        if (long.TryParse(Console.ReadLine(), out long newSize))
            fileSize = newSize;
        else
        {
            Console.WriteLine("Wrong input!");
            Console.WriteLine($"FileSize (same value): {fileSize} B");
        }

        Console.Write("Enter new file count: ");
        if (int.TryParse(Console.ReadLine(), out int newCount) && fileCount > 0)
            fileCount = newCount;
        else
        {
            Console.WriteLine("Wrong input!");
            Console.WriteLine($"FileCount (same value): {fileCount}");
        }
    }
    else if (key == ConsoleKey.G) // Generate files
    {
        Console.WriteLine("G");
        var sw = Stopwatch.StartNew();
        var generator = new MyTestFileGenerator();

        if (fileCount == 1)
        {
            Console.WriteLine($"\nGenerate 1 file: {fileName}, Size = {fileSize}");
            await generator.Generate(fileName, fileSize); // genarate 1 file
        }
        else
        {
            Console.WriteLine($"\nGenerate {fileCount} files: {fileName}, Size = {fileSize}");
            await generator.GenerateFiles(fileName, fileCount, fileSize); // generate multiple files
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