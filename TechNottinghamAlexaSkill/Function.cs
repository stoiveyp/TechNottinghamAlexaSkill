using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Amazon.S3;
using Newtonsoft.Json.Linq;
using TechNottingham.Common;
using TechNottinghamAlexaSkill.Processors;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamAlexaSkill
{
    public class Function
    {
        public const string BucketVariableName = "bucketId";
        public IEnvironment Environment { get; set; }
        public IS3Client S3Client { get; set; }

        public Function(IEnvironment environment, IS3Client client)
        {
            Environment = environment;
            S3Client = client;
        }

        public Function()
        {
            Environment = new DefaultEnvironment();
            S3Client = new S3ClientWrapper(Environment.Get(BucketVariableName), new AmazonS3Client());
        }

        public Task<SkillResponse> FunctionHandler(SkillRequest input)
        {
            try
            {
                return InnerHandler(input);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return Task.FromResult(ResponseBuilder.Tell(ContentCreation.ErrorText));
            }
        }

        private Task<SkillResponse> InnerHandler(SkillRequest input)
        {
            switch (input.Request)
            {
                case LaunchRequest _:
                    return Launch();
                case IntentRequest intent:
                    return Intent(input,intent);
            }

            return Task.FromResult(ResponseBuilder.Empty());
        }

        private static Task<SkillResponse> Launch()
        {
            Console.WriteLine("Launch");
            return Task.FromResult(ResponseBuilder.Ask(ContentCreation.WelcomeText, null));
        }

        private Task<SkillResponse> Intent(SkillRequest request, IntentRequest intent)
        {
            Console.WriteLine(intent.Intent.Name);
            switch (intent.Intent.Name)
            {
                case IntentNames.MissionStatement:
                    return MissionStatementProcessor.Process();
                case IntentNames.NextEvent:
                    return new NextEventProcessor(Environment, S3Client).Process();
                case IntentNames.NextSpecificEvent:
                    return new NextEventProcessor(Environment,S3Client).ProcessSpecific(intent.Intent);
                case BuiltInIntent.Help:
                    return HelpText();
            }

            return Task.FromResult(ResponseBuilder.Empty());
        }

        private Task<SkillResponse> HelpText()
        {
            return Task.FromResult(ResponseBuilder.Tell(ContentCreation.HelpText));
        }
    }
}
