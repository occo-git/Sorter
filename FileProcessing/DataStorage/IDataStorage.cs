namespace FileProcessing.DataStorage
{
    /// <summary>
    /// Interface for data storage operations.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// Checks content key and existence in the storage.
        /// </summary>
        /// <returns>True content exists and is not empty, otherwise false.</returns>
        bool CheckFile(string key);

        /// <summary>
        /// Writes content to the storage with the specified key.
        /// </summary>
        Task WriteAsync(string key, Stream contentStream);

        /// <summary>
        /// Reads content from the storage with the specified key.
        /// </summary>
        Task<Stream> ReadAsync(string key);

        /// <summary>
        /// Reads multiple contents from the storage with the specified keys.
        /// </summary>
        /// <param name="keys">Collection of keys to read from the storage.</param>
        /// <returns>An synchronous enumerable of streams corresponding to the keys.</returns>
        IEnumerable<Stream> ReadMultiple(IEnumerable<string> keys);

        /// <summary>
        /// Deletes the content associated with the specified key from the storage.
        /// </summary>
        void Delete(string key);
    }
}