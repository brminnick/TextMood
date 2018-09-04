using Microsoft.AspNetCore.SignalR;

using TextMood.Backend.Common;

namespace TextMood.SignalR
{
    public class TextMoodModelHub : Hub
    {
        public void SendNewTextMoodModel(TextMoodModel textMoodModel) => Clients.All.SendAsync(nameof(SendNewTextMoodModel), textMoodModel);
    }
}