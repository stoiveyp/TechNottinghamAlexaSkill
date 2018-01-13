using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Google.Apis.Dialogflow.v2beta1.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechNottingham.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamGoogleAction
{
    public partial class Function
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

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var request = GetRequest(input);
            var response = await GetResponse(request);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JObject.FromObject(response).ToString()
            };
        }

        private async Task<ResponseLite> GetResponse(WebhookRequest request)
        {
            try
            {
                var result = "Sorry - not quite sure what you asked for there.";
                switch (request.QueryResult.Intent.DisplayName)
                {
                    case "SpecificEvent":
                        return await new NextEventProcessor(Environment, S3Client).ProcessSpecific(request.QueryResult);
                    case "GeneralEvent":
                        return await new NextEventProcessor(Environment, S3Client).Process();
                    default:
                        return ResponseLite.AsText(ContentCreation.HelpText);
                }
            }
            catch (Exception)
            {
                return ResponseLite.AsText(ContentCreation.ErrorText);
            }
        }

        private static readonly JsonSerializer Serializer = JsonSerializer.CreateDefault();

        private WebhookRequest GetRequest(APIGatewayProxyRequest request)
        {
            using (var reader = new JsonTextReader(new StringReader(request.Body)))
            {
                return Serializer.Deserialize<WebhookRequest>(reader);
            }
        }
    }
}