using System;
using System.Collections.Generic;
using System.Linq;

namespace TextMood.Shared
{
    public static class TextMoodModelServices
    {
        public static List<ITextMoodModel> FilterRecentTextModels(this IEnumerable<ITextMoodModel> textMoodModelList, TimeSpan timeSpan) =>
            textMoodModelList.Where(x => x.SentimentScore >= 0 && DateTimeOffset.Compare(x.UpdatedAt.ToUniversalTime(), DateTimeOffset.UtcNow.Add(-timeSpan)) > -1).ToList();

        public static (double red, double green, double blue) GetRGBValues(this double sentimentScore)
        {
            if (sentimentScore < 0 || sentimentScore > 1)
                return (1, 1, 1);

            return (1 - sentimentScore, sentimentScore, 0);
        }

        public static double GetAverageSentimentScore(this IEnumerable<ITextMoodModel> textMoodModelList) => textMoodModelList.Select(x => x.SentimentScore).Average() ?? -1;
    }
}
