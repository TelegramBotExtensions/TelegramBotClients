using System;

namespace MihaZupan.TelegramBotClients.RateLimitedClient
{
    public class SchedulerSettings
    {
        public readonly int SafeGeneralInterval;
        public readonly int SafePrivateChatInterval;
        public readonly int SafeGroupChatInterval;

        public SchedulerSettings(
            int safeGeneralInterval = 34,
            int safePrivateChatInterval = 1000,
            int safeGroupChatInterval = 3000)
        {
            if (safeGeneralInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(safeGeneralInterval), "Must be a positive number");

            if (safePrivateChatInterval < safeGeneralInterval)
                throw new ArgumentOutOfRangeException(nameof(safePrivateChatInterval), "Must not be lower than the general interval");

            if (safeGroupChatInterval < safeGeneralInterval)
                throw new ArgumentOutOfRangeException(nameof(safeGroupChatInterval), "Must not be lower than the general interval");

            SafeGeneralInterval = safeGeneralInterval;
            SafePrivateChatInterval = safePrivateChatInterval;
            SafeGroupChatInterval = safeGroupChatInterval;
        }

        public static readonly SchedulerSettings Default = new SchedulerSettings();
    }
}
