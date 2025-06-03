using FileProcessing;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataStorage;

namespace Sorter
{
    internal class MySorter
    {
        public async Task Sort(string fileName)
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            IDataStorage storage = new LocalFileStorage();

            IChunkParser parser = new MyChunkParser();
            IFileProcessing fileProcessing = new MyFileProcessing(storage, parser, chunkSize);

            string resultFileName = await fileProcessing.ProcessFile($"{fileName}.txt");

            //IFileCheck fileCheck = new MyFileCheck(storage, parser, chunkSize);
            //await fileCheck.CheckData(resultFileName);
        }

        public async Task SortFiles(string fileName, int count)
        {
            int chunkSize = 256 * 1024 * 1024; // 256 MB
            IDataStorage storage = new LocalFileStorage();

            var tasks = new List<Task>();
            for (int i = 0; i < count; i++)
            {
                IChunkParser parser = new MyChunkParser();
                IFileProcessing fileProcessing = new MyFileProcessing(storage, parser, chunkSize);
                tasks.Add(fileProcessing.ProcessFile($"{fileName}_{i + 1}.txt"));
            }
            await Task.WhenAll(tasks);
        }
    }
}
