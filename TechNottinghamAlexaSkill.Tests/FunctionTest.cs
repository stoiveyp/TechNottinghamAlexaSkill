using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Newtonsoft.Json;
using Xunit;
using Newtonsoft.Json.Linq;
using NodaTime.Text;
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
            var intent = GetIntent(BuiltInIntent.Stop);

            var expected = ResponseBuilder.Empty();
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task CancelReturnsEmptyResponse()
        {
            var function = DefaultFunction();
            var intent = GetIntent(BuiltInIntent.Cancel);

            var expected = ResponseBuilder.Empty();
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task LaunchReturnsWelcomeText()
        {
            var function = DefaultFunction();
            var intent = new SkillRequest {Request = new LaunchRequest()};

            var expected = ResponseBuilder.Ask(ContentCreation.WelcomeText, null);
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task MissionStatementReturnsText()
        {
            var function =DefaultFunction();
            var intent = GetIntent(IntentNames.MissionStatement);

            var expected = ResponseBuilder.Tell(ContentCreation.MissionStatement);
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
            var technott = TechNottsEvent.Empty;
            s3.GetEventData(S3Keys.EventData).Returns(new MeetupEvent[] { });

            var function = new Function(environment, s3);

            var intent = GetIntent(IntentNames.NextEvent);

            var expected = ResponseBuilder.Tell(ContentCreation.NoNextEvent(technott));
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

            var expected = ResponseBuilder.Tell(ContentCreation.NextEvent(TechNottsEvent.Empty, events.First(),environment.CurrentTime));
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected,actual));
        }

        [Fact]
        public async Task EnsureHelpReturnsText()
        {
            var function = DefaultFunction();
            var intent = GetIntent(BuiltInIntent.Help);

            var expected = ResponseBuilder.Tell(ContentCreation.HelpText);
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task GetSpecificEvent()
        {
            var s3 = GetS3();
            var environment = GetEnvironment();
            var meetups = ExampleFileContent<MeetupEvent[]>("eventdata_tech-nottingham.json");
            s3.GetEventData(S3Keys.EventData + "_" + "tech-nottingham").Returns(meetups);
                var request = ExampleFileContent<SkillRequest>("TechOnToast.json");
            var function = new Function(environment,s3);
            var result = await function.FunctionHandler(request);

            var expected = ContentCreation.NextEvent(TechNottsEvent.EventList["tech on toast"],meetups.Skip(1).First(),environment.CurrentTime);
            Assert.Equal(expected.ToXml(), ((SsmlOutputSpeech) result.Response.OutputSpeech).Ssml);
            //    ;            var weekResult = LocalDatePattern.Iso.Parse("2015-W49");
            //Console.WriteLine(weekResult);
        }

        public static bool CompareJson(object expected, object actual)
        {
            var actualJObject = JObject.FromObject(actual);
            var expectedJObject = JObject.FromObject(expected);

            return JToken.DeepEquals(expectedJObject, actualJObject);
        }

        public static T ExampleFileContent<T>(string expectedFile)
        {
            using (var reader = new JsonTextReader(new StringReader(ExampleFileContent(expectedFile))))
            {
                return new JsonSerializer().Deserialize<T>(reader);
            }

        }

        public static string ExampleFileContent(string expectedFile)
        {
            return File.ReadAllText(Path.Combine(".", expectedFile));
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
