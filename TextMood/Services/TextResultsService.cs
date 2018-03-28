using System.Threading.Tasks;
using System.Collections.Generic;

namespace TextMood
{
	abstract class TextResultsService : BaseHttpClientService
    {
		public static Task<List<TextModel>> GetTextModels() =>
			GetDataObjectFromAPI<List<TextModel>>(BackendConstants.GetEmotionResultsAPIUrl);
    }
}
