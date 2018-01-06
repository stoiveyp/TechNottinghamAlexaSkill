using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TechNottinghamGoogleAction
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var response = new JObject(new JProperty("fulfillmentText","really real wibble"));
            //var response = new JObject(new JProperty("data",
            //    new JObject(
            //        new JProperty("google",new JObject(
            //            new JProperty("expectUserResponse",false),
            //            new JProperty("isSsml",false),
            //            new JProperty("textToSpeech","webhook response awesomeness!")
            //            ))
            //    )));

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = response.ToString()
            };
        }
    }
}
