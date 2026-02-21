using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using RedditAdsMcp.Client;
using RedditAdsMcp.Models;

namespace RedditAdsMcp.Tools;

[McpServerToolType]
public static class ReportTools
{
    private static readonly string[] DefaultMetrics =
        ["IMPRESSIONS", "CLICKS", "SPEND", "CTR", "CPC", "ECPM"];

    [McpServerTool, Description(
        "Get a performance report for a Reddit ad account. " +
        "Returns metrics like impressions, clicks, spend, CTR, CPC, and eCPM " +
        "broken down by the specified level and breakdowns.")]
    public static async Task<string> GetPerformanceReport(
        RedditAdsClient client,
        [Description("Start date in YYYY-MM-DD format")]
        string startDate,
        [Description("End date in YYYY-MM-DD format")]
        string endDate,
        [Description("Reddit ad account ID (optional)")]
        string? accountId = null,
        [Description("Metrics to include (e.g. IMPRESSIONS, CLICKS, SPEND, CTR, CPC, ECPM). Defaults to all common metrics.")]
        string[]? metrics = null,
        [Description("Breakdowns for the report (e.g. date, campaign_id, ad_group_id). Defaults to [date].")]
        string[]? breakdowns = null,
        [Description("Report level: ACCOUNT, CAMPAIGN, AD_GROUP, or AD. Defaults to CAMPAIGN.")]
        string? level = null,
        CancellationToken ct = default)
    {
        string id = client.ResolveAccountId(accountId);
        var request = new ReportRequest
        {
            Starts = startDate,
            Ends = endDate,
            Level = level ?? "CAMPAIGN",
            Metrics = metrics ?? DefaultMetrics,
            Breakdowns = breakdowns ?? ["date"]
        };

        using JsonDocument doc = await client.PostAsync($"/accounts/{id}/reports", request, ct);
        return JsonHelper.Format(doc);
    }

    [McpServerTool, Description(
        "Get daily performance for the last N days (default 7). " +
        "Convenience wrapper that returns impressions, clicks, spend, CTR, CPC, eCPM by date.")]
    public static async Task<string> GetDailyPerformance(
        RedditAdsClient client,
        [Description("Reddit ad account ID (optional)")]
        string? accountId = null,
        [Description("Number of days to look back (default 7)")]
        int days = 7,
        CancellationToken ct = default)
    {
        string endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string startDate = DateTime.UtcNow.AddDays(-days).ToString("yyyy-MM-dd");

        return await GetPerformanceReport(
            client, startDate, endDate, accountId,
            level: "CAMPAIGN", ct: ct);
    }
}
