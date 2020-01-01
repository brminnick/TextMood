using System;
using System.ComponentModel;
using TextMood.Shared;

namespace TextMood
{
    public class TextMoodModel : ITextMoodModel
    {
        public TextMoodModel(string text) : this()
        {
            Text = text;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use Overloaded Constructor")]
        public TextMoodModel()
        {
            Id = Guid.NewGuid().ToString();
            UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
        }

        public string Id { get; }
        public string Text { get; } = string.Empty;
        public double? SentimentScore { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }
        public bool IsDeleted { get; }
    }
}
