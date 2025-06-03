using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataModels.Tests;
using FileProcessing.DataModels;
using FileProcessing.DataProcessing.Parsers;
using Moq;
using Xunit;

namespace DataProcessing.Tests
{
    public class ChunkEnumeratorTests
    {
        private readonly Mock<IChunkParser> _chunkParserMock;
        private readonly MemoryStream _stream;

        public ChunkEnumeratorTests()
        {
            _chunkParserMock = new Mock<IChunkParser>();
            _stream = new MemoryStream();
        }

        [Fact]
        public void Constructor_NullStream_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ChunkEnumerator(null!, _chunkParserMock.Object));
            Assert.Equal("stream", exception.ParamName);
        }

        [Fact]
        public void Constructor_NullParser_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ChunkEnumerator(_stream, null!));
            Assert.Equal("chunkParser", exception.ParamName);
        }

        [Fact]
        public async Task MoveNextAsync_ReadsChunks()
        {
            // Arrange
            var stream = new MemoryStream();
            var model = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            int bytesLength = model.Bytes.Length;
            var chunkData = new ChunkData<IDataModel>(new List<IDataModel> { model }, bytesLength);
            _stream.Write(model.Bytes, 0, bytesLength);
            _stream.Position = 0;

            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), It.IsAny<int>()))
                .ReturnsAsync(chunkData);

            var enumerator = new ChunkEnumerator(_stream, _chunkParserMock.Object);

            // Act
            var hasNext = await enumerator.MoveNextAsync();

            // Assert
            Assert.True(hasNext);
            Assert.NotNull(enumerator.Current);
            Assert.Equal("aData0000000010", enumerator.Current.Data.First().Hash);
            Assert.Equal(bytesLength, enumerator.Current.Length);
        }

        [Fact]
        public async Task MoveNextAsync_HandlesEndOfStream()
        {
            // Arrange
            var stream = new MemoryStream();
            var model = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            int bytesLength = model.Bytes.Length;
            var chunkData = new ChunkData<IDataModel>(new List<IDataModel> { model }, bytesLength);
            _stream.Write(model.Bytes, 0, bytesLength);
            _stream.Position = 0;

            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), It.IsAny<int>()))
                .ReturnsAsync(chunkData);

            var enumerator = new ChunkEnumerator(_stream, _chunkParserMock.Object);

            // Act
            await enumerator.MoveNextAsync(); // First call
            var hasNext = await enumerator.MoveNextAsync(); // Second call

            // Assert
            Assert.False(hasNext); // Should be false since there are no more chunks
        }

        [Fact]
        public async Task DisposeAsync_DisposesStream()
        {
            // Arrange
            var stream = new MemoryStream();
            var model = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            int bytesLength = model.Bytes.Length;
            var chunkData = new ChunkData<IDataModel>(new List<IDataModel> { model }, bytesLength);
            _stream.Write(model.Bytes, 0, bytesLength);
            _stream.Position = 0;

            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), It.IsAny<int>()))
                .ReturnsAsync(chunkData);

            var enumerator = new ChunkEnumerator(_stream, _chunkParserMock.Object);

            // Act
            await enumerator.DisposeAsync();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => _stream.Position); // Stream should be disposed
        }

        [Fact]
        public async Task Reset_SetsEnumeratorState()
        {
            // Arrange
            var stream = new MemoryStream();
            var model = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            int bytesLength = model.Bytes.Length;
            var chunkData = new ChunkData<IDataModel>(new List<IDataModel> { model }, bytesLength);
            _stream.Write(model.Bytes, 0, bytesLength);
            _stream.Position = 0;

            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), It.IsAny<int>()))
                .ReturnsAsync(chunkData);

            var enumerator = new ChunkEnumerator(_stream, _chunkParserMock.Object);

            // Act
            await enumerator.MoveNextAsync();
            enumerator.Reset(); // Reset the enumerator

            // Assert
            Assert.Null(enumerator.Current);
        }
    }
}