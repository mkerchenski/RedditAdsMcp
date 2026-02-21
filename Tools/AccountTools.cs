using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using RedditAdsMcp.Client;

namespace RedditAdsMcp.Tools;

[McpServerToolType]
public static class AccountTools
{
    [McpServerTool, Description("List all Reddit ad accounts accessible with the current credentials. Discovers accounts via /me/businesses and then fetches ad accounts for each business.")]
    public static async Task<string> ListAccounts(RedditAdsClient client, CancellationToken ct)
    {
        using JsonDocument bizDoc = await client.GetAsync("me/businesses", ct);

        List<JsonElement> allAccounts = [];
        foreach (JsonElement biz in bizDoc.RootElement.GetProperty("data").EnumerateArray())
        {
            string bizId = biz.GetProperty("id").GetString()!;
            using JsonDocument acctDoc = await client.GetAsync($"businesses/{bizId}/ad_accounts", ct);
            foreach (JsonElement acct in acctDoc.RootElement.GetProperty("data").EnumerateArray())
                allAccounts.Add(acct.Clone());
        }

        return JsonHelper.Format(allAccounts);
    }
}
