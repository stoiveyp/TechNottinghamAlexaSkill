using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace TechNottinghamGoogleAction
{
    public class ResponseLite
    {
        private ResponseLite() { }

        public static ResponseLite AsText(string text)
        {
            return new ResponseLite { Text = text };
        }

        public static ResponseLite AsSsml(string ssml)
        {
            return new ResponseLite { Text = ssml };
        }

        public void AddCard(string title, string subtitle, string image)
        {
            if (Messages == null)
            { 
                Messages = new List<Message>();
            }

            Messages.Add(new Message{Card=new Card{Title = title, Subtitle = subtitle, ImageUri = image}});
        }

        [JsonProperty("fulfillment_text", NullValueHandling = NullValueHandling.Ignore)] public string Text { get; set; }
        [JsonProperty("fulfillment_messages", NullValueHandling = NullValueHandling.Ignore)] public List<Message> Messages { get; set; }
    }

    public class Message{
        [JsonProperty("card")]public Card Card { get; set; }
    }

    public class Card
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("image_uri")]
        public string ImageUri { get; set; }
    }

    public interface IMessage
    {

    }

    public class Data
    {
        [JsonProperty("google")]
        public Google Google { get; set; }
    }

    public class Google
    {
        [JsonProperty("is_ssml")]
        public bool IsSsml { get; set; }

        [JsonProperty("expect_user_response")]
        public bool ExpectResponse { get; set; }
    }

    //    {
    //    "speech": "...",  // ASCII characters only
    //    "displayText": "...",
    //    "data": {
    //    "google": {
    //    "expect_user_response": true,
    //    "is_ssml": true,
    //    "permissions_request": {
    //    "opt_context": "...",
    //    "permissions": [
    //    "NAME",
    //    "DEVICE_COARSE_LOCATION",
    //    "DEVICE_PRECISE_LOCATION"
    //    ]
    //}
    //}
    //},
    //"contextOut": [...],
    //}
}