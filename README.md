![Continuous integration](https://github.com/FAForever/faf-qai/workflows/Continuous%20integration/badge.svg)

# Faforever.Qai

A unified Discord and IRC bot for the Forged Alliance Forever (FAF) community. This project combines and rewrites the functionality of both Dostya and QAI into a single, modern application.

## Overview

Faforever.Qai serves as the official community bot for FAF, providing:
- **Discord Integration**: Rich Discord bot with slash commands, role management, and account linking
- **IRC Support**: Full IRC client support for traditional FAF channels
- **Dual Platform Commands**: Unified command system working across both Discord and IRC
- **FAF API Integration**: Direct integration with FAF services for player stats, replays, maps, and more
- **Account Linking**: Secure OAuth2-based system to link Discord accounts with FAF accounts

## Project History

### QAI Legacy
This is a total rewrite of [QAI](https://github.com/FAForever/QAI) - the original IRC bot that served the FAF community for years.

### Dostya Legacy  
This incorporates and rewrites [Dostya](https://github.com/FAForever/Dostya) - the Discord bot that provided modern Discord integration.

## Key Features

### Multi-Platform Support
- **Discord**: Modern slash commands, embeds, role management
- **IRC**: Traditional text commands, channel management
- **Unified Codebase**: Single application handling both platforms

### FAF Integration
- Player statistics and rankings
- Replay fetching and analysis
- Map information and ladder pools
- Live Twitch stream monitoring
- Clan information lookup
- Unit database searches

### Account Management
- Secure Discord ↔ FAF account linking via OAuth2
- Automatic role assignment for verified users
- Staff tools for account management
- Guild-specific configuration options

### Command Categories
- **Player Commands**: `!player`, `!seen`, statistics
- **Game Commands**: `!replay`, `!map`, `!unit`
- **Fun Commands**: `!8ball`, `!roll`, `!taunt`, `!hug`
- **Utility Commands**: `!help`, `!alive`, `!patch`
- **Admin Commands**: Link management, moderation tools

## Installation

### Quick Start
1. Download the latest release
2. Extract to desired folder
3. Run `Qai.exe` executable
4. Configure using environment variables or config files

### Development Setup
```bash
# Clone the repository
git clone https://github.com/FAForever/faf-qai.git
cd faf-qai

# Restore dependencies
dotnet restore

# Create database
dotnet ef database update --project src/Faforever.Qai.Core

# Run the application
dotnet run --project src/Faforever.Qai
```

## Configuration

### Understanding Configuration Variables

The bot uses a dual configuration system that supports both modern hierarchical configuration and legacy environment variables:

#### Configuration Loading Order
1. **appsettings.json** - Default values and structure
2. **Environment Variables** - Override defaults (two formats supported)
3. **Legacy Environment Variables** - Backward compatibility

#### Variable Naming Formats

**Format 1: Hierarchical Configuration (Recommended)**
Uses double underscores (`__`) to represent JSON hierarchy:
```bash
Config__Discord__ClientId=your_discord_client_id
Config__Faf__ClientId=your_faf_client_id
Config__Host=yourdomain.com
```
This maps to the JSON structure in `appsettings.json`:
```json
{
  "Config": {
    "Discord": {
      "ClientId": "your_discord_client_id"
    },
    "Faf": {
      "ClientId": "your_faf_client_id"
    },
    "Host": "yourdomain.com"
  }
}
```

**Format 2: Legacy Environment Variables**
Simple uppercase names for backward compatibility:
```bash
DISCORD_TOKEN=your_discord_bot_token
DISCORD_CLIENT_SECRET=your_discord_client_secret
FAF_CLIENT_SECRET=your_faf_client_secret
```

#### Why Two Formats?

- **Hierarchical (`Config__*`)**: Maps directly to the configuration structure, more explicit and clear
- **Legacy (`*_SECRET`)**: Shorter names, commonly used for sensitive data in container environments

**Best Practice**: Use hierarchical format for clarity, except for secrets where legacy format is more common in container deployments.

### Docker Compose Configuration

Example `docker-compose.yml` with proper environment configuration:

```yaml
version: '3.8'
services:
  qai-bot:
    image: ghcr.io/faforever/faf-qai:latest
    container_name: faf-qai
    environment:
      # Required: Discord Bot Token (legacy format)
      - DISCORD_TOKEN=your_discord_bot_token_here
      
      # Required: Discord OAuth2 Credentials (for account linking)
      - Config__Discord__ClientId=your_discord_client_id
      - DISCORD_CLIENT_SECRET=your_discord_client_secret  # Legacy format for secrets
      
      # Required: FAF OAuth2 Credentials (for account linking)  
      - Config__Faf__ClientId=your_faf_client_id
      - FAF_CLIENT_SECRET=your_faf_client_secret  # Legacy format for secrets
      
      # Required: Host Configuration (your public domain for OAuth2)
      - Config__Host=yourdomain.com
      
      # Optional: IRC Configuration
      - Config__Irc__Connection=irc.faforever.com
      - Config__Irc__User=your_bot_name
      - Config__Irc__Channels=aeolus,newbie
      
      # Optional: API Endpoints (defaults shown)
      - Config__Faf__Api=https://api.faforever.com
      - Config__Discord__Api=https://discord.com/api
      
      # Optional: Twitch Integration
      - Config__Twitch__ClientId=your_twitch_client_id
      - TWITCH_CLIENT_SECRET=your_twitch_client_secret
      
      # Optional: Logging Level
      - Logging__LogLevel__Default=Information
      
    ports:
      - "5000:5000"  # Required for OAuth2 callbacks
    volumes:
      - ./data:/app/data  # Persist database
    restart: unless-stopped
```

### Environment Variables Reference

| Environment Variable Name       | Required        | Description                                              |
|---------------------------------|-----------------|----------------------------------------------------------|
| `DISCORD_TOKEN`                 | Required        | Discord bot token                                        |
| `Config__Discord__ClientId`     | Account Linking | Discord OAuth2 client ID (for account linking)           |
| `DISCORD_CLIENT_SECRET`         | Account Linking | Discord OAuth2 client secret (for account linking)       |
| `Config__Faf__ClientId`         | Account Linking | FAF OAuth2 client ID (for account linking)               |
| `FAF_CLIENT_SECRET`             | Account Linking | FAF OAuth2 client secret (for account linking)           |
| `Config__Host`                  | Account Linking | Public domain for OAuth2 callbacks                       |
| `Config__Irc__Connection`       | Optional        | IRC server (default: `irc.faforever.com`)                |
| `Config__Irc__User`             | Optional        | IRC bot username                                         |
| `Config__Irc__Channels`         | Optional        | IRC channels to join (default: `aeolus,newbie`)          |
| `Config__Faf__Api`              | Optional        | FAF API base URL (default: `https://api.faforever.com`)  |

**Legend:**
- **Required** - Essential for basic bot functionality
- **Account Linking** - Required only for Discord ↔ FAF account linking feature
- **Optional** - Has sensible defaults, override only if needed

### Database Setup
Create a new database for testing:
```bash
# Using Package Manager Console
Update-Database --project Faforever.Qai.Core

# Using .NET Core CLI
dotnet ef database update --project src/Faforever.Qai.Core
```

Ensure the `test.db` file is in the `Faforever.Qai` project with `Copy to Output Directory` set to "Copy if newer".

### EF Core Tools
Install Entity Framework tools for database management:
```bash
dotnet tool install --global dotnet-ef
```

Reference: [EF Core Tools Documentation](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)

## Usage

### Discord Commands
Use slash commands in Discord:
- `/player <username>` - Get player statistics
- `/link` - Link your Discord account to FAF
- `/replay <id>` - Get replay information
- `/map <name>` - Search for maps

### IRC Commands  
Use traditional prefix commands in IRC channels:
- `!player coolmcgrrr` - Get player statistics
- `!replay 12345` - Get replay information
- `!alive` - Check bot status

All commands are prefixed with `!` in IRC and #aeolus channels.

## Architecture

### Project Structure
```
src/
├── Faforever.Qai/              # Main application and API
├── Faforever.Qai.Core/         # Core business logic and commands
├── Faforever.Qai.Discord/      # Discord-specific implementations
├── Faforever.Qai.Irc/         # IRC client implementation
└── IrcDotNet/                  # IRC protocol library
```

### Key Components
- **Command System**: Unified command processing for both platforms
- **API Integration**: HTTP clients for FAF services
- **OAuth2 Flow**: Secure account linking system
- **Database Layer**: Entity Framework Core with SQLite
- **Service Layer**: Business logic and operations

## FAF Discord Account Linking Setup

### Overview

The account linking feature allows Discord users to securely link their Discord accounts with their FAF accounts using OAuth2 authentication. This enables automatic role assignment, verification of FAF membership, and access to FAF-specific commands.

### Prerequisites

Before setting up account linking, you need:

1. **Discord Application**: A Discord bot application with OAuth2 credentials
2. **FAF OAuth2 Application**: Client credentials for FAF API access
3. **Public Domain/IP**: A publicly accessible domain or IP for OAuth2 callbacks
4. **SSL Certificate**: HTTPS is required for OAuth2 (recommended: Let's Encrypt)

### Setup Steps

#### 1. Discord Application Setup

1. Go to [Discord Developer Portal](https://discord.com/developers/applications)
2. Create a new application or select existing one
3. Navigate to **OAuth2** → **General**
4. Note down your **Client ID** and **Client Secret**
5. Add redirect URI: `https://yourdomain.com/authorization-code/discord-callback`

#### 2. FAF OAuth2 Application Setup

1. Contact FAF administrators to create an OAuth2 application
2. Provide your callback URL: `https://yourdomain.com/authorization-code/callback`
3. Receive your **FAF Client ID** and **FAF Client Secret**
4. Note the FAF API base URL (usually `https://api.faforever.com/`)

#### 3. Reverse Proxy Setup (Nginx Example)

Configure your reverse proxy to handle HTTPS and forward requests to the bot:

```nginx
server {
    listen 443 ssl;
    server_name yourdomain.com;
    
    ssl_certificate /path/to/your/certificate.pem;
    ssl_certificate_key /path/to/your/private.key;
    
    location /authorization-code/ {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    location /api/link/ {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Required OAuth2 Endpoints

The bot exposes these endpoints that must be publicly accessible:

| Endpoint                              | Purpose                             |
|---------------------------------------|-------------------------------------|
| `/authorization-code/discord-callback`| Discord OAuth2 callback             |
| `/authorization-code/callback`        | FAF OAuth2 callback                 |
| `/api/link/token/{token}`             | Link initiation endpoint            |
| `/api/link/login`                     | Discord authentication start        |
| `/api/link/auth`                      | FAF authentication start            |
| `/api/link/denied`                    | OAuth2 access denial handler        |

### Testing Account Linking

1. **Start the bot**: `docker-compose up -d`
2. **Test OAuth2 endpoints**: Visit `https://yourdomain.com/api/link/denied` (should return "User denied account linking.")
3. **Test Discord command**: Use `/link` command in Discord

### Usage Instructions

#### Setting Up Role Assignment

1. **Configure Link Role** (requires Manage Roles permission):
   ```
   /linkrole @VerifiedMember
   ```
   This role will be automatically assigned to users who complete the linking process.

2. **Remove Link Role** (to disable automatic role assignment):
   ```
   /linkrole
   ```

#### User Linking Process

1. User runs `/link` in Discord
2. Bot sends a private message with a unique link
3. User clicks the link and is redirected through OAuth2 flow:
   - Discord authentication (verifies identity)
   - FAF authentication (retrieves FAF account info)
4. Upon completion, accounts are linked and role is assigned (if configured)

#### Staff Management Commands

| Command           | Permission    | Description                     |
|-------------------|---------------|---------------------------------|
| `/links <user>`   | FAF Staff     | View link details for a user    |
| `/unlink <user>`  | FAF Staff     | Force remove account link       |
| `/linkrole <role>`| Manage Roles  | Set role for linked users       |

### Account Linking Troubleshooting

#### Common Issues

1. **"No token found" errors**
   - Check that `Config__Host` matches your public domain
   - Verify reverse proxy is forwarding requests correctly
   - Ensure HTTPS is properly configured

2. **OAuth2 callback errors**
   - Verify callback URLs in Discord/FAF applications match your setup
   - Check that port 5000 is properly exposed and forwarded
   - Confirm SSL certificates are valid and not expired

3. **Discord/FAF authentication fails**
   - Verify client IDs and secrets are correct
   - Check that Discord application has correct redirect URIs
   - Ensure FAF application is approved and active

4. **30-second timeout issues**
   - Users must complete OAuth2 flow within 30 seconds
   - Consider documenting this limitation for users
   - Have users retry `/link` if they encounter timeouts

#### Security Considerations

- **HTTPS Required**: OAuth2 flows require HTTPS in production
- **Secret Management**: Store secrets securely, never in public repositories
- **Token Expiration**: 30-second tokens prevent replay attacks
- **Scope Limitation**: OAuth2 scopes are minimal (Discord: "identify", FAF: "public_profile")
- **Duplicate Prevention**: System prevents linking already-linked accounts

## Documentation

- **Command Reference**: See [Issue #9](https://github.com/FAForever/faf-qai/issues/9) for full command list
- **API Documentation**: Integration details for FAF services

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## Troubleshooting

### Common Issues

1. **Bot not responding to commands**
   - Check that `DISCORD_TOKEN` is set correctly
   - Verify bot has proper permissions in Discord server
   - Check Docker logs: `docker-compose logs qai-bot`

2. **Database errors**
   - Ensure database volume is properly mounted
   - Check that database migrations have run
   - Verify write permissions for data directory

3. **IRC connection issues**
   - Check `Config__Irc__Connection` points to correct server
   - Verify IRC credentials if authentication required
   - Check firewall/network connectivity

### Debug Mode

Enable debug logging for troubleshooting:
```yaml
environment:
  - Logging__LogLevel__Default=Debug
```

### Health Checks

Monitor these indicators for a healthy bot:
- Bot responds to `/alive` command in Discord
- IRC connection shows as active in logs
- Database queries execute successfully
- OAuth2 endpoints return appropriate responses (if linking enabled)