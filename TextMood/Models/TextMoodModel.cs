using System;

using TextMood.Shared;

namespace TextMood
{
	public class TextMoodModel : ITextMoodModel
	{
		public TextMoodModel(string text)
		{
			Id = Guid.NewGuid().ToString();
			Text = text;
			UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
		}

		public string Id { get; set; }
		public string Text { get; set; }
		public double? SentimentScore { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
		public bool IsDeleted { get; set; }
	}
}
