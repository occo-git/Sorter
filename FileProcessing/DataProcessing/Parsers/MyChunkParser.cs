using FileProcessing.DataModels;
using System;
using System.Text;

namespace FileProcessing.DataProcessing.Parsers
{
    /// <summary>
    /// Parses a chunk of data from a byte array.
    /// </summary>
    /// <remarks>The format of each record in chunk is "[Number]. [String]\r\n".</remarks>
    public class MyChunkParser : IChunkParser
    {
        public Task<IChunkData<IDataModel>> ParseChunk(byte[] buffer, int bytesRead)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null.", nameof(buffer));
            if (bytesRead <= 0 || bytesRead > buffer.Length)
                return Task.FromResult<IChunkData<IDataModel>>(null!);

            return Task.Run(() =>
            {
                int startIndex = 0;
                int textLength = 0;
                List<IDataModel> dataRecords = new List<IDataModel>();

                for (int i = 0; i < bytesRead; i++)
                {
                    if (buffer[i] == '\n') // new line symbol
                    {
                        int recordLength = i - startIndex;
                        if (recordLength > 3) // minimum length for a valid record (e.g., "1. \n")
                        {
                            int dotIndex = Array.IndexOf(buffer, (byte)'.', startIndex, recordLength); // finding the dot
                            if (dotIndex != -1 && buffer[dotIndex + 1] == ' ') // checking for ". "
                            {
                                textLength = recordLength - (dotIndex - startIndex) - 3; // -3 for ". " and '\r'
                                if (textLength > 0) // valid Text length
                                {
                                    // get Number
                                    if (int.TryParse(Encoding.UTF8.GetString(buffer, startIndex, dotIndex - startIndex), out int number))
                                    {
                                        // get Text
                                        var textBytes = new byte[textLength];
                                        Array.Copy(buffer, dotIndex + 2, textBytes, 0, textLength); // +2 for ". " after the Number
                                        string text = Encoding.UTF8.GetString(textBytes);
                                        string hash = $"{text}{number:D10}";

                                        // recordLength + 1 for '\n'
                                        var recordBytes = new byte[recordLength + 1];
                                        Array.Copy(buffer, startIndex, recordBytes, 0, recordLength + 1);

                                        dataRecords.Add(new MyDataModel(hash, recordBytes));
                                    }
                                }
                            }
                        }
                        startIndex = i + 1;
                    }
                }
                return new ChunkData<IDataModel>(dataRecords, startIndex) as IChunkData<IDataModel>;
            });
        }
    }
}