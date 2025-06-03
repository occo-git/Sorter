using FileProcessing.DataModels;
using FileProcessing.DataProcessing.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataProcessing.Parsers
{
    /// <summary>
    /// Asynchronous enumerator for processing chunks of data from a stream.
    /// </summary>
    public class ChunkEnumerator : IAsyncEnumerator<IChunkData<IDataModel>>
    {
        private const int DefaultChunkSize = 256 * 1024 * 1024; // 1/4 GB
        public int ChunkSize { get; private set; } = DefaultChunkSize;

        private readonly Stream _stream;
        private readonly IChunkParser _chunkParser;
        private long totalBytesRead = 0;
        private IChunkData<IDataModel> current = null!;

        /// <summary>
        /// Initializes asynchronous enumerator for processing chunks of data from a stream.
        /// </summary>
        /// <param name="stream">Stream containing the data to be processed.</param>
        /// <param name="chunkParser">Parser for chunk data.</param>
        /// <param name="chunkSize">Maximum size of each chunk.</param>
        public ChunkEnumerator(Stream stream, IChunkParser chunkParser, int chunkSize = DefaultChunkSize)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _chunkParser = chunkParser ?? throw new ArgumentNullException(nameof(chunkParser));
            ChunkSize = chunkSize;
            Reset();
        }

        /// <summary>
        /// Asynchronously moves to the next chunk in the stream.
        /// </summary>
        /// <returns>True if a chunk was successfully read; otherwise, false.</returns>
        public async ValueTask<bool> MoveNextAsync()
        {
            if (totalBytesRead >= _stream.Length)
                return false;

            int bufferSize = (int)Math.Min(ChunkSize, _stream.Length - totalBytesRead);
            byte[] buffer = new byte[bufferSize];
            int bytesRead = await _stream.ReadAsync(buffer, 0, bufferSize);
            if (bytesRead == 0)
            {
                current = null!;
                return false;
            }

            current = await _chunkParser.ParseChunk(buffer, bytesRead);
            if (current == null || current.Length == 0)
                return false;

            totalBytesRead += current.Length;

            _stream.Seek(totalBytesRead, SeekOrigin.Begin);

            return true;
        }

        /// <summary>
        /// Gets the current chunk of data being processed.
        /// </summary>
        public IChunkData<IDataModel> Current => current;

        /// <summary>
        /// Resets the enumerator to the beginning of the stream.
        /// </summary>
        public void Reset()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            totalBytesRead = 0;
            current = null!;
        }

        /// <summary>
        /// Asynchronously disposes of the enumerator
        /// </summary>
        public ValueTask DisposeAsync()
        {
            _stream.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
