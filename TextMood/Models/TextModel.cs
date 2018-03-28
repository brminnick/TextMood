using System;

using TextMood.Shared;

namespace TextMood
{
	public class TextModel : ITextModel
	{
		public TextModel(string text)
		{
			Id = Guid.NewGuid().ToString();
			Text = text;
			UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
		}

		public string Id { get; set; }
		public string Text { get; set; }
		public float? SentimentScore { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
		public bool IsDeleted { get; set; }
	}
}
