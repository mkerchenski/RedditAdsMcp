# RedditAdsMcp

A C# MCP (Model Context Protocol) server for the Reddit Ads API. Provides read-only tools for listing accounts, campaigns, ad groups, ads, and pulling performance reports.

Built with .NET 10 and the official [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol/) SDK.

## Available Tools

| Tool | Description |
|------|-------------|
| `ListAccounts` | List all Reddit ad accounts accessible with current credentials |
| `ListCampaigns` | List campaigns for an account |
| `ListAdGroups` | List ad groups, optionally filtered by campaign |
| `ListAds` | List ads, optionally filtered by ad group |
| `GetPerformanceReport` | Get a performance report with custom date range, metrics, breakdowns, and level |
| `GetDailyPerformance` | Convenience wrapper — last N days of impressions, clicks, spend, CTR, CPC, eCPM |

All tools accept an optional `accountId` parameter. If omitted, the default account from `REDDIT_ACCOUNT_ID` is used.

## Prerequisites

1. A Reddit account with access to [Reddit Ads](https://ads.reddit.com)
2. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
3. A Reddit developer app (setup below)

## Setup

### 1. Create a Reddit App

Go to [reddit.com/prefs/apps](https://www.reddit.com/prefs/apps) and create a new app:

- **Type:** script
- **Name:** anything (e.g. `reddit-ads-mcp`)
- **Redirect URI:** `http://localhost:8080`

After creating, note your **Client ID** (short string under the app name) and **Client Secret**.

### 2. Get a Refresh Token

**Open this URL in your browser** (replace `YOUR_CLIENT_ID`):

```
https://www.reddit.com/api/v1/authorize?client_id=YOUR_CLIENT_ID&response_type=code&state=mcp&redirect_uri=http://localhost:8080&duration=permanent&scope=adsread
```

Click **Allow**. Reddit redirects to `http://localhost:8080/?state=mcp&code=AUTHORIZATION_CODE`. Nothing will load — just copy the `code` value from the URL bar.

**Exchange the code for a refresh token:**

```bash
curl -X POST https://www.reddit.com/api/v1/access_token \
  -u "YOUR_CLIENT_ID:YOUR_CLIENT_SECRET" \
  -A "reddit-ads-mcp/1.0" \
  -d "grant_type=authorization_code&code=AUTHORIZATION_CODE&redirect_uri=http://localhost:8080"
```

The response contains your `refresh_token`. Save it — it's permanent until revoked.

### 3. Find Your Account ID

Your Reddit Ads account ID is visible in the Reddit Ads dashboard URL: `https://ads.reddit.com/accounts/ACCOUNT_ID/...`

### 4. Configure Claude Code

Build the project first:

```bash
dotnet build --project C:\Users\mkerc\Documents\OpenSource\RedditAdsMcp
```

Add to your Claude Code MCP settings (`~/.claude/settings.json` under `mcpServers`):

```json
"reddit-ads": {
  "type": "stdio",
  "command": "dotnet",
  "args": ["run", "--project", "C:\\path\\to\\RedditAdsMcp", "--no-build"],
  "env": {
    "REDDIT_CLIENT_ID": "your_client_id",
    "REDDIT_CLIENT_SECRET": "your_client_secret",
    "REDDIT_REFRESH_TOKEN": "your_refresh_token",
    "REDDIT_ACCOUNT_ID": "your_account_id"
  }
}
```

Verify with `/mcp` in Claude Code — the `reddit-ads` server should appear with 6 tools.

## Development

```bash
dotnet build    # compile
dotnet run      # start MCP server on stdio
```

## License

MIT
