using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TechNottinghamCron.Tests
{
    public class ActionMessageHandler:HttpMessageHandler
    {
        protected Func<HttpRequestMessage, Task<HttpResponseMessage>> Action { get; }

        public ActionMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> action)
        {
            Action = req => Task.FromResult(action(req));
        }

        public ActionMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> action)
        {
            Action = action;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Action(request);
        }
    }
}
