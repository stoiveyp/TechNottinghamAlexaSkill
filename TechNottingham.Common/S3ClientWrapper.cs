using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TechNottingham.Common
{
    public class S3ClientWrapper : IS3Client
    {
        private AmazonS3Client Inner { get; }
        private string BucketId { get; }

        private static JsonSerializer Serializer = JsonSerializer.CreateDefault();

        public S3ClientWrapper(string bucket, AmazonS3Client amazonS3Client)
        {
            Inner = amazonS3Client;
            BucketId = bucket;
        }

        public async Task<DateTime?> EventDataModifiedOn(string key)
        {
            try
            {
                var data = await Inner.GetObjectMetadataAsync(BucketId, key);
                return data?.LastModified;
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }

        public async Task<MeetupEvent[]> GetEventData(string key)
        {
            var content = await ReadFileAsStringAsync(key);
            return Serializer.Deserialize<MeetupEvent[]>(new JsonTextReader(new StringReader(content)));
        }

        public async Task<string> ReadFileAsStringAsync(string fileKey)
        {
                var response = await Inner.GetObjectAsync(BucketId, fileKey);
                using (response)
                {
                    using (var reader = new StreamReader(response.ResponseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
        }


        public Task SaveData(string key, string content)
        {
            var request = new PutObjectRequest
            {
                BucketName = BucketId,
                Key = key,
                ContentBody = content
            };
            return Inner.PutObjectAsync(request);
        }
    }
}
