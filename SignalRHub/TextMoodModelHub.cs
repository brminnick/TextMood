using Microsoft.AspNet.SignalR;

using TextMood.Backend.Common;

namespace SignalRHub
{
    public class TextMoodModelHub : Hub
    {
        public void Hello(TextMoodModel textMoodModel)
        {
            Clients.All.broadcastMessage(textMoodModel);
        }
    }
}