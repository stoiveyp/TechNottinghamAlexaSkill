using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Xunit;
using Newtonsoft.Json.Linq;
using NSubstitute;
using TechNottingham.Common;

namespace TechNottinghamAlexaSkill.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task StopReturnsEmptyResponse()
        {
            var function = DefaultFunction();
            var intent = GetIntent("AMAZON.StopIntent");

            var expected = ResponseBuilder.Empty();
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task CancelReturnsEmptyResponse()
        {
            var function = DefaultFunction();
            var intent = GetIntent("AMAZON.CancelIntent");

            var expected = ResponseBuilder.Empty();
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task LaunchReturnsWelcomeText()
        {
            var function = DefaultFunction();
            var intent = new SkillRequest {Request = new LaunchRequest()};

            var expected = ResponseBuilder.Ask(PhraseList.WelcomeText, null);
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task MissionStatementReturnsText()
        {
            var function =DefaultFunction();
            var intent = GetIntent(IntentNames.MissionStatement);

            var expected = ResponseBuilder.Tell(PhraseList.MissionStatement);
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task NextEventGetsS3Data()
        {
            var s3 = GetS3();
            var function =new Function(GetEnvironment(),s3);
            var intent = GetIntent(IntentNames.NextEvent);

            await function.FunctionHandler(intent);

            await s3.Received(1).GetEventData(S3Keys.EventData);
        }

        [Fact]
        public async Task NextEventReturnsNoEventsTextWithZeroEvents()
        {
            var environment = GetEnvironment();
            var s3 = Substitute.For<IS3Client>();
            s3.GetEventData(S3Keys.EventData).Returns(new MeetupEvent[] { });

            var function = new Function(environment, s3);

            var intent = GetIntent(IntentNames.NextEvent);

            var expected = ResponseBuilder.Tell(PhraseList.NoNextEvent);
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task NextEventReturnsEventsTextWithAtLeastOneEvent()
        {
            var s3 = GetS3();
            var events = new []
            {
                new MeetupEvent
                {
                    name = "test",
                    venue = new Venue {name = "antenna"},
                    local_date = DateTime.UtcNow.ToString("O")
                }
            };

            s3.GetEventData(S3Keys.EventData).Returns(events);

            var environment = GetEnvironment();
            var function = new Function(environment,s3);
            var intent = GetIntent(IntentNames.NextEvent);

            var expected = ResponseBuilder.Tell(PhraseList.NextEvent(events.First(),environment.CurrentTime));
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected,actual));
        }

        public static bool CompareJson(object expected, object actual)
        {
            var actualJObject = JObject.FromObject(actual);
            var expectedJObject = JObject.FromObject(expected);

            return JToken.DeepEquals(expectedJObject, actualJObject);
        }


        private Function DefaultFunction()
        {
            return new Function(GetEnvironment(),GetS3());
        }

        private IEnvironment GetEnvironment()
        {
            var local = DateTime.Now;
            var env = Substitute.For<IEnvironment>();
            env.CurrentTime.Returns(local);
            env.Get(Function.BucketVariableName).Returns("test");
            return env;
        }

        private IS3Client GetS3()
        {
            var s3 = Substitute.For<IS3Client>();
            s3.GetEventData(S3Keys.EventData).Returns(new MeetupEvent[] { });
            return s3;
        }

        private SkillRequest GetIntent(string intentName)
        {
            return new SkillRequest
            {
                Request = new IntentRequest { Intent = new Intent { Name = intentName } }
            };
        }
    }
}
