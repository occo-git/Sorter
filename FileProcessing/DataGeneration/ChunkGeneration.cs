using FileProcessing.DataGeneration.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataGeneration
{
    /// <summary>
    /// Generates chunks of data based on a specified chunk size. 
    /// </summary>
    public class ChunkGeneration : IChunkGeneration
    {
        private const int DefaultChunkSize = 256 * 1024 * 1024; // 256 MB
        public int ChunkSize { get; private set; } = DefaultChunkSize;

        private readonly IChunkGenerator _chunkGenerator;

        /// <summary>
        /// Generates chunks of data based on a specified chunk size. 
        /// </summary>
        /// <param name="chunkGenerator">Generates a chunk of data.</param>
        /// <param name="chunkSize">The maximum size of each chunk.</param>
        public ChunkGeneration(IChunkGenerator chunkGenerator, int chunkSize = DefaultChunkSize)
        {
            _chunkGenerator = chunkGenerator ?? throw new ArgumentNullException(nameof(chunkGenerator));
            ChunkSize = chunkSize;
        }

        public async IAsyncEnumerable<byte[]> GenerateChunks(long dataSize)
        {
            if (dataSize <= 0)
                yield break;
            
            int t = (int)(dataSize / ChunkSize);
            long size = 0;
            for (int i = 0; i < t; i++)
            {
                Console.WriteLine($"Generate chunk: Size = {ChunkSize}, Iteration = {i}/{t}");
                byte[] chunkBytes = await _chunkGenerator.GenerateChunk(ChunkSize);
                size += chunkBytes.Length;
                yield return chunkBytes;
            }
            int rest = (int)(dataSize - size);
            if (rest > 0)
            {
                Console.WriteLine($"Generate chunk: Size = {rest}, Iteration = {t}/{t}");
                yield return await _chunkGenerator.GenerateChunk(rest);
            }
        }
    }
}
