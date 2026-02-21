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

1. Go to [ads.reddit.com](https://ads.reddit.com)
2. In the left sidebar, click **Developer Applications** (under your account/business settings)
3. Click **Create a new app** and fill in:

| Field | Value |
|-------|-------|
| **App name** | `Reddit Ads MCP` |
| **Description** | `MCP server for Reddit Ads reporting` |
| **About url** | `https://github.com/user/RedditAdsMcp` (or leave blank) |
| **Redirect URI** | `https://hurrah.dev/oauth/reddit` |

Reddit requires a public HTTPS redirect URI (localhost won't work). The URL above is a free callback page that displays the authorization code for you to copy — no server setup required.

4. Click **Create app**
5. Copy your **Client ID** and **Client Secret** — you'll need both in the next steps

> **Using your own redirect URI?** You can use any HTTPS URL you control instead — just update the URLs in Steps 2–3 to match.

### 2. Authorize the App

Open this URL in your browser, replacing `YOUR_CLIENT_ID` with the Client ID from Step 1:

```
https://www.reddit.com/api/v1/authorize?client_id=YOUR_CLIENT_ID&response_type=code&state=mcp&redirect_uri=https%3A%2F%2Fhurrah.dev%2Foauth%2Freddit&duration=permanent&scope=adsread
```

Click **Allow**. You'll be redirected to a page that displays your authorization code — click **Copy**.

### 3. Exchange the Code for a Refresh Token

Run this command, replacing the three `YOUR_*` placeholders:

```bash
curl -X POST https://www.reddit.com/api/v1/access_token \
  -u "YOUR_CLIENT_ID:YOUR_CLIENT_SECRET" \
  -A "ads-mcp/1.0" \
  -d "grant_type=authorization_code&code=YOUR_AUTHORIZATION_CODE&redirect_uri=https://hurrah.dev/oauth/reddit"
```

> The `redirect_uri` must match exactly what you entered in Step 1.

The response JSON contains a `refresh_token` field. Save it — it's permanent until revoked.

### 4. Find Your Account ID

Your Reddit Ads account ID is in the dashboard URL: `https://ads.reddit.com/accounts/ACCOUNT_ID/...`

### 5. Configure Claude Code

Build the project:

```bash
dotnet build
```

Add to your Claude Code MCP settings (`~/.claude/settings.json` under `mcpServers`), replacing the four placeholder values:

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
