using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataModels
{
    /// <summary>
    /// Represents a data model for text record.
    /// </summary>
    public class MyDataModel : IDataModel
    {
        public string Hash { get; private set; }
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// Represents a data model for a text record.
        /// </summary>
        /// <param name="hash">Hash used for sorting and comparision.</param>
        /// <param name="bytes">Byte array.</param>
        public MyDataModel(string hash, byte[] bytes)
        {
            Hash = hash;
            Bytes = bytes;
        }
    }
}
