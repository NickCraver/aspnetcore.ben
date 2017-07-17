using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HttpListenerApplication
{
    public static class HttpContextExtensions
    {
        public static async Task<string> ReadInputStringAsync(this HttpRequest request)
        {
            string result;
            using (var reader = new StreamReader(request.Body))
            {
                result = await reader.ReadToEndAsync();
            }
            return result;
        }

        public static async Task WriteOutputStringAsync(this HttpResponse response, string text)
        {
            using (var writer = new StreamWriter(response.Body))
            {
                await writer.WriteAsync(text);
            }
        }
    }
}
