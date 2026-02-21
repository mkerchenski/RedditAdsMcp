using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RedditAdsMcp.Auth;

namespace RedditAdsMcp.Client;

/// <summary>
/// Thin HTTP wrapper for the Reddit Ads API v3.
/// Injects bearer token from RedditAuthService on every request.
/// </summary>
public sealed class RedditAdsClient
{
    private const string BaseUrl = "https://ads-api.reddit.com/api/v3";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly RedditAuthService _auth;
    private readonly ILogger<RedditAdsClient> _logger;

    public RedditAdsClient(HttpClient http, RedditAuthService auth, ILogger<RedditAdsClient> logger)
    {
        _http = http;
        _auth = auth;
        _logger = logger;
        _http.BaseAddress = new Uri(BaseUrl);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("reddit-ads-mcp-csharp/1.0");
    }

    /// <summary>Resolve accountId with fallback to the default from env.</summary>
    public string ResolveAccountId(string? accountId) =>
        accountId ?? _auth.DefaultAccountId;

    public async Task<JsonDocument> GetAsync(string path, CancellationToken ct = default)
    {
        await SetAuthHeaderAsync(ct);
        _logger.LogDebug("GET {Path}", path);

        HttpResponseMessage response = await _http.GetAsync(path, ct);
        await EnsureSuccessAsync(response, ct);

        return await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
    }

    public async Task<JsonDocument> PostAsync(string path, object body, CancellationToken ct = default)
    {
        await SetAuthHeaderAsync(ct);
        _logger.LogDebug("POST {Path}", path);

        string json = JsonSerializer.Serialize(body, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _http.PostAsync(path, content, ct);
        await EnsureSuccessAsync(response, ct);

        return await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
    }

    private async Task SetAuthHeaderAsync(CancellationToken ct)
    {
        string token = await _auth.GetAccessTokenAsync(ct);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Reddit API error {Status}: {Body}", (int)response.StatusCode, body);
            throw new HttpRequestException(
                $"Reddit API returned {(int)response.StatusCode}: {body}");
        }
    }
}
