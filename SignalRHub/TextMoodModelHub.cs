using Microsoft.AspNet.SignalR;

using TextMood.Backend.Common;

namespace SignalRHub
{
    public class TextMoodModelHub : Hub
    {
		public void SendNewTextMoodModel(TextMoodModel textMoodModel)
        {
            Clients.Others.SendNewTextMoodModel(textMoodModel);
        }
    }
}