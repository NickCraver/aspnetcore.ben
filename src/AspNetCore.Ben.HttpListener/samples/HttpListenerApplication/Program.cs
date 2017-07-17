using System.Threading.Tasks;
using AspNetCore.Ben;

namespace HttpListenerApplication
{
    public class Program
    {
        public static async Task MainAsync()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000");
            listener.Start();

            while (true)
            {
                using (var context = await listener.GetContextAsync())
                {
                    var input = await context.Request.ReadInputStringAsync();

                    await context.Response.WriteOutputStringAsync("Hello World");
                }
            }
        }

        public static void Main(string[] args) => MainAsync().Wait();
    }
}
