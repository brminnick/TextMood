# TextMood
An IoT + Azure sample that detects the sentiment of incoming text messages, performs sentiment analysis on the text, and changes the color of a Philips Hue lightbulb to correspond with the average sentiment.

## Architecture

![](https://github.com/brminnick/Videos/blob/master/TextMood/TextMoodArchitecture.png)

## Demo

1. The phone on the left sends a text message to the Twilio phone number.
2. Twilio forwards the text message to an Azure Function
3. This first Azure Function determines the sentiment of the message then sends an emoji response back to the phone on the left
4. The second Azure Function saves the message to an Azure SQL Database
5. The third Azure Function invokes SignalR
6. SignalR sends a real-time update to the phone on the right, containing the new message
7. The phone on the right displays the new message, then updates its background color to match the average sentiment of the received messages
8. The phone on the right then simultaneously changes the light color of the Philips Hue light to match the average sentiment

![](https://github.com/brminnick/Videos/blob/master/TextMood/TextMoodGif.gif)
