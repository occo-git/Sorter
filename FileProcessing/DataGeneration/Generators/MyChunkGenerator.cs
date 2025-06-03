using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.DataGeneration.Generators
{
    /// <summary>
    /// Generates random chunk.
    /// </summary>
    /// <remarks>The format of each record in chunk is "[Number]. [String]\r\n".</remarks>
    public class MyChunkGenerator : IChunkGenerator
    {
        private const int maxN = int.MaxValue;
        private const int maxWords = 10;

        private Random random = new Random();
        private Faker faker = new Faker();
        private StringBuilder chunkStringBuilder = new StringBuilder();
        private StringBuilder recordBuilder = new StringBuilder();

        public Task<byte[]> GenerateChunk(int chunkSize)
        {
            if (chunkSize <= 0)
                return Task.FromResult(Array.Empty<byte>());
            
            return Task.Run(() =>
            {
                bool next = true;
                while (next)
                {
                    int n = random.Next(1, maxN);
                    int wordsCount = random.Next(1, maxWords);
                    string str = faker.Lorem.Sentence(wordsCount);
                    recordBuilder.Append(n);
                    recordBuilder.Append(". ");
                    recordBuilder.Append(str);
                    recordBuilder.Append("\r\n");
                    if (recordBuilder.Length + chunkStringBuilder.Length > chunkSize)
                        next = false;
                    else
                        chunkStringBuilder.Append(recordBuilder);
                    recordBuilder.Clear();
                }

                var batchBytes = Encoding.UTF8.GetBytes(chunkStringBuilder.ToString());
                chunkStringBuilder.Clear();
                return batchBytes;
            });
        }
    }
}
