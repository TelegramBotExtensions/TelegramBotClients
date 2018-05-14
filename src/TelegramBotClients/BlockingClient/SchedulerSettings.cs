namespace MihaZupan.TelegramBotClients.BlockingClient
{
    public class SchedulerSettings
    {
        public readonly int SafeGeneralInterval;
        public readonly int GeneralMaxBurst;

        public readonly int SafePrivateChatInterval;
        public readonly int PrivateChatMaxBurst;

        public readonly int SafeGroupChatInterval;
        public readonly int GroupChatMaxBurst;

        public SchedulerSettings(
            int safeGeneralInterval = 34,
            int generalMaxBurts = 5,
            int safePrivateChatInterval = 1000,
            int privateChatMaxBurst = 3,
            int safeGroupChatInterval = 3000,
            int groupChatMaxBurst = 3)
        {
            SafeGeneralInterval = safeGeneralInterval;
            GeneralMaxBurst = generalMaxBurts;
            SafePrivateChatInterval = safePrivateChatInterval;
            PrivateChatMaxBurst = privateChatMaxBurst;
            SafeGroupChatInterval = safeGroupChatInterval;
            GroupChatMaxBurst = groupChatMaxBurst;
        }

        public static readonly SchedulerSettings Default = new SchedulerSettings();
    }
}
