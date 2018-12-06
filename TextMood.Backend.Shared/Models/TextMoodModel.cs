using System;

using NPoco;

using TextMood.Shared;

namespace TextMood.Backend.Common
{
    [TableName(nameof(TextMoodModel))]
    [PrimaryKey(nameof(Id), AutoIncrement = false)]
    public class TextMoodModel : ITextMoodModel
    {
        #region Constructrors
        public TextMoodModel(string text)
        {
            Id = Guid.NewGuid().ToString();
            Text = text;
            UpdatedAt = CreatedAt = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties
        [Ignore]
        public double? SentimentScore
        {
            get => SentimentScore_FromDatabase;
            set => SentimentScore_FromDatabase = value ?? -1;
        }

        [Column(nameof(SentimentScore))]
        [Obsolete("Use SentimentScore Instead")]
        public double SentimentScore_FromDatabase { get; set; }

        public string Id { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        #endregion
    }
}
