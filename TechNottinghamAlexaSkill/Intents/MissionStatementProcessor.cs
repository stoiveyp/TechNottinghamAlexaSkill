using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Response;

namespace TechNottinghamAlexaSkill.Intents
{
    class MissionStatementProcessor
    {
        public static Task<SkillResponse> Process()
        {
            var response = ResponseBuilder.Tell(ContentCreation.MissionStatement);
            response.Response.Card = ContentCreation.MissionStatementCard;
            return Task.FromResult(response);
        }
    }
}
