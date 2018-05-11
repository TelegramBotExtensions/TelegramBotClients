using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;
using MihaZupan.TelegramBotClients.BlockingClient;

namespace MihaZupan.TelegramBotClients
{
    public class BlockingTelegramBotClient
    {
        private TelegramBotClient Client;
        private TelegramRequestScheduler Scheduler;

        #region Config Properties

        /// <summary>
        /// Timeout for requests
        /// </summary>
        public TimeSpan Timeout
        {
            get => Client.Timeout;
            set => Client.Timeout = value;
        }

        /// <summary>
        /// Indicates if receiving updates
        /// </summary>
        public bool IsReceiving
        {
            get => Client.IsReceiving;
            set => Client.IsReceiving = value;
        }

        /// <summary>
        /// The current message offset
        /// </summary>
        public int MessageOffset
        {
            get => Client.MessageOffset;
            set => Client.MessageOffset = value;
        }

        #endregion Config Properties

        #region Events

        /// <summary>
        /// Occurs before sending a request to API
        /// </summary>
        public event EventHandler<ApiRequestEventArgs> MakingApiRequest
        {
            add => Client.MakingApiRequest += value;
            remove => Client.MakingApiRequest -= value;
        }

        /// <summary>
        /// Occurs after receiving the response to an API request
        /// </summary>
        public event EventHandler<ApiResponseEventArgs> ApiResponseReceived
        {
            add => Client.ApiResponseReceived += value;
            remove => Client.ApiResponseReceived -= value;
        }

        /// <summary>
        /// Occurs when an <see cref="Update"/> is received.
        /// </summary>
        public event EventHandler<UpdateEventArgs> OnUpdate
        {
            add => Client.OnUpdate += value;
            remove => Client.OnUpdate -= value;
        }

        /// <summary>
        /// Occurs when a <see cref="Message"/> is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessage
        {
            add => Client.OnMessage += value;
            remove => Client.OnMessage -= value;
        }

        /// <summary>
        /// Occurs when <see cref="Message"/> was edited.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessageEdited
        {
            add => Client.OnMessageEdited += value;
            remove => Client.OnMessageEdited -= value;
        }

        /// <summary>
        /// Occurs when an <see cref="InlineQuery"/> is received.
        /// </summary>
        public event EventHandler<InlineQueryEventArgs> OnInlineQuery
        {
            add => Client.OnInlineQuery += value;
            remove => Client.OnInlineQuery -= value;
        }

        /// <summary>
        /// Occurs when a <see cref="ChosenInlineResult"/> is received.
        /// </summary>
        public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen
        {
            add => Client.OnInlineResultChosen += value;
            remove => Client.OnInlineResultChosen -= value;
        }

        /// <summary>
        /// Occurs when an <see cref="CallbackQuery"/> is received
        /// </summary>
        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery
        {
            add => Client.OnCallbackQuery += value;
            remove => Client.OnCallbackQuery -= value;
        }

        /// <summary>
        /// Occurs when an error occurs during the background update pooling.
        /// </summary>
        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError
        {
            add => Client.OnReceiveError += value;
            remove => Client.OnReceiveError -= value;
        }

        /// <summary>
        /// Occurs when an error occurs during the background update pooling.
        /// </summary>
        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError
        {
            add => Client.OnReceiveGeneralError += value;
            remove => Client.OnReceiveGeneralError -= value;
        }

        #endregion Events

        #region Constructors

        /// <summary>
        /// Create a new <see cref="BlockingTelegramBotClient"/> instance.
        /// </summary>
        /// <param name="token">API token</param>
        /// <param name="httpClient">A custom <see cref="HttpClient"/></param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="token"/> format is invalid</exception>
        public BlockingTelegramBotClient(string token, HttpClient httpClient = null, SchedulerSettings schedulerSettings = default)
        {
            Client = new TelegramBotClient(token, httpClient);
            Scheduler = new TelegramRequestScheduler(schedulerSettings ?? SchedulerSettings.Default);
            Client.MakingApiRequest += Client_MakingApiRequest;
        }

        /// <summary>
        /// Create a new <see cref="BlockingTelegramBotClient"/> instance behind a proxy.
        /// </summary>
        /// <param name="token">API token</param>
        /// <param name="webProxy">Use this <see cref="IWebProxy"/> to connect to the API</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="token"/> format is invalid</exception>
        public BlockingTelegramBotClient(string token, IWebProxy webProxy, SchedulerSettings schedulerSettings = default)
        {
            Client = new TelegramBotClient(token, webProxy);
            Scheduler = new TelegramRequestScheduler(schedulerSettings ?? SchedulerSettings.Default);
            Client.MakingApiRequest += Client_MakingApiRequest;
        }

        #endregion Constructors

        private void Client_MakingApiRequest(object sender, ApiRequestEventArgs e)
        {
            // Make sure getting updates doesn't end up pushing us over the limit
            if (e.MethodName == "getUpdates")
                Scheduler.WaitOne(SchedulingMethod.NoScheduling);
        }

#region Helpers

        /// <inheritdoc />
        [Obsolete("Use the overload with extra parameters added by BlockingTelegramBotClient instead.", true)]
        public async Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken)
        {
            return await MakeRequestAsync(request, cancellationToken, null, SchedulingMethod.Normal);
        }

        /// <inheritdoc />
        public async Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            SchedulingMethod schedulingMethod)
        {
            return await MakeRequestAsync(request, cancellationToken, null, schedulingMethod);
        }

        /// <inheritdoc />
        public async Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            ChatId chatId,
            SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod != SchedulingMethod.Ignore)
            {
                if (chatId == null) Scheduler.WaitOne(schedulingMethod);
                else Scheduler.WaitOne(chatId, schedulingMethod);
            }

            return await Client.MakeRequestAsync(request, cancellationToken);
        }

        /// <summary>
        /// Test the API token
        /// </summary>
        /// <returns><c>true</c> if token is valid</returns>
        public async Task<bool> TestApiAsync(CancellationToken cancellationToken = default)
        {
            return await Client.TestApiAsync(cancellationToken);
        }

        /// <summary>
        /// Start update receiving
        /// </summary>
        /// <param name="allowedUpdates">List the types of updates you want your bot to receive.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiRequestException"> Thrown if token is invalid</exception>
        public void StartReceiving(UpdateType[] allowedUpdates = null,
            CancellationToken cancellationToken = default)
        {
            Client.StartReceiving(allowedUpdates, cancellationToken);
        }

        /// <summary>
        /// Stop update receiving
        /// </summary>
        public void StopReceiving()
        {
            Client.StopReceiving();
        }

#endregion Helpers

#region Getting updates

        /// <inheritdoc />
        public Task<Update[]> GetUpdatesAsync(
            int offset = default,
            int limit = default,
            int timeout = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetUpdatesRequest
            {
                Offset = offset,
                Limit = limit,
                Timeout = timeout,
                AllowedUpdates = allowedUpdates
            }, cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task SetWebhookAsync(
            string url,
            InputFileStream certificate = default,
            int maxConnections = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetWebhookRequest(url, certificate)
            {
                MaxConnections = maxConnections,
                AllowedUpdates = allowedUpdates
            }, cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task DeleteWebhookAsync(CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal)
            => MakeRequestAsync(new DeleteWebhookRequest(), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task<WebhookInfo> GetWebhookInfoAsync(CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal)
            => MakeRequestAsync(new GetWebhookInfoRequest(), cancellationToken, schedulingMethod);

#endregion Getting updates

#region Available methods

        /// <inheritdoc />
        public Task<User> GetMeAsync(CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal)
            => MakeRequestAsync(new GetMeRequest(), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendTextMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = default,
            bool disableWebPagePreview = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendMessageRequest(chatId, text)
            {
                ParseMode = parseMode,
                DisableWebPagePreview = disableWebPagePreview,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> ForwardMessageAsync(
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            bool disableNotification = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new ForwardMessageRequest(chatId, fromChatId, messageId)
            {
                DisableNotification = disableNotification
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendPhotoAsync(
            ChatId chatId,
            InputOnlineFile photo,
            string caption = default,
            ParseMode parseMode = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendPhotoRequest(chatId, photo)
            {
                Caption = caption,
                ParseMode = parseMode,
                ReplyToMessageId = replyToMessageId,
                DisableNotification = disableNotification,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendAudioAsync(
            ChatId chatId,
            InputOnlineFile audio,
            string caption = default,
            ParseMode parseMode = default,
            int duration = default,
            string performer = default,
            string title = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendAudioRequest(chatId, audio)
            {
                Caption = caption,
                ParseMode = parseMode,
                Duration = duration,
                Performer = performer,
                Title = title,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendDocumentAsync(
            ChatId chatId,
            InputOnlineFile document,
            string caption = default,
            ParseMode parseMode = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendDocumentRequest(chatId, document)
            {
                Caption = caption,
                ParseMode = parseMode,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendStickerAsync(
            ChatId chatId,
            InputOnlineFile sticker,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendStickerRequest(chatId, sticker)
            {
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendVideoAsync(
            ChatId chatId,
            InputOnlineFile video,
            int duration = default,
            int width = default,
            int height = default,
            string caption = default,
            ParseMode parseMode = default,
            bool supportsStreaming = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendVideoRequest(chatId, video)
            {
                Duration = duration,
                Width = width,
                Height = height,
                Caption = caption,
                ParseMode = parseMode,
                SupportsStreaming = supportsStreaming,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendVoiceAsync(
            ChatId chatId,
            InputOnlineFile voice,
            string caption = default,
            ParseMode parseMode = default,
            int duration = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendVoiceRequest(chatId, voice)
            {
                Caption = caption,
                ParseMode = parseMode,
                Duration = duration,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendVideoNoteAsync(
            ChatId chatId,
            InputTelegramFile videoNote,
            int duration = default,
            int length = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendVideoNoteRequest(chatId, videoNote)
            {
                Duration = duration,
                Length = length,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message[]> SendMediaGroupAsync(
            ChatId chatId,
            IEnumerable<InputMediaBase> media,
            bool disableNotification = default,
            int replyToMessageId = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendMediaGroupRequest(chatId, media)
            {
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendLocationAsync(
            ChatId chatId,
            float latitude,
            float longitude,
            int livePeriod = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendLocationRequest(chatId, latitude, longitude)
            {
                LivePeriod = livePeriod,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendVenueAsync(
            ChatId chatId,
            float latitude,
            float longitude,
            string title,
            string address,
            string foursquareId = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendVenueRequest(chatId, latitude, longitude, title, address)
            {
                FoursquareId = foursquareId,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SendContactAsync(
            ChatId chatId,
            string phoneNumber,
            string firstName,
            string lastName = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendContactRequest(chatId, phoneNumber, firstName)
            {
                LastName = lastName,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task SendChatActionAsync(
            ChatId chatId,
            ChatAction chatAction,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendChatActionRequest(chatId, chatAction), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<UserProfilePhotos> GetUserProfilePhotosAsync(
            int userId,
            int offset = default,
            int limit = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetUserProfilePhotosRequest(userId)
            {
                Offset = offset,
                Limit = limit
            }, cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task<File> GetFileAsync(
            string fileId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetFileRequest(fileId), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public async Task<Stream> DownloadFileAsync(
            string filePath,
            CancellationToken cancellationToken = default
        )
        {
            var stream = new MemoryStream();
            await DownloadFileAsync(filePath, stream, cancellationToken)
                .ConfigureAwait(false);
            return stream;
        }

        /// <inheritdoc />
        public virtual async Task DownloadFileAsync(
            string filePath,
            Stream destination,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        )
        {
            Scheduler.WaitOne(schedulingMethod);
            await Client.DownloadFileAsync(filePath, destination, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<File> GetInfoAndDownloadFileAsync(
            string fileId,
            Stream destination,
            CancellationToken cancellationToken = default
        )
        {
            var file = await GetFileAsync(fileId, cancellationToken)
                .ConfigureAwait(false);

            await DownloadFileAsync(file.FilePath, destination, cancellationToken)
                .ConfigureAwait(false);

            return file;
        }

        /// <inheritdoc />
        public Task KickChatMemberAsync(
            ChatId chatId,
            int userId,
            DateTime untilDate = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new KickChatMemberRequest(chatId, userId)
            {
                UntilDate = untilDate
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task LeaveChatAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new LeaveChatRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task UnbanChatMemberAsync(
            ChatId chatId,
            int userId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new UnbanChatMemberRequest(chatId, userId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Chat> GetChatAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetChatRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<ChatMember[]> GetChatAdministratorsAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetChatAdministratorsRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<int> GetChatMembersCountAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetChatMembersCountRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<ChatMember> GetChatMemberAsync(
            ChatId chatId,
            int userId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetChatMemberRequest(chatId, userId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task AnswerCallbackQueryAsync(
            string callbackQueryId,
            string text = default,
            bool showAlert = default,
            string url = default,
            int cacheTime = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AnswerCallbackQueryRequest(callbackQueryId)
            {
                Text = text,
                ShowAlert = showAlert,
                Url = url,
                CacheTime = cacheTime
            }, cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task RestrictChatMemberAsync(
            ChatId chatId,
            int userId,
            DateTime untilDate = default,
            bool? canSendMessages = default,
            bool? canSendMediaMessages = default,
            bool? canSendOtherMessages = default,
            bool? canAddWebPagePreviews = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new RestrictChatMemberRequest(chatId, userId)
            {
                CanSendMessages = canSendMessages,
                CanSendMediaMessages = canSendMediaMessages,
                CanSendOtherMessages = canSendOtherMessages,
                CanAddWebPagePreviews = canAddWebPagePreviews,
                UntilDate = untilDate
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task PromoteChatMemberAsync(
            ChatId chatId,
            int userId,
            bool? canChangeInfo = default,
            bool? canPostMessages = default,
            bool? canEditMessages = default,
            bool? canDeleteMessages = default,
            bool? canInviteUsers = default,
            bool? canRestrictMembers = default,
            bool? canPinMessages = default,
            bool? canPromoteMembers = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new PromoteChatMemberRequest(chatId, userId)
            {
                CanChangeInfo = canChangeInfo,
                CanPostMessages = canPostMessages,
                CanEditMessages = canEditMessages,
                CanDeleteMessages = canDeleteMessages,
                CanInviteUsers = canInviteUsers,
                CanRestrictMembers = canRestrictMembers,
                CanPinMessages = canPinMessages,
                CanPromoteMembers = canPromoteMembers
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> StopMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new StopMessageLiveLocationRequest(chatId, messageId)
            {
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task StopMessageLiveLocationAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new StopInlineMessageLiveLocationRequest(inlineMessageId)
            {
                ReplyMarkup = replyMarkup
            }, cancellationToken, schedulingMethod);

#endregion Available methods

#region Updating messages

        /// <inheritdoc />
        public Task<Message> EditMessageTextAsync(
            ChatId chatId,
            int messageId,
            string text,
            ParseMode parseMode = default,
            bool disableWebPagePreview = default,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new EditMessageTextRequest(chatId, messageId, text)
            {
                ParseMode = parseMode,
                DisableWebPagePreview = disableWebPagePreview,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task EditMessageTextAsync(
            string inlineMessageId,
            string text,
            ParseMode parseMode = default,
            bool disableWebPagePreview = default,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new EditInlineMessageTextRequest(inlineMessageId, text)
            {
                DisableWebPagePreview = disableWebPagePreview,
                ReplyMarkup = replyMarkup,
                ParseMode = parseMode
            }, cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> EditMessageCaptionAsync(
            ChatId chatId,
            int messageId,
            string caption,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new EditMessageCaptionRequest(chatId, messageId)
            {
                Caption = caption,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task EditMessageCaptionAsync(
            string inlineMessageId,
            string caption,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new EditInlineMessageCaptionRequest(inlineMessageId)
            {
                Caption = caption,
                ReplyMarkup = replyMarkup
            }, cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> EditMessageReplyMarkupAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(
                new EditMessageReplyMarkupRequest(chatId, messageId, replyMarkup),
                cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task EditMessageReplyMarkupAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(
                new EditInlineMessageReplyMarkupRequest(inlineMessageId, replyMarkup),
                cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> EditMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            float latitude,
            float longitude,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new EditMessageLiveLocationRequest(chatId, messageId, latitude, longitude)
            {
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task EditMessageLiveLocationAsync(
            string inlineMessageId,
            float latitude,
            float longitude,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new EditInlineMessageLiveLocationRequest(inlineMessageId, latitude, longitude)
            {
                ReplyMarkup = replyMarkup
            }, cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task DeleteMessageAsync(
            ChatId chatId,
            int messageId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new DeleteMessageRequest(chatId, messageId), cancellationToken, chatId, schedulingMethod);

#endregion Updating messages

#region Inline mode

        /// <inheritdoc />
        public Task AnswerInlineQueryAsync(
            string inlineQueryId,
            IEnumerable<InlineQueryResultBase> results,
            int? cacheTime = default,
            bool isPersonal = default,
            string nextOffset = default,
            string switchPmText = default,
            string switchPmParameter = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AnswerInlineQueryRequest(inlineQueryId, results)
            {
                CacheTime = cacheTime,
                IsPersonal = isPersonal,
                NextOffset = nextOffset,
                SwitchPmText = switchPmText,
                SwitchPmParameter = switchPmParameter
            }, cancellationToken, schedulingMethod);

#endregion Inline mode

#region Payments

        /// <inheritdoc />
        public Task<Message> SendInvoiceAsync(
            int chatId,
            string title,
            string description,
            string payload,
            string providerToken,
            string startParameter,
            string currency,
            IEnumerable<LabeledPrice> prices,
            string providerData = default,
            string photoUrl = default,
            int photoSize = default,
            int photoWidth = default,
            int photoHeight = default,
            bool needName = default,
            bool needPhoneNumber = default,
            bool needEmail = default,
            bool needShippingAddress = default,
            bool isFlexible = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendInvoiceRequest(
                chatId,
                title,
                description,
                payload,
                providerToken,
                startParameter,
                currency,
                // ReSharper disable once PossibleMultipleEnumeration
                prices
            )
            {
                ProviderData = providerData,
                PhotoUrl = photoUrl,
                PhotoSize = photoSize,
                PhotoWidth = photoWidth,
                PhotoHeight = photoHeight,
                NeedName = needName,
                NeedPhoneNumber = needPhoneNumber,
                NeedEmail = needEmail,
                NeedShippingAddress = needShippingAddress,
                IsFlexible = isFlexible,
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task AnswerShippingQueryAsync(
            string shippingQueryId,
            IEnumerable<ShippingOption> shippingOptions,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AnswerShippingQueryRequest(shippingQueryId, shippingOptions), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task AnswerShippingQueryAsync(
            string shippingQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AnswerShippingQueryRequest(shippingQueryId, errorMessage), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AnswerPreCheckoutQueryRequest(preCheckoutQueryId), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AnswerPreCheckoutQueryRequest(preCheckoutQueryId, errorMessage), cancellationToken, schedulingMethod);

#endregion Payments

#region Games

        /// <inheritdoc />
        public Task<Message> SendGameAsync(
            long chatId,
            string gameShortName,
            bool disableNotification = default,
            int replyToMessageId = default,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SendGameRequest(chatId, gameShortName)
            {
                DisableNotification = disableNotification,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup = replyMarkup
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task<Message> SetGameScoreAsync(
            int userId,
            int score,
            long chatId,
            int messageId,
            bool force = default,
            bool disableEditMessage = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetGameScoreRequest(userId, score, chatId, messageId)
            {
                Force = force,
                DisableEditMessage = disableEditMessage
            }, cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task SetGameScoreAsync(
            int userId,
            int score,
            string inlineMessageId,
            bool force = default,
            bool disableEditMessage = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetInlineGameScoreRequest(userId, score, inlineMessageId)
            {
                Force = force,
                DisableEditMessage = disableEditMessage
            }, cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task<GameHighScore[]> GetGameHighScoresAsync(
            int userId,
            long chatId,
            int messageId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(
                new GetGameHighScoresRequest(userId, chatId, messageId),
                cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task<GameHighScore[]> GetGameHighScoresAsync(
            int userId,
            string inlineMessageId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(
                new GetInlineGameHighScoresRequest(userId, inlineMessageId),
                cancellationToken, userId, schedulingMethod);

#endregion Games

#region Group and channel management

        /// <inheritdoc />
        public Task<string> ExportChatInviteLinkAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new ExportChatInviteLinkRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task SetChatPhotoAsync(
            ChatId chatId,
            InputFileStream photo,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetChatPhotoRequest(chatId, photo), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task DeleteChatPhotoAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new DeleteChatPhotoRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task SetChatTitleAsync(
            ChatId chatId,
            string title,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetChatTitleRequest(chatId, title), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task SetChatDescriptionAsync(
            ChatId chatId,
            string description = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetChatDescriptionRequest(chatId, description), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task PinChatMessageAsync(
            ChatId chatId,
            int messageId,
            bool disableNotification = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new PinChatMessageRequest(chatId, messageId)
            {
                DisableNotification = disableNotification
            }, cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task UnpinChatMessageAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new UnpinChatMessageRequest(chatId), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task SetChatStickerSetAsync(
            ChatId chatId,
            string stickerSetName,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetChatStickerSetRequest(chatId, stickerSetName), cancellationToken, chatId, schedulingMethod);

        /// <inheritdoc />
        public Task DeleteChatStickerSetAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new DeleteChatStickerSetRequest(chatId), cancellationToken, chatId, schedulingMethod);

#endregion

#region Stickers

        /// <inheritdoc />
        public Task<StickerSet> GetStickerSetAsync(
            string name,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new GetStickerSetRequest(name), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task<File> UploadStickerFileAsync(
            int userId,
            InputFileStream pngSticker,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new UploadStickerFileRequest(userId, pngSticker), cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task CreateNewStickerSetAsync(
            int userId,
            string name,
            string title,
            InputOnlineFile pngSticker,
            string emojis,
            bool isMasks = default,
            MaskPosition maskPosition = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new CreateNewStickerSetRequest(userId, name, title, pngSticker, emojis)
            {
                ContainsMasks = isMasks,
                MaskPosition = maskPosition
            }, cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task AddStickerToSetAsync(
            int userId,
            string name,
            InputOnlineFile pngSticker,
            string emojis,
            MaskPosition maskPosition = default,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new AddStickerToSetRequest(userId, name, pngSticker, emojis)
            {
                MaskPosition = maskPosition
            }, cancellationToken, userId, schedulingMethod);

        /// <inheritdoc />
        public Task SetStickerPositionInSetAsync(
            string sticker,
            int position,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new SetStickerPositionInSetRequest(sticker, position), cancellationToken, schedulingMethod);

        /// <inheritdoc />
        public Task DeleteStickerFromSetAsync(
            string sticker,
            CancellationToken cancellationToken = default,
            SchedulingMethod schedulingMethod = SchedulingMethod.Normal
        ) =>
            MakeRequestAsync(new DeleteStickerFromSetRequest(sticker), cancellationToken, schedulingMethod);

#endregion
    }
}
