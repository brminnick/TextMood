using System;
using System.Data.Linq.Mapping;

using TextMood.Shared;

namespace TextMood.Backend.Common
{
    [Table(Name = "TextModels")]
	public class TextMoodModel : ITextMoodModel
    {
        #region Constructrors
        public TextMoodModel(string text)
        {
            Id = Guid.NewGuid().ToString();
            Text = text;
            UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
        }

        [Obsolete("Use Overloaded Constructor")]
        public TextMoodModel()
        {
        }
        #endregion

        #region Properties
        public double? SentimentScore
        {
            get => SentimentScore_FromDatabase;
            set => SentimentScore_FromDatabase = value ?? -1;
        }

        [Column(Name = nameof(Id), IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Id { get; set; }

        [Column(Name = nameof(Text), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Text { get; set; }

        [Column(Name = nameof(SentimentScore), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public double SentimentScore_FromDatabase { get; set; }

        [Column(Name = nameof(CreatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(Name = nameof(UpdatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column(Name = nameof(IsDeleted), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public bool IsDeleted { get; set; }
        #endregion
    }
}
