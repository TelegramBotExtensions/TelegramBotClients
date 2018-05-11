# TelegramBotClients
A collection of TelegramBotClients that implement various features

Based on the [C# Telegram.Bot library](https://github.com/TelegramBots/Telegram.Bot)

## Usage

Replace
```c#
TelegramBotClient Bot = new TelegramBotClient(token);
```
With
```c#
BlockingTelegramBotClient Bot = new BlockingTelegramBotClient(token);
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