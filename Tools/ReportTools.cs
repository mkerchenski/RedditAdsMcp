using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using RedditAdsMcp.Client;
using RedditAdsMcp.Models;

namespace RedditAdsMcp.Tools;

[McpServerToolType]
public static class ReportTools
{
    private static readonly string[] DefaultFields =
        ["IMPRESSIONS", "CLICKS", "SPEND", "CTR", "CPC", "ECPM"];

    [McpServerTool, Description(
        "Get a performance report for a Reddit ad account. " +
        "Returns fields like impressions, clicks, spend, CTR, CPC, and eCPM " +
        "broken down by the specified breakdowns.")]
    public static async Task<string> GetPerformanceReport(
        RedditAdsClient client,
        [Description("Start date in YYYY-MM-DD format")]
        string startDate,
        [Description("End date in YYYY-MM-DD format")]
        string endDate,
        [Description("Reddit ad account ID (optional, defaults to REDDIT_ACCOUNT_ID env var)")]
        string? accountId = null,
        [Description("Fields to include (e.g. IMPRESSIONS, CLICKS, SPEND, CTR, CPC, ECPM). Defaults to all common fields.")]
        string[]? fields = null,
        [Description("Breakdowns for the report (e.g. DATE, CAMPAIGN_ID, AD_GROUP_ID). Defaults to [DATE].")]
        string[]? breakdowns = null,
        CancellationToken ct = default)
    {
        string id = client.ResolveAccountId(accountId);
        ReportRequest request = new()
        {
            StartsAt = NormalizeDate(startDate),
            EndsAt = NormalizeDate(endDate),
            Fields = fields ?? DefaultFields,
            Breakdowns = breakdowns ?? ["DATE"]
        };

        var body = new { data = request };
        using JsonDocument doc = await client.PostAsync($"ad_accounts/{id}/reports", body, ct);
        return JsonHelper.Format(doc);
    }

    [McpServerTool, Description(
        "Get daily performance for the last N days (default 7). " +
        "Convenience wrapper that returns impressions, clicks, spend, CTR, CPC, eCPM " +
        "broken down by DATE and CAMPAIGN_ID.")]
    public static async Task<string> GetDailyPerformance(
        RedditAdsClient client,
        [Description("Reddit ad account ID (optional, defaults to REDDIT_ACCOUNT_ID env var)")]
        string? accountId = null,
        [Description("Number of days to look back (default 7)")]
        int days = 7,
        CancellationToken ct = default)
    {
        string endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string startDate = DateTime.UtcNow.AddDays(-days).ToString("yyyy-MM-dd");

        return await GetPerformanceReport(
            client, startDate, endDate, accountId,
            breakdowns: ["DATE", "CAMPAIGN_ID"], ct: ct);
    }

    // v3 API requires ISO 8601 datetime; accept YYYY-MM-DD for convenience
    private static string NormalizeDate(string date) =>
        date.Contains('T') ? date : $"{date}T00:00:00Z";
}
