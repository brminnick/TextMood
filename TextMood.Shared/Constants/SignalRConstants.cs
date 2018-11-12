namespace TextMood.Shared
{
    public static class SignalRConstants
    {
        public const string TextMoodModelHubName = "TextMoodModelHub";
        public const string SendNewTextMoodModelName = "SendNewTextMoodModel";
        public const string SignalRHubUrl = _signalRHubBaseUrl + "/" + TextMoodModelHubName;

#error Missing SignalR Hub Base Url
        const string _signalRHubBaseUrl = "Enter Base Url";
    }
}
