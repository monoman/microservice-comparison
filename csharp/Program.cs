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
        DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
    }
    public record ArticleData(string Title);
    public record Article(ArticleData Data);
    public record ListingData(Article[] Children);
    public record Listing(ListingData Data);
    public async Task<IEnumerable<ArticleData>> GetSectionTopListAsync(string section) => (await this.GetFromJsonAsync<Listing>($"r/{section}/hot.json"))?.Data?.Children?.Select(x => x.Data) ?? [];
}
