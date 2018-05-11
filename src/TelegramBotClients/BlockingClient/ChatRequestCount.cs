namespace MihaZupan.TelegramBotClients.BlockingClient
{
    internal class ChatRequestCount
    {
        private long LatestInterval;
        private int Value;

        public ChatRequestCount(int value, long currentInterval)
        {
            LatestInterval = currentInterval;
            Value = value;
        }

        public void Increment()
        {
            Value++;
        }

        public int GetRequestCount(long currentInterval)
        {
            long diff = currentInterval - LatestInterval;
            if (diff >= Value)
            {
                Value = 0;
            }
            else
            {
                Value -= (int)diff;
            }
            LatestInterval = currentInterval;
            return Value;
        }
    }
}
