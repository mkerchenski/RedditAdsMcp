using System.Text.Json;

namespace RedditAdsMcp.Tools;

internal static class JsonHelper
{
    private static readonly JsonSerializerOptions IndentedOptions = new() { WriteIndented = true };

    public static string Format(JsonDocument doc) =>
        JsonSerializer.Serialize(doc.RootElement, IndentedOptions);

    public static string Format(List<JsonElement> elements) =>
        JsonSerializer.Serialize(elements, IndentedOptions);
}
