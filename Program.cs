using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditAdsMcp.Auth;
using RedditAdsMcp.Client;

// MCP clients start this process with whatever working directory they happen to have
// (Claude Desktop on macOS uses "/"). Host.CreateApplicationBuilder(args) takes that directory as
// the content root and, with its default reloadOnChange, watches appsettings*.json there with a
// recursive FileSystemWatcher. With "/" that is the whole disk: ~100% CPU on a busy machine, and a
// growing memory leak because the watcher is restarted on every mount/unmount or event overflow.
// This server is configured only through environment variables, so pin the content root to the
// install directory and turn configuration reload (file watching) off.
ConfigurationManager hostConfig = new();
hostConfig.AddInMemoryCollection(new Dictionary<string, string?> { ["hostBuilder:reloadConfigOnChange"] = "false" });

HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    Configuration = hostConfig,
    ContentRootPath = AppContext.BaseDirectory,
});

// MCP uses stdout for protocol wire format — all logs must go to stderr
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddHttpClient("RedditAuth", c =>
    c.DefaultRequestHeaders.UserAgent.ParseAdd("reddit-ads-mcp-csharp/1.0"));
builder.Services.AddSingleton<RedditAuthService>();
builder.Services.AddHttpClient<RedditAdsClient>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
