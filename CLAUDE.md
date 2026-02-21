# RedditAdsMcp - Claude Code Instructions

## What This Is
A C# MCP server for the Reddit Ads API. Provides read-only tools for listing accounts, campaigns, ad groups, ads, and pulling performance reports.

## Build & Run
```bash
dotnet build
dotnet run              # starts MCP server on stdio
dotnet run --no-build   # skip rebuild (use after initial build)
```

## Project Structure
- `Program.cs` — host setup, DI, stdio transport
- `Auth/RedditAuthService.cs` — OAuth 2.0 token management with auto-refresh
- `Client/RedditAdsClient.cs` — HTTP client wrapper for Reddit Ads API v3
- `Tools/` — MCP tool definitions (AccountTools, CampaignTools, ReportTools)
- `Models/` — Reddit API response DTOs and request models

## Environment Variables (Required)
- `REDDIT_CLIENT_ID` — Reddit app client ID
- `REDDIT_CLIENT_SECRET` — Reddit app client secret
- `REDDIT_REFRESH_TOKEN` — OAuth refresh token
- `REDDIT_ACCOUNT_ID` — default Reddit ad account ID

## Tech Stack
- .NET 10, ModelContextProtocol SDK (v0.9.0-preview.1)
- Stdio transport (for Claude Code integration)
- System.Text.Json for serialization
