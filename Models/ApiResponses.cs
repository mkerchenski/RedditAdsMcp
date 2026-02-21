using System.Text.Json.Serialization;

namespace RedditAdsMcp.Models;

// Reddit Ads API v3 response DTOs.
// Only the fields we need for display — the rest pass through as raw JSON.

public record AccountData
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("time_zone_id")]
    public string? TimeZoneId { get; init; }
}

public record CampaignData
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; init; }

    [JsonPropertyName("effective_status")]
    public string? EffectiveStatus { get; init; }

    [JsonPropertyName("objective")]
    public string? Objective { get; init; }

    [JsonPropertyName("spend_cap")]
    public SpendCap? SpendCap { get; init; }
}

public record SpendCap
{
    [JsonPropertyName("micro_amount")]
    public long? MicroAmount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    // micro_amount is in millionths of a currency unit
    [JsonIgnore]
    public decimal? Amount => MicroAmount.HasValue ? MicroAmount.Value / 1_000_000m : null;
}

public record AdGroupData
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("campaign_id")]
    public string CampaignId { get; init; } = "";

    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; init; }

    [JsonPropertyName("effective_status")]
    public string? EffectiveStatus { get; init; }

    [JsonPropertyName("bid_strategy")]
    public string? BidStrategy { get; init; }
}

public record AdData
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("ad_group_id")]
    public string AdGroupId { get; init; } = "";

    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; init; }

    [JsonPropertyName("effective_status")]
    public string? EffectiveStatus { get; init; }

    [JsonPropertyName("click_url")]
    public string? ClickUrl { get; init; }
}
