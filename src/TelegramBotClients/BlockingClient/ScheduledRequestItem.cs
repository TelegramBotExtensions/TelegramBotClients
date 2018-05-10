using System.Threading;

namespace MihaZupan.TelegramBotClients.BlockingClient
{
    class ScheduledRequestItem
    {
        public long TimerIntervalWhenAdded { get; private set; }
        public ManualResetEvent MRE { get; private set; }
        public long ChatId { get; private set; }
        public bool IsHighPriority { get; private set; }

        public ScheduledRequestItem(long timerIntervalWhenAdded, bool isHighPriority, long chatId = 0)
        {
            TimerIntervalWhenAdded = timerIntervalWhenAdded;
            IsHighPriority = isHighPriority;
            ChatId = chatId;
            MRE = new ManualResetEvent(false);
        }
    }
}
