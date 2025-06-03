using FileProcessing.DataModels;
using FileProcessing.DataProcessing.Parsers;
using System;

namespace FileProcessing.DataProcessing
{
    /// <summary>
    /// Processing chunks of data from a stream.
    /// </summary>
    public class MyChunkProcessing : IChunkProcessing
    {
        private const int DefaultChunkSize = 256 * 1024 * 1024; // 1/4 GB
        public int ChunkSize { get; private set; } = DefaultChunkSize;

        private readonly IChunkParser _chunkParser;

        /// <summary>
        /// Processing chunks of data from a stream.
        /// </summary>
        /// <param name="chunkParser">Parser for chunk data.</param>
        /// <param name="chunkSize">Maximum size of each chunk.</param>
        public MyChunkProcessing(IChunkParser chunkParser, int chunkSize = DefaultChunkSize)
        {
            _chunkParser = chunkParser ?? throw new ArgumentNullException(nameof(chunkParser));
            ChunkSize = chunkSize;
        }

        public async IAsyncEnumerable<IChunkData<IDataModel>> GetChunks(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            int chunkNumber = 0;
            await using var chunks = new ChunkEnumerator(stream, _chunkParser, ChunkSize);
            while (await chunks.MoveNextAsync())
            {
                Console.WriteLine($"Processing chunk {chunkNumber}: Bytes = {chunks.Current.Length}");
                chunkNumber++;

                yield return chunks.Current;
            }
            if (chunkNumber == 0)
                Console.WriteLine("No data found.");
        }

        /// <summary>
        /// Merges multiple streams and yields combined streams of data asynchronously.
        /// </summary>
        /// <param name="streams">Collection of streams to merge.</param>
        /// <returns>An async enumerable of streams containing merged data.</returns>
        public async IAsyncEnumerable<Stream> MergeStreams(IEnumerable<Stream> streams)
        {
            if (streams == null || !streams.Any())
                throw new ArgumentException("No streams provided for merging.", nameof(streams));

            using (var combinedStream = new MemoryStream())
            {
                int size = 0;
                await foreach (var record in MergeChunks(streams))
                {
                    if (size + record.Bytes.Length > ChunkSize)
                    {
                        combinedStream.Position = 0;
                        yield return combinedStream;
                        combinedStream.SetLength(0);
                        size = 0;
                    }
                    combinedStream.Write(record.Bytes, 0, record.Bytes.Length);
                    size += record.Bytes.Length;
                }
                if (size > 0)
                {
                    combinedStream.Position = 0;
                    yield return combinedStream;
                }
            }
        }

        /// <summary>
        /// Merges chunks from multiple streams and yields the minimum record from each chunk.
        /// </summary>
        /// <param name="streams">Collection of streams to process.</param>
        /// <returns>An async enumerable of IDataModel representing the ordered sequence of minimum records from each chunk.</returns>
        private async IAsyncEnumerable<IDataModel> MergeChunks(IEnumerable<Stream> streams)
        {
            if (streams == null || !streams.Any())
                throw new ArgumentException("No streams provided for merging chunks.", nameof(streams));

            var chunkEnumerators = new List<ChunkEnumerator>();
            var chunkList = new List<(ChunkEnumerator enumerator, IChunkData<IDataModel> chunkInfo)>();

            // Initialize chunk enumerators for each stream
            foreach (var stream in streams)
            {
                var chunks = new ChunkEnumerator(stream, _chunkParser, ChunkSize);
                chunkEnumerators.Add(chunks);

                // Get the first chunk from each enumerator
                if (await chunks.MoveNextAsync())
                {
                    chunks.Current.MoveNext();
                    chunkList.Add((chunks, chunks.Current));
                }
            }

            while (chunkList.Count > 0)
            {
                IDataModel minRecord = null!;
                ChunkEnumerator minEnumerator = null!;
                foreach (var (enumerator, chunkInfo) in chunkList)
                {
                    if (chunkInfo.Current != null)
                    {
                        // Compare the current record's hash with the minimum found so far
                        if (minRecord == null || string.Compare(chunkInfo.Current.Hash, minRecord.Hash) < 0)
                        {
                            minRecord = chunkInfo.Current;
                            minEnumerator = enumerator;
                        }
                    }
                }

                if (minRecord != null)
                {
                    yield return minRecord;
                        
                    var index = chunkList.FindIndex(c => c.enumerator == minEnumerator);
                    var currentChunkInfo = chunkList[index].chunkInfo;

                    // If the current chunk has no records
                    if (!currentChunkInfo.MoveNext())
                    {
                        // If the current chunk is exhausted, move to the next chunk
                        if (await minEnumerator.MoveNextAsync())
                        {
                            minEnumerator.Current.MoveNext(); // Move to the next record in the enumerator
                            chunkList[index] = (minEnumerator, minEnumerator.Current);
                        }
                        else
                            chunkList.Remove(chunkList[index]); // Remove exhausted enumerator
                    }
                }
            }

            foreach (var enumerator in chunkEnumerators)
                await enumerator.DisposeAsync();
        }
    }
}
