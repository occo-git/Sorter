using System;
using System.Text;

namespace FileProcessing.DataModels
{
    /// <summary>
    /// Represents a data model that contains a byte array and a hash.
    /// </summary>
    public interface IDataModel
    {
        /// <summary>
        /// The hash of the data model, used for sorting and comparison.
        /// </summary>
        string Hash { get; }

        /// <summary>
        /// The byte array of the data model.
        /// </summary>
        byte[] Bytes { get; }
    }
}