using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute.Routing.Handlers;

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
