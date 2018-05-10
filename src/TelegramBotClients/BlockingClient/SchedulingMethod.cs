namespace MihaZupan.TelegramBotClients.BlockingClient
{
    public enum SchedulingMethod
    {
        /// <summary>
        /// Default behaviour. May block the caller if rate-limiting is active
        /// </summary>
        Normal,

        /// <summary>
        /// Same as normal but higher priority. Will be put at the front of the queue if rate-limiting is active
        /// </summary>
        HighPriority,

        /// <summary>
        /// Let the request through immediately. Will compensate by pushing back other requests
        /// </summary>
        NoScheduling,

        /// <summary>
        /// Let the request through immediately and completely ignore the scheduling system
        /// </summary>
        Ignore
    }
}
