using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace TextMood.Functions
{
    class HttpRequestServices
    {
        public static async Task<string> GetContentAsString(Stream body)
        {
            using var streamReader = new StreamReader(body);

            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
