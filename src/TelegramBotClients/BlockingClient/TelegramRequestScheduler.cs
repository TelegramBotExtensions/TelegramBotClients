using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Telegram.Bot.Types;

namespace MihaZupan.TelegramBotClients.BlockingClient
{
    public class TelegramRequestScheduler
    {
        public readonly int SafeGeneralInterval;
        public readonly int GeneralMaxBurst;

        public readonly int SafePrivateChatInterval;
        public readonly int PrivateChatMaxBurst;

        public readonly int SafeGroupChatInterval;
        public readonly int GroupChatMaxBurst;

        private long TimerIntervals = 0;
        private long PrivateChatIntervals = 0;
        private long GroupChatIntervals = 0;

        // Amount of all sent requests that have not yet been written off
        private int GeneralRequestCount = 0;

        private readonly object QueueLock = new object();

        private readonly Timer Timer;
        private readonly Stopwatch Stopwatch;

        private readonly LinkedList<ScheduledRequestItem> PrivateChatQueue = new LinkedList<ScheduledRequestItem>();
        private readonly LinkedList<ScheduledRequestItem> GroupChatQueue = new LinkedList<ScheduledRequestItem>();
        private readonly LinkedList<ScheduledRequestItem> GeneralQueue = new LinkedList<ScheduledRequestItem>();

        private readonly Dictionary<long, ChatRequestCount> RequestCounts = new Dictionary<long, ChatRequestCount>(50);

        private void TimerCallback(object state)
        {
            TimerIntervals++;
            long msElapsed = Stopwatch.ElapsedMilliseconds;

            bool CanUpdatePrivateChats = msElapsed > (PrivateChatIntervals + 1) * SafePrivateChatInterval;
            if (CanUpdatePrivateChats) PrivateChatIntervals++;
            bool CanUpdateGroupChats = msElapsed > (GroupChatIntervals + 1) * SafeGroupChatInterval;
            if (CanUpdateGroupChats) GroupChatIntervals++;

            lock (QueueLock)
            {
                if (CanUpdatePrivateChats && PrivateChatQueue.Count != 0)
                {
                    ProcessQueue(PrivateChatQueue, PrivateChatIntervals, PrivateChatMaxBurst);
                }

                if (CanUpdateGroupChats && GroupChatQueue.Count != 0)
                {
                    ProcessQueue(GroupChatQueue, GroupChatIntervals, GroupChatMaxBurst);
                }

                if (GeneralRequestCount <= GeneralMaxBurst && GeneralQueue.Count != 0)
                {
                    GeneralQueue.First.Value.MRE.Set();
                    GeneralQueue.RemoveFirst();
                }
                else if (GeneralRequestCount > 0)
                {
                    GeneralRequestCount--;
                }
            }

            int wait = Math.Max(SafeGeneralInterval / 2, (int)(((TimerIntervals + 1) * SafeGeneralInterval) - msElapsed));
            Timer.Change(wait, Timeout.Infinite);
        }

        private void ProcessQueue(LinkedList<ScheduledRequestItem> queue, long intervals, int maxBurst)
        {
            LinkedListNode<ScheduledRequestItem> lastAddedNode = null;
            LinkedListNode<ScheduledRequestItem> lastAddedHpNode = GeneralQueue.Count == 0 ? null : GeneralQueue.First;
            var node = queue.First;
            do
            {
                var nextNode = node.Next;

                ChatRequestCount requestCount = RequestCounts[node.Value.ChatId];
                if (requestCount.GetRequestCount(intervals) <= maxBurst)
                {
                    requestCount.Increment();
                    if (node.Value.IsHighPriority)
                    {
                        lastAddedHpNode = InsertToGeneralQueueHp(node.Value, lastAddedHpNode);
                    }
                    else
                    {
                        lastAddedNode = InsertToGeneralQueue(node.Value, lastAddedNode);
                    }
                    queue.Remove(node);
                }
                node = nextNode;
            }
            while (node != null);
        }

        private LinkedListNode<ScheduledRequestItem> InsertToGeneralQueue(ScheduledRequestItem requestItem, LinkedListNode<ScheduledRequestItem> searchFrom)
        {
            var node = searchFrom;
            while (
                node != null &&
                (node.Value.IsHighPriority ||
                requestItem.TimerIntervalWhenAdded > node.Value.TimerIntervalWhenAdded)
            )
            {
                node = node.Next;
            }

            if (node == null)
                return GeneralQueue.AddLast(requestItem);

            return GeneralQueue.AddBefore(node, requestItem);
        }

        private LinkedListNode<ScheduledRequestItem> InsertToGeneralQueueHp(ScheduledRequestItem requestItem, LinkedListNode<ScheduledRequestItem> searchFrom)
        {
            var node = searchFrom;
            while (node?.Value.IsHighPriority == true)
                node = node.Next;

            if (node == null)
                return GeneralQueue.AddLast(requestItem);

            return GeneralQueue.AddBefore(node, requestItem);
        }

        #region Publicly available
        public TelegramRequestScheduler(SchedulerSettings schedulerSettings)
        {
            SafeGeneralInterval = schedulerSettings.SafeGeneralInterval;
            GeneralMaxBurst = schedulerSettings.GeneralMaxBurst;
            SafePrivateChatInterval = schedulerSettings.SafePrivateChatInterval;
            PrivateChatMaxBurst = schedulerSettings.PrivateChatMaxBurst;
            SafeGroupChatInterval = schedulerSettings.SafeGroupChatInterval;
            GroupChatMaxBurst = schedulerSettings.GroupChatMaxBurst;

            Stopwatch = Stopwatch.StartNew();
            Timer = new Timer(TimerCallback, null, SafeGeneralInterval, Timeout.Infinite);
        }

        public void WaitOne(SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod == SchedulingMethod.Ignore) return;

            ManualResetEvent mre;
            lock (QueueLock)
            {
                mre = WaitOneInternalUnlocked(schedulingMethod);
            }
            mre?.WaitOne();
        }

        public void WaitOne(ChatId chatId, SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod == SchedulingMethod.Ignore) return;

            if (chatId.Identifier != default) // Chat referenced by ID
            {
                WaitOneInternalLocked(chatId.Identifier, schedulingMethod);
            }
            else
            {
                // It is a channel and referenced by the @Username instead of the ID
                // Fallback to general limits
                // You should not send that much crap to a channel anyway
                WaitOne(schedulingMethod);
            }
        }
        #endregion

        // Returns a ManualResetEvent if the request was enqueued or null if it can be executed immediately
        private ManualResetEvent WaitOneInternalUnlocked(SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod == SchedulingMethod.NoScheduling ||
                GeneralRequestCount <= GeneralMaxBurst)
            {
                GeneralRequestCount++;
                return null;
            }

            bool isHighPriority = schedulingMethod == SchedulingMethod.HighPriority;
            var requestItem = new ScheduledRequestItem(TimerIntervals, isHighPriority);
            if (isHighPriority)
                GeneralQueue.AddFirst(requestItem);
            else
                GeneralQueue.AddLast(requestItem);

            return requestItem.MRE;
        }

        // Used for private chats, groups, supergroups and channels when they are referenced by ID
        private void WaitOneInternalLocked(long chatId, SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod == SchedulingMethod.NoScheduling)
            {
                ProcessRequestWithoutScheduling(chatId);
                return;
            }

            long chatIntervals;
            int chatMaxBurst;
            LinkedList<ScheduledRequestItem> chatQueue;
            if (chatId > 0)
            {
                chatIntervals = PrivateChatIntervals;
                chatMaxBurst = PrivateChatMaxBurst;
                chatQueue = PrivateChatQueue;
            }
            else
            {
                chatIntervals = GroupChatIntervals;
                chatMaxBurst = GroupChatMaxBurst;
                chatQueue = GroupChatQueue;
            }

            bool isHighPriority = schedulingMethod == SchedulingMethod.HighPriority;
            ManualResetEvent mre = ProcessRequest(chatId, schedulingMethod, isHighPriority, chatIntervals, chatMaxBurst, chatQueue);

            mre?.WaitOne();
        }

        private ManualResetEvent ProcessRequest(long chatId, SchedulingMethod schedulingMethod, bool isHighPriority, long chatIntervals, int chatMaxBurst, LinkedList<ScheduledRequestItem> chatQueue)
        {
            ManualResetEvent mre;

            lock (QueueLock)
            {
                if (RequestCounts.TryGetValue(chatId, out ChatRequestCount ChatRequestCount))
                {
                    if (ChatRequestCount.GetRequestCount(chatIntervals) <= chatMaxBurst)
                    {
                        ChatRequestCount.Increment();
                        mre = WaitOneInternalUnlocked(schedulingMethod);
                    }
                    else
                    {
                        var requestItem = new ScheduledRequestItem(TimerIntervals, isHighPriority, chatId);
                        if (isHighPriority)
                            chatQueue.AddFirst(requestItem);
                        else
                            chatQueue.AddLast(requestItem);
                        mre = requestItem.MRE;
                    }
                }
                else
                {
                    RequestCounts.Add(chatId, new ChatRequestCount(1, chatIntervals));
                    mre = WaitOneInternalUnlocked(schedulingMethod);
                }
            }

            return mre;
        }

        private void ProcessRequestWithoutScheduling(long chatId)
        {
            lock (QueueLock)
            {
                GeneralRequestCount++;
                if (RequestCounts.TryGetValue(chatId, out ChatRequestCount chatRequestCount))
                {
                    chatRequestCount.GetRequestCount(chatId > 0 ? PrivateChatIntervals : GroupChatIntervals);
                    chatRequestCount.Increment();
                }
                else
                {
                    RequestCounts.Add(chatId, new ChatRequestCount(1, chatId > 0 ? PrivateChatIntervals : GroupChatIntervals));
                }
            }
        }
    }
}