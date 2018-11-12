namespace TextMood.Shared
{
    public static class SignalRConstants
    {
        public const string TextMoodModelHubName = "TextMoodModelHub";
        public const string SendNewTextMoodModelName = "SendNewTextMoodModel";
        public const string SignalRHubUrl = _signalRHubBaseUrl + "/" + TextMoodModelHubName;

        const string _signalRHubBaseUrl = "https://textmoodsignalrwebapp.azurewebsites.net";
    }
}
