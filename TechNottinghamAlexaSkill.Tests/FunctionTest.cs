using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json.Linq;
using TechNottinghamAlexaSkill;

namespace TechNottinghamAlexaSkill.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task StopReturnsEmptyResponse()
        {
            var function = new Function();
            var intent = GetIntent("AMAZON.StopIntent");

            var expected = ResponseBuilder.Empty();
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected,actual));
        }

        [Fact]
        public async Task CancelReturnsEmptyResponse()
        {
            var function = new Function();
            var intent = GetIntent("AMAZON.CancelIntent");

            var expected = ResponseBuilder.Empty();
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        [Fact]
        public async Task LaunchReturnsWelcomeText()
        {
            var function = new Function();
            var intent = new SkillRequest {Request = new LaunchRequest()};

            var expected = ResponseBuilder.Ask(PhraseList.WelcomeText,null);
            var actual = await function.FunctionHandler(intent);

            Assert.True(CompareJson(expected, actual));
        }

        public static bool CompareJson(object expected, object actual)
        {
            var actualJObject = JObject.FromObject(actual);
            var expectedJObject = JObject.FromObject(expected);

            return JToken.DeepEquals(expectedJObject, actualJObject);
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
