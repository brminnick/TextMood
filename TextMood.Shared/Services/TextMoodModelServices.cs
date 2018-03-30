using System;
using System.Collections.Generic;
using System.Linq;

namespace TextMood.Shared
{
	public static class TextMoodModelServices
	{
		public static List<ITextMoodModel> GetRecentTextModels(IList<ITextMoodModel> textMoodModelList, TimeSpan timeSpan)
		{
			return textMoodModelList?.Where(x => x?.SentimentScore >= 0 &&
											 DateTimeOffset.Compare(x?.UpdatedAt.ToUniversalTime() ?? default(DateTimeOffset), DateTimeOffset.UtcNow.Add(-timeSpan)) > -1)?.ToList() ?? new List<ITextMoodModel>();
		}

		public static (float red, float green, float blue) GetRGBFromSentimentScore(float? sentimentScore)
        {
            if (sentimentScore == null || sentimentScore < 0 || sentimentScore > 1)
                return (1, 1, 1);

            return (1 - (float)sentimentScore, (float)sentimentScore, 0);
        }

		public static float GetAverageSentimentScore(IList<ITextMoodModel> textMoodModelList) => textMoodModelList?.Select(x => x.SentimentScore)?.Average() ?? -1;
	}
}
