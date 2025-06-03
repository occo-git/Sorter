using System;
using System.Collections;

namespace FileProcessing.DataModels
{
    /// <summary>
    /// Represents a chunk of data containing a list of data models.
    /// </summary>
    public class ChunkData<T> : IChunkData<T> where T : IDataModel
    {
        public Guid Id { get; } = Guid.NewGuid();
        public List<T> Data { get; private set; }
        public int Length { get; private set; }

        private int _position = -1;
        private T current = default!;

        /// <summary>
        /// Represents a chunk of data containing a list of data models.
        /// </summary>
        /// <param name="data">The list of data models.</param>
        /// <param name="length">The total number of bytes analyzed to get data.</param>
        public ChunkData(List<T> data, int length)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Length = length;
        }

        /// <summary>
        /// Gets the current data model in the enumeration.
        /// </summary>
        public T Current => current;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Moves to the next data model in the chunk.
        /// </summary>
        /// <returns>true if the next data model is available; otherwise, false.</returns>
        public bool MoveNext()
        {
            if (_position >= Data.Count - 1)
            {
                current = default!;
                return false;
            }

            _position++;
            current = Data[_position];

            return true;
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public void Reset()
        {
            _position = -1;
            current = default!;
        }

        public void Dispose()
        {
            
        }
    }
}
