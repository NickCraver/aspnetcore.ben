using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace AspNetCore.Ben
{
    public class HttpListener : IDisposable
    {
        IWebHost _webHost;
        Task _serverTask;
        ConcurrentQueue<HttpListenerContext> _requestQueue = new ConcurrentQueue<HttpListenerContext>();
        SemaphoreSlim _requests = new SemaphoreSlim(0);

        public List<string> Prefixes { get; } = new List<string>();

        public void Start()
        {
            _webHost = WebHost.CreateDefaultBuilder()
                            .UseStartup<HttpServer>()
                            .UseUrls(Prefixes.ToArray())
                            .ConfigureServices((builderContext, services) => {
                                services.AddSingleton(this);
                            })
                            .Build();

            _serverTask = _webHost.RunAsync();
        }

        internal void QueueRequest(HttpListenerContext listenerContext)
        {
            _requestQueue.Enqueue(listenerContext);
            _requests.Release();
        }

        public async Task<HttpListenerContext> GetContextAsync()
        {
            await _requests.WaitAsync();
            _requestQueue.TryDequeue(out var context);
            return context;
        }

        public void Stop() => _webHost.StopAsync().Wait();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
