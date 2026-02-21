using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RedditAdsMcp.Auth;

/// <summary>
/// Manages Reddit OAuth 2.0 token lifecycle with automatic refresh.
/// Reads credentials from environment variables:
///   REDDIT_CLIENT_ID, REDDIT_CLIENT_SECRET, REDDIT_REFRESH_TOKEN, REDDIT_ACCOUNT_ID
/// </summary>
public sealed class RedditAuthService
{
    private const string TokenUrl = "https://www.reddit.com/api/v1/access_token";
    private static readonly TimeSpan RefreshBuffer = TimeSpan.FromSeconds(60);

    private readonly ILogger<RedditAuthService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _refreshToken;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _accessToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public string DefaultAccountId { get; }

    public RedditAuthService(ILogger<RedditAuthService> logger)
    {
        _logger = logger;
        _clientId = GetRequiredEnv("REDDIT_CLIENT_ID");
        _clientSecret = GetRequiredEnv("REDDIT_CLIENT_SECRET");
        _refreshToken = GetRequiredEnv("REDDIT_REFRESH_TOKEN");
        DefaultAccountId = GetRequiredEnv("REDDIT_ACCOUNT_ID");

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("reddit-ads-mcp-csharp/1.0");
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        // fast path — token still valid
        if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt - RefreshBuffer)
            return _accessToken;

        await _semaphore.WaitAsync(ct);
        try
        {
            // double-check after acquiring lock
            if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt - RefreshBuffer)
                return _accessToken;

            _logger.LogInformation("Refreshing Reddit access token");

            string credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _refreshToken
            });

            HttpResponseMessage response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            using JsonDocument doc = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

            _accessToken = doc.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("No access_token in response");

            int expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            _logger.LogInformation("Token refreshed, expires in {Seconds}s", expiresIn);
            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static string GetRequiredEnv(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException(
            $"Missing required environment variable: {name}");
}
