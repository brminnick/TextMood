namespace TextMood.Shared
{
    public static class EmojiServices
    {
        public static string GetEmoji(double? sentimentScore)
        {
            switch (sentimentScore)
            {
                case double number when (number >= 0 && number < 0.4):
                    return EmojiConstants.SadFaceEmoji;
                case double number when (number >= 0.4 && number <= 0.6):
                    return EmojiConstants.NeutralFaceEmoji;
                case double number when (number > 0.6):
                    return EmojiConstants.HappyFaceEmoji;
                case null:
                    return EmojiConstants.BlankFaceEmoji;
                default:
                    return string.Empty;
            }
        }
    }
}
