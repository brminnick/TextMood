using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TextMood.Shared;

namespace TextMood.Backend.Common
{
    [Table(nameof(TextMoodModel))]
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

        public double? SentimentScore
        {
            get => SentimentScore_FromDatabase;
            set => SentimentScore_FromDatabase = value ?? -1;
        }

        [Column(nameof(SentimentScore))]
        [Obsolete("Use SentimentScore")]
        public double SentimentScore_FromDatabase { get; set; }

        [Key]
        public string Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
