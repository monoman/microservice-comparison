var app = WebApplication.CreateBuilder(args)
                        .ConfigureHttpJsonOptions()
                        .Build();
app.MapGet("/", async () => Results.Ok(await "https://www.reddit.com/r/politics/hot.json".GetTitlesAsync()));
app.Run("http://localhost:8080");

public static class Code {
    public record Listing(Listing.ListingData Data) {
        public record ListingData(ListingData.Article[] Children) {
            public record Article(Article.ArticleData Data) {
                public record ArticleData(string Title);
            }
        }
    }
    public static WebApplicationBuilder ConfigureHttpJsonOptions(this WebApplicationBuilder builder) {
        builder.Services.ConfigureHttpJsonOptions(o => {
            o.SerializerOptions.WriteIndented = _options.WriteIndented;
            o.SerializerOptions.PropertyNameCaseInsensitive = _options.PropertyNameCaseInsensitive;
        });
        return builder;
    }
    private static async Task<Stream> GetStreamAsync(this string requestUri) => 
        await new HttpClient().GetStreamAsync(requestUri);
    private static async Task<Listing?> GetListingAsync(this Task<Stream> getStreamAsync) =>
        await JsonSerializer.DeserializeAsync<Listing>(await getStreamAsync, _options);
    public static async Task<Listing.ListingData.Article.ArticleData[]> GetTitlesAsync(this string requestUri) =>
        (await requestUri.GetStreamAsync().GetListingAsync())?.Data?.Children?.Select(x => x.Data).ToArray() ?? _empty;
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    private static readonly Listing.ListingData.Article.ArticleData[] _empty = Array.Empty<Listing.ListingData.Article.ArticleData>();
}

