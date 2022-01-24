using System;
using System.ComponentModel;

namespace TextMood.Shared
{
    public interface ITextMoodModel
    {
        string Id { get; }
        string Text { get; }
        double? SentimentScore { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
        bool IsDeleted { get; }
    }
}

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public record IsExternalInit;
}
