using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using RedditAdsMcp.Client;

namespace RedditAdsMcp.Tools;

[McpServerToolType]
public static class CampaignTools
{
    [McpServerTool, Description(
        "List all campaigns for a Reddit ad account. " +
        "If accountId is not provided, uses the default account from REDDIT_ACCOUNT_ID.")]
    public static async Task<string> ListCampaigns(
        RedditAdsClient client,
        [Description("Reddit ad account ID (optional, defaults to REDDIT_ACCOUNT_ID env var)")]
        string? accountId = null,
        CancellationToken ct = default)
    {
        string id = client.ResolveAccountId(accountId);
        using JsonDocument doc = await client.GetAsync($"/accounts/{id}/campaigns", ct);
        return JsonHelper.Format(doc);
    }

    [McpServerTool, Description(
        "List ad groups for a Reddit ad account, optionally filtered by campaign. " +
        "If accountId is not provided, uses the default account.")]
    public static async Task<string> ListAdGroups(
        RedditAdsClient client,
        [Description("Reddit ad account ID (optional)")]
        string? accountId = null,
        [Description("Campaign ID to filter ad groups (optional)")]
        string? campaignId = null,
        CancellationToken ct = default)
    {
        string id = client.ResolveAccountId(accountId);
        string path = $"/accounts/{id}/ad_groups";
        if (campaignId is not null)
            path += $"?campaign_id={campaignId}";

        using JsonDocument doc = await client.GetAsync(path, ct);
        return JsonHelper.Format(doc);
    }

    [McpServerTool, Description(
        "List ads for a Reddit ad account, optionally filtered by ad group. " +
        "If accountId is not provided, uses the default account.")]
    public static async Task<string> ListAds(
        RedditAdsClient client,
        [Description("Reddit ad account ID (optional)")]
        string? accountId = null,
        [Description("Ad group ID to filter ads (optional)")]
        string? adGroupId = null,
        CancellationToken ct = default)
    {
        string id = client.ResolveAccountId(accountId);
        string path = $"/accounts/{id}/ads";
        if (adGroupId is not null)
            path += $"?ad_group_id={adGroupId}";

        using JsonDocument doc = await client.GetAsync(path, ct);
        return JsonHelper.Format(doc);
    }
}
