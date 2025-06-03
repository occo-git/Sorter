using FileProcessing.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Tests
{
    /// <summary>
    /// Represents a test data model for unit testing purposes.
    /// </summary>
    internal class TestDataModel : IDataModel
    {
        public string Hash { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = default!;
    }
}
