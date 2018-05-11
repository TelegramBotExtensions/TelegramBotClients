using System.Threading;

namespace MihaZupan.TelegramBotClients.BlockingClient
{
    internal class ScheduledRequestItem
    {
        public long TimerIntervalWhenAdded { get; }
        public ManualResetEvent MRE { get; }
        public long ChatId { get; }
        public bool IsHighPriority { get; }

        public ScheduledRequestItem(long timerIntervalWhenAdded, bool isHighPriority, long chatId = 0)
        {
            TimerIntervalWhenAdded = timerIntervalWhenAdded;
            IsHighPriority = isHighPriority;
            ChatId = chatId;
            MRE = new ManualResetEvent(false);
        }
    }
}
