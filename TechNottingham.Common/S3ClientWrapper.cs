using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace TechNottingham.Common
{
    public class S3ClientWrapper : IS3Client
    {
        private AmazonS3Client Inner { get; }
        private string BucketId { get; }

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

        public async Task<Stream> GetData(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = BucketId,
                Key = key
            };

            var response = await Inner.GetObjectAsync(request);
            return response.ResponseStream;
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
