using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;

namespace TechNottinghamLex
{
    public class MissionStatementProcessor : AbstractIntentProcessor
    {
        public override LexResponse Process(LexEvent lexEvent, ILambdaContext context)
        {
            var sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();
            return this.Close(sessionAttributes, "Fulfilled",
                new LexResponse.LexMessage
                {
                    Content = "mission statement here",
                    ContentType = MESSAGE_CONTENT_TYPE
                });
        }
    }
}