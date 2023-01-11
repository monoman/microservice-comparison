var app = WebApplication.CreateBuilder(args).Build();
app.MapGet("/", Code.GetRedditHotPoliticsTitlesAsync);
app.Run("http://localhost:8080");

public static class Code {
    public static async Task GetRedditHotPoliticsTitlesAsync(HttpContext context) =>
        await context.Response.WriteAsJsonAsync(await "https://www.reddit.com/r/politics/hot.json".GetTitlesAsync(), Options);
    public record Listing(Listing.ListingData Data) {
        public record ListingData(ListingData.Article[] Children) {
            public record Article(Article.ArticleData Data) {
                public record ArticleData(string Title);
            }
        }
    }
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };
    private static async Task<IEnumerable<Listing.ListingData.Article.ArticleData>> GetTitlesAsync(this string requestUri) {
        Listing? listing = await JsonSerializer.DeserializeAsync<Listing>(await new HttpClient().GetStreamAsync(requestUri), Options);
        return listing is null ? Enumerable.Empty<Listing.ListingData.Article.ArticleData>() : listing.Data.Children.Select(x => x.Data);
    }
}
