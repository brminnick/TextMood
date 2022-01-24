using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace TextMood.Functions
{
    class HttpRequestServices
    {
        public static async Task<string> GetContentAsString(HttpRequest request)
        {
            using var streamReader = new StreamReader(request.Body);

            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
