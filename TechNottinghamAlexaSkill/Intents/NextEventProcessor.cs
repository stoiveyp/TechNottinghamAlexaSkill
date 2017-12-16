using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using TechNottingham.Common;

namespace TechNottinghamAlexaSkill.Intents
{
    public class NextEventProcessor
    {
        private IEnvironment Environment { get; }
        private IS3Client Client { get; }

        public NextEventProcessor(IEnvironment env,IS3Client client)
        {
            Environment = env;
            Client = client;
        }

        public async Task<SkillResponse> Process()
        {
            var technotts = TechNottsEvent.TechNottingham;
            var meetup = (await GetNextEventData(string.Empty)).FirstOrDefault();

            if (meetup != null)
            {
                var response = ResponseBuilder.Tell(ContentCreation.NextEvent(technotts, meetup, Environment.CurrentTime,false));
                if (!string.IsNullOrWhiteSpace(technotts.LargeImage))
                {
                    response.Response.Card = ContentCreation.EventCard(technotts, meetup.name,
                        $"{DateTime.Parse(meetup.local_date):D} at {meetup.venue.name}");
                }
                return response;
            }

            return ResponseBuilder.Tell(ContentCreation.NoNextEvent(technotts));
        }

        public async Task<SkillResponse> ProcessSpecific(Intent intent)
        {
            var technotts = TechNottsEvent.Parse(intent);

            if (string.IsNullOrWhiteSpace(technotts.Name) && intent.Slots.Count > 0)
            {
                Console.WriteLine("event failed: " + intent.Slots["event"].Value);
                return ResponseBuilder.Tell($"I'm sorry, I couldn't find information for {(string.IsNullOrWhiteSpace(intent.Slots["event"].Value) ? "that event" : intent.Slots["event"].Value)}. Please try again");
            }

            var meetups = await GetNextEventData(technotts.EventType);

            meetups = meetups.Where(m =>
                technotts.TitleFilter == null || m.name.IndexOf(technotts.TitleFilter, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
            if (meetups.Length > 0)
            {
                var meetup = meetups.First();
                var response = ResponseBuilder.Tell(ContentCreation.NextEvent(technotts, meetup, Environment.CurrentTime));
                if (!string.IsNullOrWhiteSpace(technotts.LargeImage))
                {
                    response.Response.Card = ContentCreation.EventCard(technotts, meetup.name,
                        $"{DateTime.Parse(meetup.local_date):D} at {meetup.venue.name}");
                }
                return response;
            }

            return ResponseBuilder.Tell(ContentCreation.NoNextEvent(technotts));
        }

        private Task<MeetupEvent[]> GetNextEventData(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return Client.GetEventData(S3Keys.EventData);
            }
            return Client.GetEventData(S3Keys.EventData + "_" + type);
        }
    }
}
