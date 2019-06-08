# TelegramBotClients
A collection of TelegramBotClients that implement various features

Based on the [C# Telegram.Bot library](https://github.com/TelegramBots/Telegram.Bot)

## Usage

```c#
// Add usings
using MihaZupan.TelegramBotClients;

// Replace
ITelegramBotClient Bot = new TelegramBotClient(token);
// With
ITelegramBotClient Bot = new RateLimitedTelegramBotClient(token);

// All requests to the bot API are now rate-limited
```

For even better rate-limiting and update processing, consider using [SharpCollections]'s `WorkScheduler<Update>`.

This will ensure that only one update from each group/chat will be processed at the same time.

```c#
void StartBot()
{
    var updateQueue = new WorkScheduler<Update>(OnUpdate);
    Bot.OnUpdate += (_, e) => updateQueue.Enqueue(e.Update, GetBucket(e.Update));
    Bot.StartReceiving();
}

public async Task OnUpdate(Update update)
{
    if (update.Message is Message message && message.Text is string text)
    {
        await Bot.SendTextMessageAsync(message.Chat, "Echo:\n" + text);
    }
}
```

See [example `GetBucket(Update)` implementation](https://gist.github.com/MihaZupan/fc55672e78091b2e5f21da311ff5c7d0).

## Installation

Install as [NuGet package](https://www.nuget.org/packages/TelegramBotClients/):

Package manager:

```powershell
Install-Package TelegramBotClients
```

[SharpCollections]: https://github.com/MihaZupan/SharpCollections