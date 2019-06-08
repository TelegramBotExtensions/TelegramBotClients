// Copyright (c) Miha Zupan. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE file in the project root for more information.
using MihaZupan.TelegramBotClients.RateLimitedClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace MihaZupan.TelegramBotClients
{
    public class RateLimitedTelegramBotClient : ITelegramBotClient
    {
        public readonly ITelegramBotClient BotClient;
        public readonly TelegramRequestScheduler RequestScheduler;

        public RateLimitedTelegramBotClient(ITelegramBotClient botClient, SchedulerSettings schedulerSettings = null)
        {
            BotClient = botClient ?? throw new ArgumentNullException(nameof(botClient));

            RequestScheduler = new TelegramRequestScheduler(schedulerSettings);
        }

        public RateLimitedTelegramBotClient(string token, HttpClient httpClient = null, SchedulerSettings schedulerSettings = null)
            : this(new TelegramBotClient(token, httpClient), schedulerSettings)
        { }

        public RateLimitedTelegramBotClient(string token, IWebProxy webProxy, SchedulerSettings schedulerSettings = null)
            : this(new TelegramBotClient(token, webProxy), schedulerSettings)
        { }


        #region Config Properties

        /// <inheritdoc />
        public int BotId
        {
            get => BotClient.BotId;
        }

        /// <inheritdoc />
        public TimeSpan Timeout
        {
            get => BotClient.Timeout;
            set => BotClient.Timeout = value;
        }

        /// <inheritdoc />
        public bool IsReceiving
        {
            get => BotClient.IsReceiving;
        }

        /// <inheritdoc />
        public int MessageOffset
        {
            get => BotClient.MessageOffset;
            set => BotClient.MessageOffset = value;
        }

        #endregion Config Properties

        #region Events

        /// <inheritdoc />
        public event EventHandler<ApiRequestEventArgs> MakingApiRequest
        {
            add => BotClient.MakingApiRequest += value;
            remove => BotClient.MakingApiRequest -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ApiResponseEventArgs> ApiResponseReceived
        {
            add => BotClient.ApiResponseReceived += value;
            remove => BotClient.ApiResponseReceived -= value;
        }

        /// <inheritdoc />
        public event EventHandler<UpdateEventArgs> OnUpdate
        {
            add => BotClient.OnUpdate += value;
            remove => BotClient.OnUpdate -= value;
        }

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs> OnMessage
        {
            add => BotClient.OnMessage += value;
            remove => BotClient.OnMessage -= value;
        }

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs> OnMessageEdited
        {
            add => BotClient.OnMessageEdited += value;
            remove => BotClient.OnMessageEdited -= value;
        }

        /// <inheritdoc />
        public event EventHandler<InlineQueryEventArgs> OnInlineQuery
        {
            add => BotClient.OnInlineQuery += value;
            remove => BotClient.OnInlineQuery -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen
        {
            add => BotClient.OnInlineResultChosen += value;
            remove => BotClient.OnInlineResultChosen -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery
        {
            add => BotClient.OnCallbackQuery += value;
            remove => BotClient.OnCallbackQuery -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError
        {
            add => BotClient.OnReceiveError += value;
            remove => BotClient.OnReceiveError -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError
        {
            add => BotClient.OnReceiveGeneralError += value;
            remove => BotClient.OnReceiveGeneralError -= value;
        }

        #endregion Events

        public void StartReceiving(UpdateType[] allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            BotClient.StartReceiving(allowedUpdates, cancellationToken);
        }

        public void StopReceiving()
        {
            BotClient.StopReceiving();
        }

        // __GENERATED__
        #region GENERATED

        /// <inheritdoc />
        public async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AddStickerToSetAsync(int userId, string name, InputOnlineFile pngSticker, string emojis, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BotClient.AddStickerToSetAsync(userId, name, pngSticker, emojis, maskPosition, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AnswerCallbackQueryAsync(string callbackQueryId, string text = null, bool showAlert = false, string url = null, int cacheTime = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.AnswerCallbackQueryAsync(callbackQueryId, text, showAlert, url, cacheTime, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AnswerInlineQueryAsync(string inlineQueryId, IEnumerable<InlineQueryResultBase> results, int? cacheTime = null, bool isPersonal = false, string nextOffset = null, string switchPmText = null, string switchPmParameter = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.AnswerInlineQueryAsync(inlineQueryId, results, cacheTime, isPersonal, nextOffset, switchPmText, switchPmParameter, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.AnswerPreCheckoutQueryAsync(preCheckoutQueryId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, string errorMessage, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.AnswerPreCheckoutQueryAsync(preCheckoutQueryId, errorMessage, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AnswerShippingQueryAsync(string shippingQueryId, IEnumerable<ShippingOption> shippingOptions, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.AnswerShippingQueryAsync(shippingQueryId, shippingOptions, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AnswerShippingQueryAsync(string shippingQueryId, string errorMessage, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.AnswerShippingQueryAsync(shippingQueryId, errorMessage, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task CreateNewStickerSetAsync(int userId, string name, string title, InputOnlineFile pngSticker, string emojis, bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BotClient.CreateNewStickerSetAsync(userId, name, title, pngSticker, emojis, isMasks, maskPosition, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteChatPhotoAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.DeleteChatPhotoAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteChatStickerSetAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.DeleteChatStickerSetAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteMessageAsync(ChatId chatId, int messageId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.DeleteMessageAsync(chatId, messageId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteStickerFromSetAsync(string sticker, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.DeleteStickerFromSetAsync(sticker, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteWebhookAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.DeleteWebhookAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        [Obsolete]
        public async Task<Stream> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.DownloadFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.DownloadFileAsync(filePath, destination, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> EditMessageCaptionAsync(ChatId chatId, int messageId, string caption, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default, ParseMode parseMode = ParseMode.Default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.EditMessageCaptionAsync(chatId, messageId, caption, replyMarkup, cancellationToken, parseMode).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EditMessageCaptionAsync(string inlineMessageId, string caption, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default, ParseMode parseMode = ParseMode.Default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.EditMessageCaptionAsync(inlineMessageId, caption, replyMarkup, cancellationToken, parseMode).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> EditMessageLiveLocationAsync(ChatId chatId, int messageId, float latitude, float longitude, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.EditMessageLiveLocationAsync(chatId, messageId, latitude, longitude, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EditMessageLiveLocationAsync(string inlineMessageId, float latitude, float longitude, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.EditMessageLiveLocationAsync(inlineMessageId, latitude, longitude, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> EditMessageMediaAsync(ChatId chatId, int messageId, InputMediaBase media, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.EditMessageMediaAsync(chatId, messageId, media, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EditMessageMediaAsync(string inlineMessageId, InputMediaBase media, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.EditMessageMediaAsync(inlineMessageId, media, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> EditMessageReplyMarkupAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EditMessageReplyMarkupAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.EditMessageReplyMarkupAsync(inlineMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> EditMessageTextAsync(ChatId chatId, int messageId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.EditMessageTextAsync(chatId, messageId, text, parseMode, disableWebPagePreview, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EditMessageTextAsync(string inlineMessageId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.EditMessageTextAsync(inlineMessageId, text, parseMode, disableWebPagePreview, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<string> ExportChatInviteLinkAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.ExportChatInviteLinkAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> ForwardMessageAsync(ChatId chatId, ChatId fromChatId, int messageId, bool disableNotification = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.ForwardMessageAsync(chatId, fromChatId, messageId, disableNotification, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ChatMember[]> GetChatAdministratorsAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.GetChatAdministratorsAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.GetChatAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ChatMember> GetChatMemberAsync(ChatId chatId, int userId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.GetChatMemberAsync(chatId, userId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<int> GetChatMembersCountAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.GetChatMembersCountAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Telegram.Bot.Types.File> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.GetFileAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<GameHighScore[]> GetGameHighScoresAsync(int userId, long chatId, int messageId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.GetGameHighScoresAsync(userId, chatId, messageId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<GameHighScore[]> GetGameHighScoresAsync(int userId, string inlineMessageId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            return await BotClient.GetGameHighScoresAsync(userId, inlineMessageId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Telegram.Bot.Types.File> GetInfoAndDownloadFileAsync(string fileId, Stream destination, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.GetInfoAndDownloadFileAsync(fileId, destination, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<User> GetMeAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.GetMeAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StickerSet> GetStickerSetAsync(string name, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.GetStickerSetAsync(name, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Update[]> GetUpdatesAsync(int offset = 0, int limit = 0, int timeout = 0, IEnumerable<UpdateType> allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.GetUpdatesAsync(offset, limit, timeout, allowedUpdates, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<UserProfilePhotos> GetUserProfilePhotosAsync(int userId, int offset = 0, int limit = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            return await BotClient.GetUserProfilePhotosAsync(userId, offset, limit, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebhookInfo> GetWebhookInfoAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.GetWebhookInfoAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task KickChatMemberAsync(ChatId chatId, int userId, DateTime untilDate = default, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.KickChatMemberAsync(chatId, userId, untilDate, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LeaveChatAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.LeaveChatAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task PinChatMessageAsync(ChatId chatId, int messageId, bool disableNotification = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.PinChatMessageAsync(chatId, messageId, disableNotification, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task PromoteChatMemberAsync(ChatId chatId, int userId, bool? canChangeInfo = null, bool? canPostMessages = null, bool? canEditMessages = null, bool? canDeleteMessages = null, bool? canInviteUsers = null, bool? canRestrictMembers = null, bool? canPinMessages = null, bool? canPromoteMembers = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.PromoteChatMemberAsync(chatId, userId, canChangeInfo, canPostMessages, canEditMessages, canDeleteMessages, canInviteUsers, canRestrictMembers, canPinMessages, canPromoteMembers, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task RestrictChatMemberAsync(ChatId chatId, int userId, DateTime untilDate = default, bool? canSendMessages = null, bool? canSendMediaMessages = null, bool? canSendOtherMessages = null, bool? canAddWebPagePreviews = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.RestrictChatMemberAsync(chatId, userId, untilDate, canSendMessages, canSendMediaMessages, canSendOtherMessages, canAddWebPagePreviews, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendAnimationAsync(ChatId chatId, InputOnlineFile animation, int duration = 0, int width = 0, int height = 0, InputMedia thumb = null, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendAnimationAsync(chatId, animation, duration, width, height, thumb, caption, parseMode, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendAudioAsync(ChatId chatId, InputOnlineFile audio, string caption = null, ParseMode parseMode = ParseMode.Default, int duration = 0, string performer = null, string title = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendAudioAsync(chatId, audio, caption, parseMode, duration, performer, title, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SendChatActionAsync(ChatId chatId, ChatAction chatAction, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.SendChatActionAsync(chatId, chatAction, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendContactAsync(ChatId chatId, string phoneNumber, string firstName, string lastName = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, string vCard = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendContactAsync(chatId, phoneNumber, firstName, lastName, disableNotification, replyToMessageId, replyMarkup, cancellationToken, vCard).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendDocumentAsync(ChatId chatId, InputOnlineFile document, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendDocumentAsync(chatId, document, caption, parseMode, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendGameAsync(long chatId, string gameShortName, bool disableNotification = false, int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendGameAsync(chatId, gameShortName, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendInvoiceAsync(int chatId, string title, string description, string payload, string providerToken, string startParameter, string currency, IEnumerable<LabeledPrice> prices, string providerData = null, string photoUrl = null, int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false, bool needEmail = false, bool needShippingAddress = false, bool isFlexible = false, bool disableNotification = false, int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendInvoiceAsync(chatId, title, description, payload, providerToken, startParameter, currency, prices, providerData, photoUrl, photoSize, photoWidth, photoHeight, needName, needPhoneNumber, needEmail, needShippingAddress, isFlexible, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendLocationAsync(ChatId chatId, float latitude, float longitude, int livePeriod = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendLocationAsync(chatId, latitude, longitude, livePeriod, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        [Obsolete]
        public async Task<Message[]> SendMediaGroupAsync(ChatId chatId, IEnumerable<InputMediaBase> media, bool disableNotification = false, int replyToMessageId = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendMediaGroupAsync(chatId, media, disableNotification, replyToMessageId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message[]> SendMediaGroupAsync(IEnumerable<IAlbumInputMedia> inputMedia, ChatId chatId, bool disableNotification = false, int replyToMessageId = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendMediaGroupAsync(inputMedia, chatId, disableNotification, replyToMessageId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendPhotoAsync(ChatId chatId, InputOnlineFile photo, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendPhotoAsync(chatId, photo, caption, parseMode, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendPollAsync(ChatId chatId, string question, IEnumerable<string> options, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendPollAsync(chatId, question, options, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendStickerAsync(ChatId chatId, InputOnlineFile sticker, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendStickerAsync(chatId, sticker, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendVenueAsync(ChatId chatId, float latitude, float longitude, string title, string address, string foursquareId = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, string foursquareType = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendVenueAsync(chatId, latitude, longitude, title, address, foursquareId, disableNotification, replyToMessageId, replyMarkup, cancellationToken, foursquareType).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendVideoAsync(ChatId chatId, InputOnlineFile video, int duration = 0, int width = 0, int height = 0, string caption = null, ParseMode parseMode = ParseMode.Default, bool supportsStreaming = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendVideoAsync(chatId, video, duration, width, height, caption, parseMode, supportsStreaming, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendVideoNoteAsync(ChatId chatId, InputTelegramFile videoNote, int duration = 0, int length = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendVideoNoteAsync(chatId, videoNote, duration, length, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SendVoiceAsync(ChatId chatId, InputOnlineFile voice, string caption = null, ParseMode parseMode = ParseMode.Default, int duration = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SendVoiceAsync(chatId, voice, caption, parseMode, duration, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetChatDescriptionAsync(ChatId chatId, string description = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.SetChatDescriptionAsync(chatId, description, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetChatPhotoAsync(ChatId chatId, InputFileStream photo, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.SetChatPhotoAsync(chatId, photo, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetChatStickerSetAsync(ChatId chatId, string stickerSetName, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.SetChatStickerSetAsync(chatId, stickerSetName, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetChatTitleAsync(ChatId chatId, string title, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.SetChatTitleAsync(chatId, title, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> SetGameScoreAsync(int userId, int score, long chatId, int messageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.SetGameScoreAsync(userId, score, chatId, messageId, force, disableEditMessage, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetGameScoreAsync(int userId, int score, string inlineMessageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BotClient.SetGameScoreAsync(userId, score, inlineMessageId, force, disableEditMessage, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetStickerPositionInSetAsync(string sticker, int position, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.SetStickerPositionInSetAsync(sticker, position, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetWebhookAsync(string url, InputFileStream certificate = null, int maxConnections = 0, IEnumerable<UpdateType> allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.SetWebhookAsync(url, certificate, maxConnections, allowedUpdates, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Message> StopMessageLiveLocationAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.StopMessageLiveLocationAsync(chatId, messageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task StopMessageLiveLocationAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BotClient.StopMessageLiveLocationAsync(inlineMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Poll> StopPollAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BotClient.StopPollAsync(chatId, messageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> TestApiAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BotClient.TestApiAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task UnbanChatMemberAsync(ChatId chatId, int userId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.UnbanChatMemberAsync(chatId, userId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task UnpinChatMessageAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BotClient.UnpinChatMessageAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Telegram.Bot.Types.File> UploadStickerFileAsync(int userId, InputFileStream pngSticker, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            return await BotClient.UploadStickerFileAsync(userId, pngSticker, cancellationToken).ConfigureAwait(false);
        }

        #endregion GENERATED
    }
}
