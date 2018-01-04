using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Response;
using TechNottingham.Common;

namespace TechNottinghamAlexaSkill.Processors
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
