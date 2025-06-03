using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataGeneration.Generators
{
    /// <summary>
    /// Interface for generating a chunk of data.
    /// </summary>
    public interface IChunkGenerator
    {
        /// <summary>
        /// Generates a chunk of data of the specified size.
        /// </summary>
        /// <param name="chunkSize">The size of the generated chunk.</param>
        /// <returns>Generated chunk as byte array.</returns>
        Task<byte[]> GenerateChunk(int chunkSize);
    }
}
