using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class Program
{
    public static void Main(string[] args)
    {
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Program>()
            .Build()
            .Run(); // or RunAsync();
    }

    // This method gets called by the runtime, to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        // Add handler to the HTTP request pipeline.
        app.Run(HandleRequest);
    }

    static byte[] data = System.Text.Encoding.ASCII.GetBytes("Hello");

    private async static Task HandleRequest(HttpContext context)
    {
        // Handle HTTP request
        await context.Response.Body.WriteAsync(data, 0, data.Length);
    }
}
