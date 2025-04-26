var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<RedditClient>()
    .ConfigureHttpJsonOptions(o => o.SerializerOptions.WriteIndented = true);
var app = builder.Build();
app.MapGet("/", () => Results.Redirect("/politics"));
app.MapGet("/{sectionName}", async (string sectionName, RedditClient client) => Results.Ok(await client.GetSectionTopListAsync(sectionName)));
app.Run("http://localhost:8080");

public class RedditClient : HttpClient
{
    public RedditClient()
    {
        BaseAddress = new Uri("https://www.reddit.com/");
        DefaultRequestHeaders.Add("user-agent", "microservice-comparison-csharp");
    }
    public record ArticleData(string Title);
    public record Article(ArticleData Data);
    public record ListingData(Article[] Children);
    public record Listing(ListingData Data);
    public async Task<IEnumerable<ArticleData>> GetSectionTopListAsync(string section) => (await this.GetFromJsonAsync<Listing>($"r/{section}/hot.json"))?.Data?.Children?.Select(x => x.Data) ?? [];
}
