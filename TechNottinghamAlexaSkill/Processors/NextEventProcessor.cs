using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using TechNottingham.Common;

namespace TechNottinghamAlexaSkill.Processors
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

        public async Task<SkillResponse> Process(bool supportsDisplay)
        {
            var technotts = TechNottsEvent.TechNottingham;
            var meetupList = await GetNextEventData(string.Empty);
            var meetup = meetupList.FirstOrDefault();

            if (meetup == null)
            {
                return ResponseBuilder.Tell(ContentCreation.NoNextEvent(technotts));
            }

            var response = ResponseBuilder.Tell(ContentCreation.NextEvent(technotts, meetup, Environment.CurrentTime,false));
            if (!string.IsNullOrWhiteSpace(technotts.LargeImage))
            {
                response.Response.Card = ContentCreation.EventCard(technotts, meetup.name,
                    $"{DateTime.Parse(meetup.local_date):D} at {meetup.venue.name}");
            }

            if (supportsDisplay)
            {
                var template = ContentCreation.CreateMeetupListTemplate(meetupList.Take(4));
                var directive = new DisplayRenderTemplateDirective {Template = template};
                response.Response.Directives.Add(directive);
            }

            return response;

        }

        public async Task<SkillResponse> ProcessSpecific(Intent intent)
        {
            var technotts = intent.ParseTechNottsEvent();

            if (string.IsNullOrWhiteSpace(technotts.Name) && intent.Slots.Count > 0)
            {
                Console.WriteLine("event failed: " + intent.Slots["event"].Value);
                return ResponseBuilder.Tell($"I'm sorry, I couldn't find information for {(string.IsNullOrWhiteSpace(intent.Slots["event"].Value) ? "that event" : intent.Slots["event"].Value)}.");
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

        private async Task<MeetupEvent[]> GetNextEventData(string type)
        {
            Task<MeetupEvent[]> evenTask;
            if (string.IsNullOrWhiteSpace(type))
            {
                evenTask = Client.GetEventData(S3Keys.EventData);
            }
            else
            {
                evenTask = Client.GetEventData(S3Keys.EventData + "_" + type);
            }

            var results = await evenTask;
            return results.Where(me => DateTime.Parse(me.local_date) > Environment.CurrentTime).ToArray();
        }
    }
}
