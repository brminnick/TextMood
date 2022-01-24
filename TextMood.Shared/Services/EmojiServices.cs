namespace TextMood.Shared
{
    public static class EmojiServices
    {
        public static string GetEmoji(double? sentimentScore) => sentimentScore switch
        {
            >= 0 and < 0.4 => EmojiConstants.SadFaceEmoji,
            >= 0.4 and 0.6 => EmojiConstants.NeutralFaceEmoji,
            > 0.6 => EmojiConstants.HappyFaceEmoji,
            null => EmojiConstants.BlankFaceEmoji,
            _ => string.Empty,
        };
    }
}
