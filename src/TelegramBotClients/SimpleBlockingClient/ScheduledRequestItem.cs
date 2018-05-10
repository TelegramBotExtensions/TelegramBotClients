using System.Threading;

namespace MihaZupan.TelegramBotClients
{
    class ScheduledRequestItem
    {
        public long TimerIntervalWhenAdded { get; private set; }
        public ManualResetEvent MRE { get; private set; }
        public long ChatId { get; private set; }

        public ScheduledRequestItem(long timerIntervalWhenAdded, long chatId = 0)
        {
            TimerIntervalWhenAdded = timerIntervalWhenAdded;
            ChatId = chatId;
            MRE = new ManualResetEvent(false);
        }
    }
}
