using System;
using System.ComponentModel;

namespace TextMood.Shared
{
    public interface ITextMoodModel
    {
        string Id { get; init; }
        string Text { get; init; }
        double? SentimentScore { get; init; }
        DateTimeOffset CreatedAt { get; init; }
        DateTimeOffset UpdatedAt { get; init; }
        bool IsDeleted { get; init; }
    }
}

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public record IsExternalInit;
}
