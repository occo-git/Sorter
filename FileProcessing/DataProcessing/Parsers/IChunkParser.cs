using FileProcessing.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataProcessing.Parsers
{
    /// <summary>
    /// Interface for parsing a chunk of data from a byte array.
    /// </summary>
    public interface IChunkParser
    {
        /// <summary>
        /// Parses a chunk of data from a byte array and returns an object containing the parsed data models.
        /// </summary>
        /// <param name="buffer">Byte array containing data.</param>
        /// <param name="bytesRead">Number of bytes read from the buffer.</param>
        /// <returns>An object with the parsed data models.</returns>
        Task<IChunkData<IDataModel>> ParseChunk(byte[] buffer, int bytesRead);
    }
}
