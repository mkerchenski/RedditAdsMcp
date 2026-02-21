using System.Text.Json.Serialization;

namespace RedditAdsMcp.Models;

public record ReportRequest
{
    [JsonPropertyName("starts")]
    public string Starts { get; init; } = "";

    [JsonPropertyName("ends")]
    public string Ends { get; init; } = "";

    [JsonPropertyName("level")]
    public string Level { get; init; } = "CAMPAIGN";

    [JsonPropertyName("metrics")]
    public string[] Metrics { get; init; } = ["IMPRESSIONS", "CLICKS", "SPEND", "CTR", "CPC", "ECPM"];

    [JsonPropertyName("breakdowns")]
    public string[] Breakdowns { get; init; } = ["date"];
}
