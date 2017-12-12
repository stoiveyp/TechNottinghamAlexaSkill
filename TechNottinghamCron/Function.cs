using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;
using Newtonsoft.Json;
using TechNottingham.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamCron
{
    public class Function
    {
        public const string BucketVariableName = "bucketId";
        public const string MeetupUrlVariableName = "meetupDataUrl";
        public const string MeetupsVariableName = "meetups";

        public IS3Client S3Client { get; set; }
        public IEnvironment Environment { get; set; }
        public HttpMessageHandler Handler { get; set; }

        public async Task FunctionHandler(ILambdaContext context)
        {
            //Coalesce lambda to allow unit testing
            var environment = Environment ?? new DefaultEnvironment();
            var s3 = S3Client ?? new S3ClientWrapper(environment.Get(BucketVariableName), new AmazonS3Client());

            var lastModified = await s3.EventDataModifiedOn(S3Keys.EventData);

            if (lastModified.HasValue && DateTime.UtcNow.Subtract(lastModified.Value.ToUniversalTime()).TotalMinutes <= 5)
            {
                return;
            }

            var list = new List<MeetupEvent>();
            var meetupUrl = environment.Get(MeetupUrlVariableName);
            foreach (var meetup in environment.Get(MeetupsVariableName).Split(',').Select(s => s.ToLower()))
            {
                var events = await NextEvents(string.Format(meetupUrl, meetup)) ?? new MeetupEvent[]{};
                list.AddRange(events);
                await SaveEvents(s3, S3Keys.EventData + "_" + meetup, events);
            }

            await SaveEvents(s3, S3Keys.EventData, list.ToArray());
        }

        private async Task SaveEvents(IS3Client client, string key, MeetupEvent[] content)
        {
            var osb = new StringBuilder();
            using (var sw = new JsonTextWriter(new StringWriter(osb)))
            {
                JsonSerializer.Create().Serialize(sw,content);
                await client.SaveData(key,osb.ToString());
            }
        }

        private async Task<MeetupEvent[]> NextEvents(string meetupUrl)
        {
            var client = Handler == null ? new HttpClient() : new HttpClient(Handler);
            var response = await client.GetStringAsync(new Uri(meetupUrl, UriKind.Absolute));
            return JsonSerializer.Create().Deserialize<MeetupEvent[]>(new JsonTextReader(new StringReader(response)));
        }
    }
}
