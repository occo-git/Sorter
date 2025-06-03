using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataGeneration
{
    /// <summary>
    /// Interface for generating chunks of data.
    /// </summary>
    public interface IChunkGeneration
    {
        /// <summary>
        /// Generates chunks of data.
        /// </summary>
        /// <param name="dataSize">The maximum size of the generated data.</param>
        /// <returns>Generated chunks as byte arrays.</returns>
        IAsyncEnumerable<byte[]> GenerateChunks(long dataSize);
    }
}
