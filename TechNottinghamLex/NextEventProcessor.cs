using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using TechNottingham.Common;

namespace TechNottinghamLex
{
    public class NextEventProcessor:AbstractIntentProcessor
    {
        private IEnvironment Environment { get; }
        private IS3Client Client { get; }

        public NextEventProcessor(IEnvironment env, IS3Client client)
        {
            Environment = env;
            Client = client;
        }

        public override async Task<LexResponse> ProcessAsync(LexEvent lexEvent, ILambdaContext context)
        {
            var technotts = TechNottsEvent.TechNottingham;
            var meetup = (await GetNextEventData(string.Empty)).FirstOrDefault();

            var sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();
            if (meetup == null)
            {
                
                return this.Close(sessionAttributes, "Fulfilled", null);
            }

            var closer = this.Close(sessionAttributes, "Fulfilled", ContentCreation.NextEvent(technotts, meetup, Environment.CurrentTime, false));
            closer.DialogAction.ResponseCard = ContentCreation.NextEventCard(technotts);
            return closer;

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
