using System;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;
using TechNottingham.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamCron
{
    public class Function
    {
        public const string BucketVariableName = "bucketId";
        public const string DataVariableName = "meetupDataUrl";
        public const string EventDataKey = "eventdata";

        public IS3Client S3Client { get; set; }
        public IEnvironment Environment { get; set; }
        public HttpMessageHandler Handler { get; set; }

        public async Task FunctionHandler(ILambdaContext context)
        {
            //Coalesce lambda to allow unit testing
            var environment = Environment ?? new DefaultEnvironment();
            var s3 = S3Client ?? new S3ClientWrapper(environment.Get(BucketVariableName), new AmazonS3Client());

            var lastModified = await s3.EventDataModifiedOn(EventDataKey);

            if (lastModified.HasValue && DateTime.UtcNow.Subtract(lastModified.Value.ToUniversalTime()).TotalMinutes <= 5)
            {
                return;
            }

            var content = await NextEvents(environment.Get(DataVariableName));
            await s3.SaveData(EventDataKey, content);
        }

        private Task<string> NextEvents(string meetupUrl)
        {
            var client = Handler == null ? new HttpClient() : new HttpClient(Handler);
            return client.GetStringAsync(new Uri(meetupUrl, UriKind.Absolute));
        }
    }
}
