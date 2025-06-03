using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing
{
    /// <summary>
    /// Interface for processing a file.
    /// </summary>
    public interface IFileProcessing
    {
        /// <summary>
        /// Processes a file and returns the result as a string.
        /// </summary>
        /// <param name="fileName">File name to process.</param>
        Task<string> ProcessFile(string fileName);
    }
}
