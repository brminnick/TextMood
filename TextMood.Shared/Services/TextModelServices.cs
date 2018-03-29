using System;
using System.Collections.Generic;
using System.Linq;

namespace TextMood.Shared
{
	public static class TextModelServices
	{
		public static List<ITextMoodModel> GetRecentTextModels(IList<ITextMoodModel> textMoodModelList, TimeSpan timeSpan)
		{
			return textMoodModelList?.Where(x => x?.SentimentScore >= 0 &&
											 DateTimeOffset.Compare(x?.UpdatedAt.ToUniversalTime() ?? default(DateTimeOffset), DateTimeOffset.UtcNow.Add(-timeSpan)) > -1)?.ToList() ?? new List<ITextMoodModel>();
		}

		public static float GetAverageSentimentScore(IList<ITextMoodModel> textMoodModelList) => textMoodModelList?.Select(x => x.SentimentScore)?.Average() ?? -1;
	}
}
