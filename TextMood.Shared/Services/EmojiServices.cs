namespace TextMood.Shared
{
    public static class EmojiServices
    {
        public static string GetEmoji(double? sentimentScore) => sentimentScore switch
        {
            double number when number >= 0 && number < 0.4 => EmojiConstants.SadFaceEmoji,
            double number when number >= 0.4 && number <= 0.6 => EmojiConstants.NeutralFaceEmoji,
            double number when number > 0.6 => EmojiConstants.HappyFaceEmoji,
            null => EmojiConstants.BlankFaceEmoji,
            _ => string.Empty,
        };
    }
}
