using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Refit;

namespace TextMood
{
    abstract class TextResultsService : BaseApiService
    {
        readonly static Lazy<ITextModelApi> _textModelApiClientHolder = new Lazy<ITextModelApi>(() => RestService.For<ITextModelApi>(BackendConstants.GetEmotionResultsAPIUrl));

        static ITextModelApi TextModelApiClient => _textModelApiClientHolder.Value;

        public static Task<List<TextMoodModel>> GetTextModels() => AttemptAndRetry(TextModelApiClient.GetTextModels);
    }
}
