using System;
using System.ComponentModel;
using TextMood.Shared;

namespace TextMood
{
    public class TextMoodModel : ITextMoodModel
    {
        public TextMoodModel(string text)
        {
            Text = text;
            UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use Overloaded Constructor")]
        public TextMoodModel()
        {

        }

        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string Text { get; init; } = string.Empty;
        public double? SentimentScore { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
        public bool IsDeleted { get; init; }
    }
}
