using System;

namespace TextMood.Shared
{
    public interface ITextMoodModel
    {
		string Id { get; set; }
		string Text { get; set; }
        float? SentimentScore { get; set; }
		DateTimeOffset CreatedAt { get; set; }
		DateTimeOffset UpdatedAt { get; set; }
		bool IsDeleted { get; set; }
    }
}
