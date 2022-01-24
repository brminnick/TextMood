using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextMood
{
    public class TextResultsService : BaseApiService
    {
        readonly ITextModelApi _textModelApiClient;

        public TextResultsService(ITextModelApi textModelApi) => _textModelApiClient = textModelApi;

        public Task<List<TextMoodModel>> GetTextModels() => AttemptAndRetry(_textModelApiClient.GetTextModels);
    }
}
