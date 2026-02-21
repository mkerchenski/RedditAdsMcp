using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using RedditAdsMcp.Client;

namespace RedditAdsMcp.Tools;

[McpServerToolType]
public static class AccountTools
{
    [McpServerTool, Description("List all Reddit ad accounts accessible with the current credentials.")]
    public static async Task<string> ListAccounts(RedditAdsClient client, CancellationToken ct)
    {
        using JsonDocument doc = await client.GetAsync("/accounts", ct);
        return FormatJson(doc);
    }

    private static string FormatJson(JsonDocument doc) =>
        JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
}
