using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Twilio.TwiML;

namespace TextMood.Functions
{
    public class TwilioServices
    {
        public string? GetTextMessageBody(string httpRequestBody, ILogger log)
        {
            var formValues = httpRequestBody.Split('&')
                                .Select(value => value.Split('='))
                                .ToDictionary(pair => Uri.UnescapeDataString(pair[0]).Replace("+", " "),
                                              pair => Uri.UnescapeDataString(pair[1]).Replace("+", " "));

            foreach (var value in formValues)
                log.LogInformation($"Key: {value.Key}, Value: {value.Value}");

            var textMessageKeyValuePair = formValues.Where(x => x.Key?.ToUpper()?.Equals("BODY") ?? false)?.FirstOrDefault();

            return textMessageKeyValuePair?.Value;
        }

        public string CreateTwilioResponse(string message)
        {
            var response = new MessagingResponse().Message(message);

            return response.ToString().Replace("utf-16", "utf-8");
        }
    }
}
