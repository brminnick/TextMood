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
            Id = Guid.NewGuid().ToString();
            UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use Overloaded Constructor")]
        public TextMoodModel()
        {

        }

        public string Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public double? SentimentScore { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
