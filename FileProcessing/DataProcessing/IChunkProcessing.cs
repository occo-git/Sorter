using FileProcessing.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataProcessing
{
    /// <summary>
    /// Interface for processing chunks of data from a stream.
    /// </summary>
    public interface IChunkProcessing
    {
        /// <summary>
        /// Processes a stream of data and returns an enumerable of chunk data models.
        /// </summary>
        /// <param name="stream">Stream containing the data to be processed.</param>
        /// <returns>An asynchronous enumerable of chunk data models.</returns>
        IAsyncEnumerable<IChunkData<IDataModel>> GetChunks(Stream stream);

        /// <summary>
        /// Merges multiple streams into a single stream, yielding each merged stream as it is created.
        /// </summary>
        /// <param name="streams">Collection of streams to be merged.</param>
        /// <returns>An asynchronous enumerable of merged streams.</returns>
        IAsyncEnumerable<Stream> MergeStreams(IEnumerable<Stream> streams);
    }
}
