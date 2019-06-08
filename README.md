# TelegramBotClients
A collection of TelegramBotClients that implement various features

Based on the [C# Telegram.Bot library](https://github.com/TelegramBots/Telegram.Bot)

## Usage

```c#
// Add usings
using MihaZupan.TelegramBotClients;

// Replace
TelegramBotClient Bot = new TelegramBotClient(token);
// With
ITelegramBotClient Bot = new RateLimitedTelegramBotClient(token);

// All requests to the bot API are now rate-limited
```

For even better rate-limiting and update processing, consider using [SharpCollections]'s `WorkScheduler<Update>`

## Installation

Install as [NuGet package](https://www.nuget.org/packages/TelegramBotClients/):

Package manager:

```powershell
Install-Package TelegramBotClients
```

[SharpCollections]: https://github.com/MihaZupan/SharpCollections