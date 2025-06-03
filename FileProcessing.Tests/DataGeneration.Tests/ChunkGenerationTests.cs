using FileProcessing.DataGeneration.Generators;
using FileProcessing.DataGeneration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataGeneration.Tests
{
    public class ChunkGenerationTests
    {
        private readonly Mock<IChunkGenerator> _chunkGeneratorMock;
        private readonly ChunkGeneration _chunkGeneration;

        public ChunkGenerationTests()
        {
            _chunkGeneratorMock = new Mock<IChunkGenerator>();
            _chunkGeneration = new ChunkGeneration(_chunkGeneratorMock.Object);
        }

        [Fact]
        public void Constructor_NullChunkGenerator_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ChunkGeneration(null!));
            Assert.Equal("chunkGenerator", exception.ParamName);
        }

        [Fact]
        public async Task GenerateChunks_ReturnsCorrectChunks()
        {
            // Arrange
            long dataSize = 512 * 1024 * 1024; // 512 MB
            long chunkSize = 256 * 1024 * 1024; // 256 MB
            _chunkGeneratorMock
                .Setup(g => g.GenerateChunk(It.IsAny<int>())) 
                .ReturnsAsync(new byte[chunkSize]);

            // Act
            var chunks = new List<byte[]>();
            await foreach (var chunk in _chunkGeneration.GenerateChunks(dataSize))
            {
                chunks.Add(chunk);
            }

            // Assert
            Assert.Equal(2, chunks.Count); // 2 chunks
            Assert.Equal(chunkSize, chunks[0].Length);
            Assert.Equal(chunkSize, chunks[1].Length);
        }

        [Fact]
        public async Task GenerateChunks_ReturnsFinalChunkForRemainingData()
        {
            // Arrange
            long dataSize = 600 * 1024 * 1024; // 600 MB
            long chunkSize = 256 * 1024 * 1024; // 256 MB
            int remainingSize = 88 * 1024 * 1024; // 88 MB remaining
            _chunkGeneratorMock
                .Setup(g => g.GenerateChunk(It.IsAny<int>()))
                .ReturnsAsync(new byte[chunkSize]);
            _chunkGeneratorMock
                .Setup(g => g.GenerateChunk(remainingSize))
                .ReturnsAsync(new byte[remainingSize]);

            // Act
            var chunks = new List<byte[]>();
            await foreach (var chunk in _chunkGeneration.GenerateChunks(dataSize))
            {
                chunks.Add(chunk);
            }

            // Assert
            Assert.Equal(3, chunks.Count); // 2 chunks and 1 remaining chunk
            Assert.Equal(chunkSize, chunks[0].Length);
            Assert.Equal(chunkSize, chunks[1].Length);
            Assert.Equal(remainingSize, chunks[2].Length);
        }

        [Fact]
        public async Task GenerateChunks_HandlesZeroDataSize()
        {
            // Arrange
            long dataSize = 0; // No data

            // Act
            var chunks = new List<byte[]>();
            await foreach (var chunk in _chunkGeneration.GenerateChunks(dataSize))
            {
                chunks.Add(chunk);
            }

            // Assert
            Assert.Empty(chunks); // Should return no chunks
        }
    }
}
