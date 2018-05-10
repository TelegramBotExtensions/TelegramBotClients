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
            int safeGeneralInterval = 35,
            int generalMaxBurts = 10,
            int safePrivateChatInterval = 1000,
            int privateChatMaxBurst = 5,
            int safeGroupChatInterval = 3000,
            int groupChatMaxBurst = 5)
        {
            SafeGeneralInterval = safeGeneralInterval;
            GeneralMaxBurst = generalMaxBurts;
            SafePrivateChatInterval = safeGeneralInterval;
            PrivateChatMaxBurst = privateChatMaxBurst;
            SafeGroupChatInterval = safeGroupChatInterval;
            GroupChatMaxBurst = groupChatMaxBurst;
        }

        public static readonly SchedulerSettings Default = new SchedulerSettings();
    }
}
