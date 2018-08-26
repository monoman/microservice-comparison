using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace microservice_comparison
{
    public class Program
    {
        public static void Main(string[] args) => WebHost
            .Start("http://localhost:3000", async context => await context.Response.WriteAsync(SerializeObject(Map(await GetData()))))
            .WaitForShutdown();

        private static async Task<object> GetData()
            => DeserializeObject(await new HttpClient().GetStringAsync("https://www.reddit.com/r/politics/hot.json"));

        private static IEnumerable Map(dynamic reddit)
        {
            foreach (var children in reddit.data.children)
                yield return new { children.data.title };
        }
    }
}
