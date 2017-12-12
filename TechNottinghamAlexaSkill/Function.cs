using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Amazon.S3;
using TechNottingham.Common;

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
                case IntentNames.MissionStatement:
                    return MissionStatement();
                case IntentNames.NextEvent:
                    return NextEvent(intent.Intent);
            }

            return Task.FromResult(ResponseBuilder.Empty());
        }

        private async Task<SkillResponse> NextEvent(Intent intent)
        {
            var meetups = await GetNextEventData();

            if (meetups.Length > 0)
            {
                return ResponseBuilder.Tell(PhraseList.NextEvent(meetups.First(),Environment.CurrentTime));
                //NodaTime.Calendars.WeekYearRules.FromCalendarWeekRule(CalendarWeekRule.FirstFourDayWeek,DayOfWeek.Monday).GetLocalDate()
            }
            
            return ResponseBuilder.Tell(PhraseList.NoNextEvent);
        }

        private Task<MeetupEvent[]> GetNextEventData()
        {
            return S3Client.GetEventData(S3Keys.EventData);
        }

        private Task<SkillResponse> MissionStatement()
        {
            return Task.FromResult(ResponseBuilder.Tell(PhraseList.MissionStatement));
        }
    }
}
