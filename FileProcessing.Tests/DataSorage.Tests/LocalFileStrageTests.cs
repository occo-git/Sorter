using FileProcessing.DataStorage;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DataStorage.Tests
{
    public class LocalFileStorageTests : IDisposable
    {
        private const string TestFileName = "testfile.txt";
        private const string TestContent = "Hello, World!";

        private LocalFileStorage _fileStorage;

        public LocalFileStorageTests()
        {
            _fileStorage = new LocalFileStorage();

            if (File.Exists(TestFileName))
                File.Delete(TestFileName);
        }

        [Fact]
        public void CheckFile_NullOrEmptyFileName_ReturnsFalse()
        {
            Assert.False(_fileStorage.CheckFile(null!));
            Assert.False(_fileStorage.CheckFile(string.Empty));
        }

        [Fact]
        public void CheckFile_FileDoesNotExist_ReturnsFalse()
        {
            Assert.False(_fileStorage.CheckFile(TestFileName));
        }

        [Fact]
        public async Task WriteAsync_WritesDataToFile()
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestContent)))
            {
                await _fileStorage.WriteAsync(TestFileName, stream);
            }

            Assert.True(File.Exists(TestFileName));
            Assert.Equal(TestContent.Length, new FileInfo(TestFileName).Length);
        }

        [Fact]
        public async Task ReadAsync_ReadsDataFromFile()
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestContent)))
            {
                await _fileStorage.WriteAsync(TestFileName, stream);
            }

            using (var readStream = await _fileStorage.ReadAsync(TestFileName))
            {
                using (var reader = new StreamReader(readStream))
                {
                    var content = await reader.ReadToEndAsync();
                    Assert.Equal(TestContent, content);
                }
            }
        }

        [Fact]
        public async Task ReadMultiple_ReturnsStreamsForEachFile()
        {
            var fileNames = new List<string> { "file1.txt", "file2.txt" };
            foreach (var fileName in fileNames)
            {
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestContent)))
                {
                    await _fileStorage.WriteAsync(fileName, stream);
                }
            }
            var streams = _fileStorage.ReadMultiple(fileNames);

            Assert.Equal(fileNames.Count, streams.ToList().Count);

            foreach (var stream in streams)
                stream.Dispose();

            foreach (var fileName in fileNames)
                _fileStorage.Delete(fileName);            
        }

        [Fact]
        public async Task Delete_FileExists_DeletesFile()
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestContent)))
            {
                await _fileStorage.WriteAsync(TestFileName, stream);
            }

            _fileStorage.Delete(TestFileName);
            Assert.False(File.Exists(TestFileName));
        }

        public void Dispose()
        {
            if (File.Exists(TestFileName))
                File.Delete(TestFileName);
        }
    }
}