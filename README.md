# TelegramBotClients
A collection of TelegramBotClients that implement various features

Based on the [C# Telegram.Bot library](https://github.com/TelegramBots/Telegram.Bot)

## Usage

```c#
// Add usings
using MihaZupan.TelegramBotClients; // For the Client
using MihaZupan.TelegramBotClients.BlockingClient; // For the SchedulingMethod enum

// Replace
TelegramBotClient Bot = new TelegramBotClient(token);
// With
BlockingTelegramBotClient Bot = new BlockingTelegramBotClient(token);

// All requests to the bot API are now rate-limited
// You can also use an extra parameter on all requests:
Bot.SendTextMessageAsync(chatId, "Hello");
Bot.SendTextMessageAsync(chatId, "Urgent!", schedulingMethod: SchedulingMethod.HighPriority);
```

## Installation

Install as [NuGet package](https://www.nuget.org/packages/TelegramBotClients/):

Package manager:

```powershell
Install-Package TelegramBotClients
```

## List of bot clients

+ BlockingTelegramBotClient (implements rate limiting and queueing, adds a SchedulingMethod parameter to every method)
+ Everything that ends up in the DeprecatedClients folder