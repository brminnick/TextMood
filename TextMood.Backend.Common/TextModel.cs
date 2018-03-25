using System;
using System.Data.Linq.Mapping;

using TextMood.Shared;

namespace TextMood.Backend.Common
{
    public class TextModel : ITextModel
    {
        public TextModel(string text)
        {
            Id = Guid.NewGuid().ToString();
            Text = text;
            UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
        }

        [Column(Name = nameof(Id), IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Id { get; set; }

        [Column(Name = nameof(Text), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Text { get; set; }

        [Column(Name = nameof(SentimentScore), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public float? SentimentScore { get; set; }

        [Column(Name = nameof(CreatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(Name = nameof(UpdatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column(Name = nameof(IsDeleted), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public bool IsDeleted { get; set; }
    }
}
