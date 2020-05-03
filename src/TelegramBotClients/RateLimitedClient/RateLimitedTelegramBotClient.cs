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
        public readonly ITelegramBotClient BaseClient;
        public readonly TelegramRequestScheduler RequestScheduler;

        public RateLimitedTelegramBotClient(ITelegramBotClient botClient, SchedulerSettings schedulerSettings = null)
        {
            BaseClient = botClient ?? throw new ArgumentNullException(nameof(botClient));

            RequestScheduler = new TelegramRequestScheduler(schedulerSettings);
        }

        public RateLimitedTelegramBotClient(string token, HttpClient httpClient = null, SchedulerSettings schedulerSettings = null)
            : this(new TelegramBotClient(token, httpClient), schedulerSettings)
        { }

        public RateLimitedTelegramBotClient(string token, IWebProxy webProxy, SchedulerSettings schedulerSettings = null)
            : this(new TelegramBotClient(token, webProxy), schedulerSettings)
        { }

        ~RateLimitedTelegramBotClient()
        {
            RequestScheduler.Stop();
        }


        #region Config Properties

        public int BotId => BaseClient.BotId;

        public TimeSpan Timeout
        {
            get => BaseClient.Timeout;
            set => BaseClient.Timeout = value;
        }

        public bool IsReceiving => BaseClient.IsReceiving;

        public int MessageOffset
        {
            get => BaseClient.MessageOffset;
            set => BaseClient.MessageOffset = value;
        }

        #endregion Config Properties

        #region Events

        public event EventHandler<ApiRequestEventArgs> MakingApiRequest
        {
            add => BaseClient.MakingApiRequest += value;
            remove => BaseClient.MakingApiRequest -= value;
        }

        public event EventHandler<ApiResponseEventArgs> ApiResponseReceived
        {
            add => BaseClient.ApiResponseReceived += value;
            remove => BaseClient.ApiResponseReceived -= value;
        }

        public event EventHandler<UpdateEventArgs> OnUpdate
        {
            add => BaseClient.OnUpdate += value;
            remove => BaseClient.OnUpdate -= value;
        }

        public event EventHandler<MessageEventArgs> OnMessage
        {
            add => BaseClient.OnMessage += value;
            remove => BaseClient.OnMessage -= value;
        }

        public event EventHandler<MessageEventArgs> OnMessageEdited
        {
            add => BaseClient.OnMessageEdited += value;
            remove => BaseClient.OnMessageEdited -= value;
        }

        public event EventHandler<InlineQueryEventArgs> OnInlineQuery
        {
            add => BaseClient.OnInlineQuery += value;
            remove => BaseClient.OnInlineQuery -= value;
        }

        public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen
        {
            add => BaseClient.OnInlineResultChosen += value;
            remove => BaseClient.OnInlineResultChosen -= value;
        }

        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery
        {
            add => BaseClient.OnCallbackQuery += value;
            remove => BaseClient.OnCallbackQuery -= value;
        }

        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError
        {
            add => BaseClient.OnReceiveError += value;
            remove => BaseClient.OnReceiveError -= value;
        }

        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError
        {
            add => BaseClient.OnReceiveGeneralError += value;
            remove => BaseClient.OnReceiveGeneralError -= value;
        }

        #endregion Events

        public void StartReceiving(UpdateType[] allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            BaseClient.StartReceiving(allowedUpdates, cancellationToken);
        }

        public void StopReceiving()
        {
            BaseClient.StopReceiving();
        }

        // __GENERATED__
        #region GENERATED

        public async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddStickerToSetAsync(int userId, string name, InputOnlineFile pngSticker, string emojis, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BaseClient.AddStickerToSetAsync(userId, name, pngSticker, emojis, maskPosition, cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateNewAnimatedStickerSetAsync(int userId, string name, string title, InputFileStream tgsSticker, string emojis,
            bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BaseClient.CreateNewAnimatedStickerSetAsync(userId, name, title, tgsSticker, emojis, isMasks,
                maskPosition, cancellationToken);
        }

        public async Task AddAnimatedStickerToSetAsync(int userId, string name, InputFileStream tgsSticker, string emojis,
            MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BaseClient.AddAnimatedStickerToSetAsync(userId, name, tgsSticker, emojis, maskPosition,
                cancellationToken);
        }

        public async Task AnswerCallbackQueryAsync(string callbackQueryId, string text = null, bool showAlert = false, string url = null, int cacheTime = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.AnswerCallbackQueryAsync(callbackQueryId, text, showAlert, url, cacheTime, cancellationToken).ConfigureAwait(false);
        }

        public async Task AnswerInlineQueryAsync(string inlineQueryId, IEnumerable<InlineQueryResultBase> results, int? cacheTime = null, bool isPersonal = false, string nextOffset = null, string switchPmText = null, string switchPmParameter = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.AnswerInlineQueryAsync(inlineQueryId, results, cacheTime, isPersonal, nextOffset, switchPmText, switchPmParameter, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendInvoiceAsync(int chatId, string title, string description, string payload, string providerToken,
            string startParameter, string currency, IEnumerable<LabeledPrice> prices, string providerData = null, string photoUrl = null,
            int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false,
            bool needEmail = false, bool needShippingAddress = false, bool isFlexible = false, bool disableNotification = false,
            int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default, bool sendPhoneNumberToProvider = default,
            bool sendEmailToProvider = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendInvoiceAsync(chatId, title, description, payload, providerToken, startParameter,
                currency, prices, providerData, photoUrl, photoSize, photoWidth, photoHeight, needName, needPhoneNumber,
                needEmail, needShippingAddress, isFlexible, disableNotification, replyToMessageId, replyMarkup,
                cancellationToken, sendPhoneNumberToProvider, sendEmailToProvider);
        }

        public async Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.AnswerPreCheckoutQueryAsync(preCheckoutQueryId, cancellationToken).ConfigureAwait(false);
        }

        public async Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, string errorMessage, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.AnswerPreCheckoutQueryAsync(preCheckoutQueryId, errorMessage, cancellationToken).ConfigureAwait(false);
        }

        public async Task AnswerShippingQueryAsync(string shippingQueryId, IEnumerable<ShippingOption> shippingOptions, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.AnswerShippingQueryAsync(shippingQueryId, shippingOptions, cancellationToken).ConfigureAwait(false);
        }

        public async Task AnswerShippingQueryAsync(string shippingQueryId, string errorMessage, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.AnswerShippingQueryAsync(shippingQueryId, errorMessage, cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateNewStickerSetAsync(int userId, string name, string title, InputOnlineFile pngSticker, string emojis, bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BaseClient.CreateNewStickerSetAsync(userId, name, title, pngSticker, emojis, isMasks, maskPosition, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteChatPhotoAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.DeleteChatPhotoAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteChatStickerSetAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.DeleteChatStickerSetAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteMessageAsync(ChatId chatId, int messageId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.DeleteMessageAsync(chatId, messageId, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteStickerFromSetAsync(string sticker, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.DeleteStickerFromSetAsync(sticker, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetStickerSetThumbAsync(string name, int userId, InputOnlineFile thumb = null,
            CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BaseClient.SetStickerSetThumbAsync(name, userId, thumb, cancellationToken).ConfigureAwait(false);        
        }

        public async Task DeleteWebhookAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.DeleteWebhookAsync(cancellationToken).ConfigureAwait(false);
        }

        [Obsolete]
        public async Task<Stream> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.DownloadFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        }

        public async Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.DownloadFileAsync(filePath, destination, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> EditMessageCaptionAsync(ChatId chatId, int messageId, string caption, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default, ParseMode parseMode = ParseMode.Default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.EditMessageCaptionAsync(chatId, messageId, caption, replyMarkup, cancellationToken, parseMode).ConfigureAwait(false);
        }

        public async Task EditMessageCaptionAsync(string inlineMessageId, string caption, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default, ParseMode parseMode = ParseMode.Default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.EditMessageCaptionAsync(inlineMessageId, caption, replyMarkup, cancellationToken, parseMode).ConfigureAwait(false);
        }

        public async Task<Message> EditMessageLiveLocationAsync(ChatId chatId, int messageId, float latitude, float longitude, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.EditMessageLiveLocationAsync(chatId, messageId, latitude, longitude, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task EditMessageLiveLocationAsync(string inlineMessageId, float latitude, float longitude, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.EditMessageLiveLocationAsync(inlineMessageId, latitude, longitude, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> EditMessageMediaAsync(ChatId chatId, int messageId, InputMediaBase media, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.EditMessageMediaAsync(chatId, messageId, media, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task EditMessageMediaAsync(string inlineMessageId, InputMediaBase media, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.EditMessageMediaAsync(inlineMessageId, media, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> EditMessageReplyMarkupAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task EditMessageReplyMarkupAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.EditMessageReplyMarkupAsync(inlineMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> EditMessageTextAsync(ChatId chatId, int messageId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.EditMessageTextAsync(chatId, messageId, text, parseMode, disableWebPagePreview, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task EditMessageTextAsync(string inlineMessageId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.EditMessageTextAsync(inlineMessageId, text, parseMode, disableWebPagePreview, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> ExportChatInviteLinkAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.ExportChatInviteLinkAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> ForwardMessageAsync(ChatId chatId, ChatId fromChatId, int messageId, bool disableNotification = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.ForwardMessageAsync(chatId, fromChatId, messageId, disableNotification, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ChatMember[]> GetChatAdministratorsAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.GetChatAdministratorsAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.GetChatAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ChatMember> GetChatMemberAsync(ChatId chatId, int userId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.GetChatMemberAsync(chatId, userId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetChatMembersCountAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.GetChatMembersCountAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Telegram.Bot.Types.File> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetFileAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<GameHighScore[]> GetGameHighScoresAsync(int userId, long chatId, int messageId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.GetGameHighScoresAsync(userId, chatId, messageId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<GameHighScore[]> GetGameHighScoresAsync(int userId, string inlineMessageId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            return await BaseClient.GetGameHighScoresAsync(userId, inlineMessageId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Telegram.Bot.Types.File> GetInfoAndDownloadFileAsync(string fileId, Stream destination, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetInfoAndDownloadFileAsync(fileId, destination, cancellationToken).ConfigureAwait(false);
        }

        public async Task<User> GetMeAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetMeAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<StickerSet> GetStickerSetAsync(string name, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetStickerSetAsync(name, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Update[]> GetUpdatesAsync(int offset = 0, int limit = 0, int timeout = 0, IEnumerable<UpdateType> allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetUpdatesAsync(offset, limit, timeout, allowedUpdates, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserProfilePhotos> GetUserProfilePhotosAsync(int userId, int offset = 0, int limit = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            return await BaseClient.GetUserProfilePhotosAsync(userId, offset, limit, cancellationToken).ConfigureAwait(false);
        }

        public async Task<WebhookInfo> GetWebhookInfoAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetWebhookInfoAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task KickChatMemberAsync(ChatId chatId, int userId, DateTime untilDate = default, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.KickChatMemberAsync(chatId, userId, untilDate, cancellationToken).ConfigureAwait(false);
        }

        public async Task LeaveChatAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.LeaveChatAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task PinChatMessageAsync(ChatId chatId, int messageId, bool disableNotification = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.PinChatMessageAsync(chatId, messageId, disableNotification, cancellationToken).ConfigureAwait(false);
        }

        public async Task PromoteChatMemberAsync(ChatId chatId, int userId, bool? canChangeInfo = null, bool? canPostMessages = null, bool? canEditMessages = null, bool? canDeleteMessages = null, bool? canInviteUsers = null, bool? canRestrictMembers = null, bool? canPinMessages = null, bool? canPromoteMembers = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.PromoteChatMemberAsync(chatId, userId, canChangeInfo, canPostMessages, canEditMessages, canDeleteMessages, canInviteUsers, canRestrictMembers, canPinMessages, canPromoteMembers, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetChatAdministratorCustomTitleAsync(ChatId chatId, int userId, string customTitle,
            CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SetChatAdministratorCustomTitleAsync(chatId, userId, customTitle, cancellationToken);
        }

        public async Task<Message> SendAnimationAsync(ChatId chatId, InputOnlineFile animation, int duration = 0, int width = 0, int height = 0, InputMedia thumb = null, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendAnimationAsync(chatId, animation, duration, width, height, thumb, caption, parseMode, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendAudioAsync(ChatId chatId, InputOnlineFile audio, string caption = null, ParseMode parseMode = ParseMode.Default, int duration = 0, string performer = null, string title = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendAudioAsync(chatId, audio, caption, parseMode, duration, performer, title, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        public async Task<Message> SendDiceAsync(ChatId chatId, bool disableNotification = false, int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendDiceAsync(chatId, disableNotification, replyToMessageId, replyMarkup,
                cancellationToken);
        }

        public async Task SendChatActionAsync(ChatId chatId, ChatAction chatAction, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SendChatActionAsync(chatId, chatAction, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendContactAsync(ChatId chatId, string phoneNumber, string firstName, string lastName = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, string vCard = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendContactAsync(chatId, phoneNumber, firstName, lastName, disableNotification, replyToMessageId, replyMarkup, cancellationToken, vCard).ConfigureAwait(false);
        }

        public async Task<Message> SendDocumentAsync(ChatId chatId, InputOnlineFile document, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendDocumentAsync(chatId, document, caption, parseMode, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        public async Task<Message> SendGameAsync(long chatId, string gameShortName, bool disableNotification = false, int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendGameAsync(chatId, gameShortName, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendLocationAsync(ChatId chatId, float latitude, float longitude, int livePeriod = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendLocationAsync(chatId, latitude, longitude, livePeriod, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete]
        public async Task<Message[]> SendMediaGroupAsync(ChatId chatId, IEnumerable<InputMediaBase> media, bool disableNotification = false, int replyToMessageId = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendMediaGroupAsync(chatId, media, disableNotification, replyToMessageId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message[]> SendMediaGroupAsync(IEnumerable<IAlbumInputMedia> inputMedia, ChatId chatId, bool disableNotification = false, int replyToMessageId = 0, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendMediaGroupAsync(inputMedia, chatId, disableNotification, replyToMessageId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendPhotoAsync(ChatId chatId, InputOnlineFile photo, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendPhotoAsync(chatId, photo, caption, parseMode, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendPollAsync(ChatId chatId, string question, IEnumerable<string> options, 
            bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, 
            CancellationToken cancellationToken = default, bool? isAnonymous = null, PollType? type = null, 
            bool? allowsMultipleAnswers = null, int? correctOptionId = null, bool? isClosed = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendPollAsync(chatId, question, options, disableNotification, replyToMessageId, replyMarkup, cancellationToken, isAnonymous, type, allowsMultipleAnswers, correctOptionId, isClosed).ConfigureAwait(false);
        }

        public async Task<Message> SendStickerAsync(ChatId chatId, InputOnlineFile sticker, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendStickerAsync(chatId, sticker, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SendVenueAsync(ChatId chatId, float latitude, float longitude, string title, string address, string foursquareId = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, string foursquareType = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendVenueAsync(chatId, latitude, longitude, title, address, foursquareId, disableNotification, replyToMessageId, replyMarkup, cancellationToken, foursquareType).ConfigureAwait(false);
        }

        public async Task<Message> SendVideoAsync(ChatId chatId, InputOnlineFile video, int duration = 0, int width = 0, int height = 0, string caption = null, ParseMode parseMode = ParseMode.Default, bool supportsStreaming = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendVideoAsync(chatId, video, duration, width, height, caption, parseMode, supportsStreaming, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        public async Task<Message> SendVideoNoteAsync(ChatId chatId, InputTelegramFile videoNote, int duration = 0, int length = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, InputMedia thumb = null)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendVideoNoteAsync(chatId, videoNote, duration, length, disableNotification, replyToMessageId, replyMarkup, cancellationToken, thumb).ConfigureAwait(false);
        }

        public async Task<Message> SendVoiceAsync(ChatId chatId, InputOnlineFile voice, string caption = null, ParseMode parseMode = ParseMode.Default, int duration = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SendVoiceAsync(chatId, voice, caption, parseMode, duration, disableNotification, replyToMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetChatDescriptionAsync(ChatId chatId, string description = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SetChatDescriptionAsync(chatId, description, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetChatPhotoAsync(ChatId chatId, InputFileStream photo, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SetChatPhotoAsync(chatId, photo, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetChatStickerSetAsync(ChatId chatId, string stickerSetName, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SetChatStickerSetAsync(chatId, stickerSetName, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetChatTitleAsync(ChatId chatId, string title, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SetChatTitleAsync(chatId, title, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> SetGameScoreAsync(int userId, int score, long chatId, int messageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.SetGameScoreAsync(userId, score, chatId, messageId, force, disableEditMessage, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetGameScoreAsync(int userId, int score, string inlineMessageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            await BaseClient.SetGameScoreAsync(userId, score, inlineMessageId, force, disableEditMessage, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetStickerPositionInSetAsync(string sticker, int position, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.SetStickerPositionInSetAsync(sticker, position, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetWebhookAsync(string url, InputFileStream certificate = null, int maxConnections = 0, IEnumerable<UpdateType> allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.SetWebhookAsync(url, certificate, maxConnections, allowedUpdates, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message> StopMessageLiveLocationAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.StopMessageLiveLocationAsync(chatId, messageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task StopMessageLiveLocationAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.StopMessageLiveLocationAsync(inlineMessageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Poll> StopPollAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            return await BaseClient.StopPollAsync(chatId, messageId, replyMarkup, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> TestApiAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.TestApiAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task UnbanChatMemberAsync(ChatId chatId, int userId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.UnbanChatMemberAsync(chatId, userId, cancellationToken).ConfigureAwait(false);
        }

        public async Task UnpinChatMessageAsync(ChatId chatId, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.UnpinChatMessageAsync(chatId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Telegram.Bot.Types.File> UploadStickerFileAsync(int userId, InputFileStream pngSticker, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(userId).ConfigureAwait(false);

            return await BaseClient.UploadStickerFileAsync(userId, pngSticker, cancellationToken).ConfigureAwait(false);
        }

        public async Task RestrictChatMemberAsync(ChatId chatId, int userId, ChatPermissions permissions, DateTime untilDate = default, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.RestrictChatMemberAsync(chatId, userId, permissions, untilDate, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetChatPermissionsAsync(ChatId chatId, ChatPermissions permissions, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync(chatId).ConfigureAwait(false);

            await BaseClient.SetChatPermissionsAsync(chatId, permissions, cancellationToken).ConfigureAwait(false);
        }

        public async Task SetMyCommandsAsync(IEnumerable<BotCommand> commands, CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            await BaseClient.SetMyCommandsAsync(commands, cancellationToken);
        }
        
        public async Task<BotCommand[]> GetMyCommandsAsync(CancellationToken cancellationToken = default)
        {
            await RequestScheduler.YieldAsync().ConfigureAwait(false);

            return await BaseClient.GetMyCommandsAsync(cancellationToken);        
        }

        #endregion GENERATED
    }
}
