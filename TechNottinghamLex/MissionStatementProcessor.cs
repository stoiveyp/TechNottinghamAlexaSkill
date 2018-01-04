using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using TechNottingham.Common;

namespace TechNottinghamLex
{
    public class MissionStatementProcessor : AbstractIntentProcessor
    {
        public override Task<LexResponse> ProcessAsync(LexEvent lexEvent, ILambdaContext context)
        {
            var sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();
            return Task.FromResult(this.Close(sessionAttributes, "Fulfilled",
                new LexResponse.LexMessage
                {
                    Content = ContentCreation.MissionStatement,
                    ContentType = MESSAGE_CONTENT_TYPE
                }));
        }
    }
}