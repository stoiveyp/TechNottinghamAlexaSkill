using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.TestUtilities;
using NSubstitute;
using TechNottingham.Common;

namespace TechNottinghamCron.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task ExecutionChecksForS3FileModifiedDateTime()
        {
            var function = new Function();
            var context = new TestLambdaContext();

            var s3 = Substitute.For<IS3Client>();
            s3.EventDataModifiedOn(Function.EventDataKey).Returns(DateTime.UtcNow);
            function.S3Client = s3;
            function.Environment = GetEnvironment();
            function.Handler = new ActionMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK));
            await function.FunctionHandler(context);

            await s3.Received(1).EventDataModifiedOn(Function.EventDataKey);
        }

        [Fact]
        public async Task ExecutionCheckCallsClientIfNoFileExists()
        {
            var function = new Function();
            var context = new TestLambdaContext();

            var env = GetEnvironment();
            function.Environment = env;

            var s3 = Substitute.For<IS3Client>();
            s3.EventDataModifiedOn(Function.EventDataKey).Returns(new DateTime?());
            s3.SaveData(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult((object) null));
            function.S3Client = s3;

            var correctRequest = false;
            var handler = new ActionMessageHandler(req =>
            {
                if (req.RequestUri.AbsolutePath == "/randompath")
                {
                    correctRequest = true;
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            });
            function.Handler = handler;

            await function.FunctionHandler(context);
            Assert.True(correctRequest);
        }

        [Fact]
        public async Task ExecutionCheckCallsClientIfFileIsMoreThanFiveMinutesOld()
        {
            var function = new Function();
            var context = new TestLambdaContext();

            var env = GetEnvironment();
            function.Environment = env;

            var s3 = Substitute.For<IS3Client>();
            s3.EventDataModifiedOn(Function.EventDataKey).Returns(DateTime.UtcNow.AddMinutes(-11));
            s3.SaveData(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult((object)null));
            function.S3Client = s3;

            var correctRequest = false;
            var handler = new ActionMessageHandler(req =>
            {
                correctRequest = true;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });
            function.Handler = handler;

            await function.FunctionHandler(context);
            Assert.True(correctRequest);
        }

        [Fact]
        public async Task ExecutionCheckCallsClientIfFileIsLessThanFiveMinutesOld()
        {
            var function = new Function();
            var context = new TestLambdaContext();

            var env = GetEnvironment();
            function.Environment = env;

            var s3 = Substitute.For<IS3Client>();
            s3.EventDataModifiedOn(Function.EventDataKey).Returns(DateTime.UtcNow.AddMinutes(-3));
            s3.SaveData(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult((object)null));
            function.S3Client = s3;

            var correctRequest = true;
            var handler = new ActionMessageHandler(req =>
            {
                correctRequest = false;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });
            function.Handler = handler;

            await function.FunctionHandler(context);
            Assert.True(correctRequest);
        }

        private IEnvironment GetEnvironment()
        {
            var env = Substitute.For<IEnvironment>();
            env.Get(Function.BucketVariableName).Returns("test");
            env.Get(Function.DataVariableName).Returns("https://www.meetup.com/randompath");
            return env;
        }
    }
}
