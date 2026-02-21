using System.Text.Json.Serialization;

namespace RedditAdsMcp.Models;

public record ReportRequest
{
    [JsonPropertyName("starts_at")]
    public string StartsAt { get; init; } = "";

    [JsonPropertyName("ends_at")]
    public string EndsAt { get; init; } = "";

    [JsonPropertyName("fields")]
    public string[] Fields { get; init; } = ["IMPRESSIONS", "CLICKS", "SPEND", "CTR", "CPC", "ECPM"];

    [JsonPropertyName("breakdowns")]
    public string[] Breakdowns { get; init; } = ["DATE"];
}
