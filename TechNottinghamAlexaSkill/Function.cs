using System;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamAlexaSkill
{
    public class Function
    {
        
        public Task<SkillResponse> FunctionHandler(SkillRequest input)
        {
            try
            {
                return InnerHandler(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult(ResponseBuilder.Tell(PhraseList.ErrorText));
            }
        }

        private Task<SkillResponse> InnerHandler(SkillRequest input)
        {
            switch (input.Request)
            {
                case LaunchRequest _:
                    return Launch();
                case IntentRequest intent:
                    return Handle(intent);
            }

            return Task.FromResult(ResponseBuilder.Empty());
        }

        private static Task<SkillResponse> Launch()
        {
            return Task.FromResult(ResponseBuilder.Ask(PhraseList.WelcomeText, null));
        }

        private Task<SkillResponse> Handle(IntentRequest intent)
        {
            switch (intent.Intent.Name)
            {
                case "MissionStatement":
                    return MissionStatement();
            }

            return Task.FromResult(ResponseBuilder.Empty());
        }

        private Task<SkillResponse> MissionStatement()
        {
            return Task.FromResult(ResponseBuilder.Tell(PhraseList.MissionStatement));
        }
    }
}
