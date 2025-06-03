using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileProcessing.DataModels;
using FileProcessing.DataProcessing.Parsers;
using FileProcessing.DataProcessing;
using Moq;
using Xunit;
using DataModels.Tests;

namespace DataProcessing.Tests
{
    public class MyChunkProcessingTests
    {
        private readonly Mock<IChunkParser> _chunkParserMock;
        private readonly MyChunkProcessing _chunkProcessing;

        public MyChunkProcessingTests()
        {
            _chunkParserMock = new Mock<IChunkParser>();
            _chunkProcessing = new MyChunkProcessing(_chunkParserMock.Object);
        }

        [Fact]
        public async Task GetChunks_ReturnsChunks()
        {
            // Arrange
            var stream = new MemoryStream();
            var model = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            int bytesLength = model.Bytes.Length;
            var chunkData = new ChunkData<IDataModel>(new List<IDataModel> { model }, bytesLength);
            stream.Write(model.Bytes, 0, bytesLength);
            stream.Position = 0;

            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), It.IsAny<int>()))
                .ReturnsAsync(chunkData);

            // Write mock data to the stream
            stream.Write(model.Bytes, 0, 10);
            stream.Position = 0;

            // Act
            var chunks = _chunkProcessing.GetChunks(stream);
            var chunkList = await GetAsyncEnumerableResult(chunks);

            // Assert
            Assert.Single(chunkList);
            var resultChunk = chunkList.First();
            Assert.Single(resultChunk.Data);
            var dataModel = resultChunk.Data.First();
            Assert.Equal("aData0000000010", dataModel.Hash);
            Assert.Equal(bytesLength, dataModel.Bytes.Length);
            Assert.Equal("10. aData\r\n", Encoding.UTF8.GetString(dataModel.Bytes));
        }

        [Fact]
        public async Task MergeStreams_MergesMultipleStreams()
        {
            // Arrange
            var stream1 = new MemoryStream();
            var model1 = new TestDataModel { Hash = "aData0000000010", Bytes = Encoding.UTF8.GetBytes("10. aData\r\n") };
            var model3 = new TestDataModel { Hash = "cData0000000030", Bytes = Encoding.UTF8.GetBytes("30. cData\r\n") };
            int bytesLength1 = model1.Bytes.Length + model3.Bytes.Length;
            var chunkData1 = new ChunkData<IDataModel>(new List<IDataModel> { model1, model3 }, bytesLength1);
            stream1.Write(model1.Bytes, 0, model1.Bytes.Length); // model1
            stream1.Write(model3.Bytes, 0, model3.Bytes.Length); // model3
            stream1.Position = 0;

            var stream2 = new MemoryStream();
            var model2 = new TestDataModel { Hash = "bData0000000020", Bytes = Encoding.UTF8.GetBytes("20. bData\r\n") };
            int bytesLength2 = model2.Bytes.Length;
            var chunkData2 = new ChunkData<IDataModel>(new List<IDataModel> { model2 }, bytesLength2);
            stream2.Write(model2.Bytes, 0, model2.Bytes.Length); // model2
            stream2.Position = 0;

            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), bytesLength1))
                .ReturnsAsync(chunkData1);
            _chunkParserMock
                .Setup(cp => cp.ParseChunk(It.IsAny<byte[]>(), bytesLength2))
                .ReturnsAsync(chunkData2);
            int bytesLength = bytesLength1 + bytesLength2;

            // Act
            var streams = new List<Stream> { stream1, stream2 };
            var mergedData = new MemoryStream();
            await foreach (var stream in _chunkProcessing.MergeStreams(streams))
            {
                stream.Position = 0;
                await stream.CopyToAsync(mergedData);
            }

            // Check that the merged data matches the expected output
            var finalResult = Encoding.UTF8.GetString(mergedData.ToArray());
            Assert.Equal(bytesLength, finalResult.Length);
            Assert.Equal("10. aData\r\n20. bData\r\n30. cData\r\n", finalResult);
        }

        private async Task<List<T>> GetAsyncEnumerableResult<T>(IAsyncEnumerable<T> asyncEnumerable)
        {
            var resultList = new List<T>();
            await foreach (var item in asyncEnumerable)
            {
                resultList.Add(item);
            }
            return resultList;
        }
    }
}