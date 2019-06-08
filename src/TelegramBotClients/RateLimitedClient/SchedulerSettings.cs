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
            SafeGeneralInterval = safeGeneralInterval;
            SafePrivateChatInterval = safePrivateChatInterval;
            SafeGroupChatInterval = safeGroupChatInterval;
        }

        public static readonly SchedulerSettings Default = new SchedulerSettings();
    }
}
