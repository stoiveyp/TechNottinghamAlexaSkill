using System;
using System.Collections.Generic;
using System.Linq;
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
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<SkillResponse> FunctionHandler(SkillRequest input)
        {
            switch (input.Request)
            {
                case LaunchRequest launch:
                    return Handle(launch);
                case IntentRequest intent:
                    return Handle(intent);
            }

            return Task.FromResult(ResponseBuilder.Empty());
        }

        private Task<SkillResponse> Handle(LaunchRequest _)
        {
            return Task.FromResult(ResponseBuilder.Ask(PhraseList.WelcomeText, null));
        }

        private Task<SkillResponse> Handle(IntentRequest intent)
        {
            switch (intent.Intent.Name)
            {

            }

            return Task.FromResult(ResponseBuilder.Empty());
        }
    }
}
