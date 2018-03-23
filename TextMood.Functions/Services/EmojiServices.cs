using System;
namespace TextMood.Functions
{
    public static class EmojiServices
    {
		public static string GetEmoji(float? sentimentScore)
        {
            switch (sentimentScore)
            {
                case float number when (number >= 0 && number < 0.4):
                    return EmojiConstants.SadFaceEmoji;
                case float number when (number >= 0.4 && number <= 0.6):
                    return EmojiConstants.NeutralFaceEmoji;
                case float number when (number > 0.6):
                    return EmojiConstants.HappyFaceEmoji;
                case null:
                    return EmojiConstants.BlankFaceEmoji;
                default:
                    return string.Empty;
            }
        }
    }
}
