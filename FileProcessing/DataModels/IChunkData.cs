using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataModels
{
    /// <summary>
    /// Represents a chunk of data containing a list of data models.
    /// </summary>
    public interface IChunkData<T> : IEnumerator<T> where T : IDataModel
    {
        /// <summary>
        /// The unique identifier of the chunk, which is a GUID.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The data contained in the chunk, which is a list of data models.
        /// </summary>
        List<T> Data { get; }

        /// <summary>
        /// The total number of bytes analyzed to get data.
        /// </summary>
        int Length { get; }
    }
}
