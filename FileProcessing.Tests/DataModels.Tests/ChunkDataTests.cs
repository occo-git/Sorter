using FileProcessing.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;

namespace DataModels.Tests
{
    public class ChunkDataTests
    {
        [Fact]
        public void Constructor_ValidData_SetsProperties()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var mockChunkData = new Mock<IChunkData<IDataModel>>();

            // Arrange
            var model1 = new TestDataModel { Hash = "bData0000000020", Bytes = Encoding.UTF8.GetBytes("20. bData\r\n") };
            var model2 = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            var data = new List<IDataModel> { model1, model2 };
            mockChunkData.Setup(m => m.Id).Returns(expectedId);
            int length = 20;

            // Act
            var actualId = mockChunkData.Object.Id;
            var chunkData = new ChunkData<IDataModel>(data, length);

            // Assert
            Assert.Equal(expectedId, actualId);
            Assert.Equal(data, chunkData.Data);
            Assert.Equal(length, chunkData.Length);
        }

        [Fact]
        public void Constructor_NullData_ThrowsArgumentNullException()
        {
            // Arrange
            List<IDataModel> data = null!;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ChunkData<IDataModel>(data, 0));
            Assert.Equal("data", exception.ParamName);
        }

        [Fact]
        public void MoveNextAndReset_EnumeratesData()
        {
            // Arrange
            var model1 = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            var model2 = new TestDataModel { Hash = "bData0000000020", Bytes = Encoding.UTF8.GetBytes("20. bData\r\n") };
            var data = new List<IDataModel> { model1, model2 };
            var chunkData = new ChunkData<IDataModel>(data, 20);

            // Act & Assert
            Assert.True(chunkData.MoveNext());
            Assert.NotNull(chunkData.Current);
            Assert.Equal("aData0000000010", chunkData.Current.Hash);

            Assert.True(chunkData.MoveNext());
            Assert.NotNull(chunkData.Current);
            Assert.Equal("bData0000000020", chunkData.Current.Hash);

            Assert.False(chunkData.MoveNext()); // No more items
            Assert.Null(chunkData.Current);

            chunkData.Reset(); // Reset the enumerator
            Assert.True(chunkData.MoveNext()); // Should return to first item after reset
            Assert.NotNull(chunkData.Current);
            Assert.Equal("aData0000000010", chunkData.Current.Hash);
        }
    }
}