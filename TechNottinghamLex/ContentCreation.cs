using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.LexEvents;
using Humanizer;
using TechNottingham.Common;

namespace TechNottinghamLex
{
    public static class ContentCreation
    {
        public const string WelcomeText = "Hello from Tech Nottingham. How can we help?";
        public const string ErrorText = "Sorry. I can't get hold of Tech Nottingham right now. Please try again later.";
        public const string HelpText = "You can find out about Tech Nottingham's work by asking about the mission statement, or find out about our next event by saying. ask technottingham about the next event";

        public const string MissionStatement = "Tech Nottingham is an organisation with the mission to Make Nottingham a better place to live and work in technology. All our events are free to attend, and we welcome everyone regardless of background or technical experience.";
        public const string ImageUrl = "https://s3-eu-west-1.amazonaws.com/technottinghamalexaimages/";

        public static string NoNextEvent(TechNottsEvent technotts)
        {
            return $"There's no confirmed{technotts.InSentence} events at the moment, but please check back soon as Tech Nottingham put on several events every month";
        }


        public static LexResponse.LexMessage NextEvent(TechNottsEvent technotts, MeetupEvent meetup, DateTime currentDateTime, bool hackTitle = true)
        {
            var eventDate = DateTime.Parse(meetup.local_date);
            var timeUntilEvent = currentDateTime.Subtract(eventDate);
            var preposition = char.IsNumber(timeUntilEvent.Humanize(1)[0]) ? " in" : string.Empty;
            var meetupName = System.Net.WebUtility.HtmlDecode(meetup.name.Contains(":") && hackTitle ? meetup.name.Substring(meetup.name.IndexOf(':')) : meetup.name);

            var sb = new StringBuilder();
            sb.AppendLine($"The next{technotts.InSentence} event is {meetupName}");
            sb.AppendLine();
            sb.AppendLine($"It's at {meetup.venue.name} on {eventDate:D}");

            return new LexResponse.LexMessage
            {
                Content = sb.ToString(),
                ContentType=AbstractIntentProcessor.MESSAGE_CONTENT_TYPE
            };
        }

        public static LexResponse.LexResponseCard NextEventCard(TechNottsEvent technotts)
        {
            return new LexResponse.LexResponseCard
            {
                GenericAttachments = new List<LexResponse.LexGenericAttachments>
                {
                    new LexResponse.LexGenericAttachments {Title=technotts.Name,ImageUrl = ImageUrl + technotts.LargeImage}
                }
            };
        }
    }
}
