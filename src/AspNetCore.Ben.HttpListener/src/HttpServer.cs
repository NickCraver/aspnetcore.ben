using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Ben
{
    internal class HttpServer
    {
        public void Configure(IApplicationBuilder app) => app.Run((context) => HandleRequest(context));

        private static Task HandleRequest(HttpContext context)
        {
            var listenerContext = new HttpListenerContext(context);
            var listener = context.RequestServices.GetRequiredService<HttpListener>();

            listener.QueueRequest(listenerContext);

            return listenerContext.Task;
        }
    }
}
