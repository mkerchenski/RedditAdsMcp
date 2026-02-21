using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditAdsMcp.Auth;
using RedditAdsMcp.Client;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// MCP uses stdout for protocol wire format — all logs must go to stderr
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddSingleton<RedditAuthService>();
builder.Services.AddHttpClient<RedditAdsClient>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
