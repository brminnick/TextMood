using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Refit;

namespace TextMood
{
    abstract class TextResultsService : BaseApiService
    {
        #region Constant Fields
        static Lazy<ITextModelApi> _textModelApiClientHolder => new Lazy<ITextModelApi>(() => RestService.For<ITextModelApi>(BackendConstants.GetEmotionResultsAPIUrl));
        #endregion

        #region Properties
        static ITextModelApi TextModelApiClient => _textModelApiClientHolder.Value;
        #endregion

        #region Methods
        public static Task<List<TextMoodModel>> GetTextModels() => ExecutePollyHttpFunction(TextModelApiClient.GetTextModels);
        #endregion
    }
}
