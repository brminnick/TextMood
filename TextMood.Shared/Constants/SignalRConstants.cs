namespace TextMood.Shared
{
	public static class SignalRConstants
	{
		public static string TextMoodModelHubName = "TextMoodModelHub";
		public static string SendNewTextMoodModelName = "SendNewTextMoodModel";

		public static string SignalRHubUrl => $"https://textmoodsignalrwebapp.azurewebsites.net/{TextMoodModelHubName}";
	}
}
