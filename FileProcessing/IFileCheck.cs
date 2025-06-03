using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing
{
    /// <summary>
    /// Interface for checking a file.
    /// </summary>
    public interface IFileCheck
    {
        /// <summary>
        /// Checks the data in a file.
        /// </summary>
        /// <param name="fileName">File to check.</param>
        Task CheckData(string fileName);
    }
}
