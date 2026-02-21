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

1. A Reddit account with an active [Reddit Ads](https://ads.reddit.com) advertiser account
2. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Setup

### 1. Create a Reddit Ads API App

Go to [ads.reddit.com](https://ads.reddit.com), then navigate to **Developer Applications** in the left sidebar (under your account/business settings). Click **Create a new app** and fill in:

- **App name:** a descriptive name for your app
- **Description:** optional
- **About url:** optional (e.g. your project's GitHub URL)
- **Redirect URI:** `https://hurrah.dev/oauth/reddit`

> **Tip:** You can use your own HTTPS redirect URI instead if you prefer — just update the URLs in the steps below to match.

After creating, note your **Client ID** and **Client Secret**.

### 2. Get a Refresh Token

**Open this URL in your browser** (replace `YOUR_CLIENT_ID`):

```
https://www.reddit.com/api/v1/authorize?client_id=YOUR_CLIENT_ID&response_type=code&state=mcp&redirect_uri=https%3A%2F%2Fhurrah.dev%2Foauth%2Freddit&duration=permanent&scope=adsread
```

Click **Allow**. Reddit redirects to `hurrah.dev/oauth/reddit` which displays your authorization code. Click **Copy**.

**Exchange the code for a refresh token:**

```bash
curl -X POST https://www.reddit.com/api/v1/access_token \
  -u "YOUR_CLIENT_ID:YOUR_CLIENT_SECRET" \
  -A "ads-mcp/1.0" \
  -d "grant_type=authorization_code&code=AUTHORIZATION_CODE&redirect_uri=https://hurrah.dev/oauth/reddit"
```

> **Important:** The `redirect_uri` in the curl command must match exactly what you registered in Step 1.

The response contains your `refresh_token`. Save it — it's permanent until revoked.

### 3. Find Your Account ID

Your Reddit Ads account ID is visible in the Reddit Ads dashboard URL: `https://ads.reddit.com/accounts/ACCOUNT_ID/...`

### 4. Configure Claude Code

Build the project first:

```bash
dotnet build
```

Add to your Claude Code MCP settings (`~/.claude/settings.json` under `mcpServers`):

```json
"reddit-ads": {
  "type": "stdio",
  "command": "dotnet",
  "args": ["run", "--project", "/path/to/RedditAdsMcp", "--no-build"],
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
