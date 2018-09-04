namespace TextMood.Shared
{
	public static class SignalRConstants
	{
		public const string TextMoodModelHubName = "TextMoodModelHub";
		public const string SendNewTextMoodModelName = "SendNewTextMoodModel";

		public static string SignalRHubUrl => $"https://textmoodsignalrwebapp.azurewebsites.net/{TextMoodModelHubName}";
	}
}
