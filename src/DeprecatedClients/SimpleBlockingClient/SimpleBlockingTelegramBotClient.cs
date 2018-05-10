using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using MihaZupan.TelegramBotClients.SimpleBlocking;

namespace MihaZupan.TelegramBotClients
{
    public class SimpleBlockingTelegramBotClient : TelegramBotClient
    {
        private TelegramRequestScheduler Scheduler = new TelegramRequestScheduler();

        /// <summary>
        /// Create a new <see cref="SimpleBlockingTelegramBotClient"/> instance.
        /// </summary>
        /// <param name="token">API token</param>
        /// <param name="httpClient">A custom <see cref="HttpClient"/></param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="token"/> format is invalid</exception>
        public SimpleBlockingTelegramBotClient(string token, HttpClient httpClient = null)
            : base(token, httpClient)
        { }

        /// <summary>
        /// Create a new <see cref="SimpleBlockingTelegramBotClient"/> instance behind a proxy.
        /// </summary>
        /// <param name="token">API token</param>
        /// <param name="webProxy">Use this <see cref="IWebProxy"/> to connect to the API</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="token"/> format is invalid</exception>
        public SimpleBlockingTelegramBotClient(string token, IWebProxy webProxy)
            : base(token, webProxy)
        { }
        
        public override async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            PropertyInfo chatId = request.GetType().GetProperty("ChatId");
            if (chatId == null)
            {
                Scheduler.WaitOne();
            }
            else
            {
                Scheduler.WaitOne((ChatId)chatId.GetValue(request));
            }
            return await base.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public override async Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken = default)
        {
            Scheduler.WaitOne();
            await base.DownloadFileAsync(filePath, destination, cancellationToken).ConfigureAwait(false);
        }
    }
}
