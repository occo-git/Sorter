using FileProcessing.DataModels;
using FileProcessing.DataProcessing.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataProcessing.Tests
{
    public class MyChunkParserTests
    {
        private readonly MyChunkParser _parser;

        public MyChunkParserTests()
        {
            _parser = new MyChunkParser();
        }

        [Fact]
        public async Task ParseChunk_ValidData()
        {
            // Arrange
            var inputData = Encoding.UTF8.GetBytes($"1. First record.\r\n2. Second record.\r\n");
            int bytesRead = inputData.Length;

            // Act
            var result = await _parser.ParseChunk(inputData, bytesRead);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bytesRead, result.Length);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("First record.0000000001", result.Data[0].Hash);
            Assert.Equal("Second record.0000000002", result.Data[1].Hash);
        }

        [Fact]
        public async Task ParseChunk_IgnoresInvalidRecords_EmptyRecords()
        {
            // Arrange
            var inputData = Encoding.UTF8.GetBytes("1. First record.\r\n\r\n2. Second record.\r\n\r\n\r\n");
            int bytesRead = inputData.Length;

            // Act
            var result = await _parser.ParseChunk(inputData, bytesRead);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bytesRead, result.Length);
            Assert.Equal(2, result.Data.Count); // 2 valid records
            Assert.Equal("First record.0000000001", result.Data[0].Hash);
            Assert.Equal("Second record.0000000002", result.Data[1].Hash);
        }

        [Fact]
        public async Task ParseChunk_IgnoresInvalidRecords_NoNumber()
        {
            // Arrange
            var inputData = Encoding.UTF8.GetBytes("1. Valid record.\r\nInvalid record.\r\n3. Another valid record.\r\n");
            int bytesRead = inputData.Length;

            // Act
            var result = await _parser.ParseChunk(inputData, bytesRead);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bytesRead, result.Length);
            Assert.Equal(2, result.Data.Count); // 2 valid records
            Assert.Equal("Valid record.0000000001", result.Data[0].Hash);
            Assert.Equal("Another valid record.0000000003", result.Data[1].Hash);
        }

        [Fact]
        public async Task ParseChunk__IgnoresInvalidRecords_EmptyString()
        {
            // Arrange
            var inputData = Encoding.UTF8.GetBytes("1. Valid record.\r\n2. \r\n3. Another valid record.\r\n");
            int bytesRead = inputData.Length;

            // Act
            var result = await _parser.ParseChunk(inputData, bytesRead);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bytesRead, result.Length);
            Assert.Equal(2, result.Data.Count); // 2 valid records
            Assert.Equal("Valid record.0000000001", result.Data[0].Hash);
            Assert.Equal("Another valid record.0000000003", result.Data[1].Hash);
        }

        [Fact]
        public async Task ParseChunk_EmptyData_ReturnsEmptyChunkData()
        {
            // Arrange
            var inputData = Encoding.UTF8.GetBytes("");
            int bytesRead = inputData.Length;

            // Act
            var result = await _parser.ParseChunk(inputData, bytesRead);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ParseChunk_SingleLineWithoutDot_ReturnsEmptyChunkData()
        {
            // Arrange
            var inputData = Encoding.UTF8.GetBytes("Just a single record without a number or dot\r\n");
            int bytesRead = inputData.Length;

            // Act
            var result = await _parser.ParseChunk(inputData, bytesRead);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bytesRead, result.Length);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data); // No valid records
        }
    }
}