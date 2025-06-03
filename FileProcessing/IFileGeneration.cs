using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing
{
    /// <summary>
    /// Interface for generating a file with specified size.
    /// </summary>
    public interface IFileGeneration
    {
        /// <summary>
        /// Generates a file with the specified name and size.
        /// </summary>
        /// <param name="fileName">File name to generate.</param>
        /// <param name="fileSize">Size of the file to generate in bytes.</param>
        Task Generate(string fileName, long fileSize);
    }
}
