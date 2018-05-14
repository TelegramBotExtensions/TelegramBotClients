namespace MihaZupan.TelegramBotClients.BlockingClient
{
    internal class ChatRequestCount
    {
        private long _latestInterval;
        private int _requestCount;

        public ChatRequestCount(int requestCount, long currentInterval)
        {
            _latestInterval = currentInterval;
            _requestCount = requestCount;
        }

        public void Increment()
        {
            _requestCount++;
        }

        public int GetRequestCount(long currentInterval)
        {
            long diff = currentInterval - _latestInterval;
            if (diff >= _requestCount)
            {
                _requestCount = 0;
            }
            else
            {
                _requestCount -= (int)diff;
            }
            _latestInterval = currentInterval;
            return _requestCount;
        }
    }
}
