using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Amazon.Lambda.LexEvents;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Amazon.S3;
using TechNottingham.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamLex
{
    public class Function
    {
        public const string BucketVariableName = "bucketId";

        /// <summary>
        /// Then entry point for the Lambda function that looks at the current intent and calls 
        /// the appropriate intent process.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<LexResponse> FunctionHandler(LexEvent lexEvent, ILambdaContext context)
        {
            IIntentProcessor process;

            switch (lexEvent.CurrentIntent.Name.ToLower())
            {
                case "missionstatement":
                    process = new MissionStatementProcessor();
                    break;
                case "nextevent":
                        process = new NextEventProcessor(new DefaultEnvironment(), new S3ClientWrapper(Environment.GetEnvironmentVariable(BucketVariableName),new AmazonS3Client()));
                    break;
                default:
                    throw new Exception($"Intent with name {lexEvent.CurrentIntent.Name} not supported");
            }


            return process.ProcessAsync(lexEvent, context);
        }

    }
}
