using System;
using System.Linq;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using Humanizer;
using Humanizer.Localisation;
using TechNottingham.Common;

namespace TechNottinghamAlexaSkill
{
    public static class ContentCreation
    {
        public const string WelcomeText = "Hello from Tech Nottingham. How can we help?";
        public const string ErrorText = "Sorry. I can't get hold of Tech Nottingham right now. Please try again later.";
        public const string HelpText = @"You can find out more about Tech Nottingham's work through their mission statement, or you can find out when event's are running. Tech Nottingham run several events every month. The main Tech Nottingham event, as well as Women in Technology, Nott Tuesday, and Tech on Toast.  What would you like to know?";

        public const string MissionStatement = "Tech Nottingham is an organisation with the mission to Make Nottingham a better place to live and work in technology. All our events are free to attend, and we welcome everyone regardless of background or technical experience.";
        public const string ImageUrl = "https://s3-eu-west-1.amazonaws.com/technottinghamalexaimages/";
        public static ICard MissionStatementCard => EventCard(TechNottsEvent.TechNottingham, "Tech Nottingham", MissionStatement);

        public static TechNottsEvent ParseTechNottsEvent(this Intent intent)
        {
            string slotName = "event";
            if (intent.Slots == null || !intent.Slots.ContainsKey(slotName))
            {
                return TechNottsEvent.Empty;
            }

            if (string.IsNullOrWhiteSpace(intent.Slots[slotName].Value))
            {
                return TechNottsEvent.Empty;
            }

            var name = intent.Slots[slotName].Value;
            Console.WriteLine(name);
            var authority = intent.Slots[slotName]?.Resolution?.Authorities?.FirstOrDefault()?.Values.FirstOrDefault();
            if (authority != null)
            {
                name = authority.Value.Name;
            }

            name = name.ToLower();
            return TechNottsEvent.EventList.ContainsKey(name) ? TechNottsEvent.EventList[name] : TechNottsEvent.Empty;
        }

        public static ICard EventCard(TechNottsEvent meetup, string title, string content)
        {
            return new StandardCard
            {
                Title = title,
                Content = content,
                Image = new CardImage
                {
                    SmallImageUrl = ImageUrl + meetup.LargeImage,
                    LargeImageUrl = ImageUrl + meetup.SmallImage
                }
            };
        }

        public static string NoNextEvent(TechNottsEvent technotts)
        {
            return $"There's no confirmed{technotts.InSentence} events at the moment, but please check back soon as Tech Nottingham put on several events every month";
        }


        public static Speech NextEvent(TechNottsEvent technotts, MeetupEvent meetup, DateTime currentDateTime,bool hackTitle = true)
        {
            var speech = new Speech();

            var dateSummary = new Paragraph();

            var eventDate = DateTime.Parse(meetup.local_date);
            var difference = eventDate.Subtract(currentDateTime);
            var timeInWords = DateTime.Parse("01/01/1980 " + meetup.local_time).ToString("h mm tt");

            var days = difference.TotalDays == 0 ? difference : TimeSpan.FromDays(difference.TotalDays+1);

            var preposition = char.IsNumber(days.Humanize(maxUnit:TimeUnit.Week,minUnit:TimeUnit.Day,precision:2)[0]) ? " in" : string.Empty;

            dateSummary.Elements.Add(new Sentence($"The next{technotts.InSentence} event is{preposition} {days.Humanize(maxUnit:TimeUnit.Week,minUnit:TimeUnit.Day,precision:2)}"));
            dateSummary.Elements.Add(new Sentence($", {eventDate.ToOrdinalWords()}, at {timeInWords}. "));
            speech.Elements.Add(dateSummary);

            var eventSummary = new Paragraph();
            eventSummary.Elements.Add(new Sentence($"It's at {meetup.venue.name}."));

            //var title = new Prosody{Rate=ProsodyRate.Slow};
            var meetupName = System.Net.WebUtility.HtmlDecode(meetup.name.Contains(":") && hackTitle? meetup.name.Substring(meetup.name.IndexOf(':')): meetup.name);
            eventSummary.Elements.Add(new Sentence($"and is called {meetupName}"));
            //eventSummary.Elements.Add(title);
            speech.Elements.Add(eventSummary);

            return speech;
        }

    }
}
