using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Dialogflow.v2beta1.Data;
using TechNottingham.Common;

namespace TechNottinghamGoogleAction
{
    internal class NextEventProcessor
    {
        private IS3Client Client { get; }
        private IEnvironment Environment { get; }


        public NextEventProcessor(IEnvironment environment, IS3Client client)
        {
            Environment = environment;
            Client = client;
        }

        public async Task<ResponseLite> Process()
        {
            var technotts = TechNottsEvent.TechNottingham;
            var meetup = (await GetNextEventData(string.Empty)).FirstOrDefault();

            if (meetup != null)
            {
                var response = ResponseLite.AsSsml(ContentCreation.NextEvent(technotts, meetup, Environment.CurrentTime, false).ToXml());
                if (!string.IsNullOrWhiteSpace(technotts.LargeImage))
                {
                    ContentCreation.EventCard(response,technotts, meetup.name, $"{DateTime.Parse(meetup.local_date):D} at {meetup.venue.name}");
                }
                return response;
            }

            return ResponseLite.AsSsml(ContentCreation.NoNextEvent(technotts));
        }

        public async Task<ResponseLite> ProcessSpecific(QueryResult query)
        {
            const string slotName = "Event";
            var technotts = query.ParseTechNottsEvent();

            if (string.IsNullOrWhiteSpace(technotts.Name) && query.Parameters.Count > 0)
            {
                Console.WriteLine("event failed: " + query.Parameters[slotName]);
                return ResponseLite.AsText($"I'm sorry, I couldn't find information for {(string.IsNullOrWhiteSpace(query.Parameters[slotName].ToString()) ? "that event" : query.Parameters[slotName].ToString())}.");
            }

            var meetups = await GetNextEventData(technotts.EventType);

            meetups = meetups.Where(m =>
                technotts.TitleFilter == null || m.name.IndexOf(technotts.TitleFilter, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
            if (meetups.Length > 0)
            {
                var meetup = meetups.First();
                var response = ResponseLite.AsSsml(ContentCreation.NextEvent(technotts, meetup, Environment.CurrentTime).ToXml());
                if (!string.IsNullOrWhiteSpace(technotts.LargeImage))
                {
                    ContentCreation.EventCard(response,technotts, meetup.name,
                        $"{DateTime.Parse(meetup.local_date):D} at {meetup.venue.name}");
                }
                return response;
            }

            return ResponseLite.AsText(ContentCreation.NoNextEvent(technotts));
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