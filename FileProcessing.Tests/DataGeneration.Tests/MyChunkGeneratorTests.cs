using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileProcessing.DataGeneration.Generators;
using Xunit;

namespace DataGeneration.Tests
{
    public class MyChunkGeneratorTests
    {
        private readonly MyChunkGenerator _chunkGenerator;

        public MyChunkGeneratorTests()
        {
            _chunkGenerator = new MyChunkGenerator();
        }

        [Fact]
        public async Task GenerateChunk_ReturnsByteArray_WithinChunkSize()
        {
            int chunkSize = 200;
            byte[] result = await _chunkGenerator.GenerateChunk(chunkSize);

            Assert.NotNull(result);
            Assert.True(result.Length <= chunkSize, "The generated chunk is  exceeds the specified chunk size.");
        }

        [Fact]
        public async Task GenerateChunk_HandlesZeroChunkSize_ReturnsEmptyArray()
        {
            int chunkSize = 0; // zero size
            byte[] result = await _chunkGenerator.GenerateChunk(chunkSize);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GenerateChunk_GeneratesUniqueContent()
        {
            int chunkSize = 200;
            byte[] result1 = await _chunkGenerator.GenerateChunk(chunkSize);
            byte[] result2 = await _chunkGenerator.GenerateChunk(chunkSize);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1, result2); // Check that the two chunks are not the same
        }

        [Fact]
        public async Task GenerateChunk_HandlesLargeChunkSize()
        {
            int chunkSize = 256 * 1024 * 1024; // larger chunk size - 256 MB
            byte[] result = await _chunkGenerator.GenerateChunk(chunkSize);

            Assert.NotNull(result);
            Assert.True(result.Length <= chunkSize, "The generated chunk exceeds the specified chunk size.");
        }

        [Fact]
        public async Task GenerateChunk_ValidatesOutputFormat()
        {
            int chunkSize = 1000;
            byte[] result = await _chunkGenerator.GenerateChunk(chunkSize);
            string output = Encoding.UTF8.GetString(result);

            var records = output.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (var record in records)
            {
                Assert.Matches(new Regex(@"^\d+\.\s.+$"), record); // Check format "Number. String"
            }
        }
    }
}
