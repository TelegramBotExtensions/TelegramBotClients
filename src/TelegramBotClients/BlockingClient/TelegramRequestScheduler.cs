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
                    var nextNode = node.Next;
                    queue.Remove(node);
                    node = nextNode;
                }
                else
                {
                    node = node.Next;
                }
            }
            while (node != null);
        }

        private LinkedListNode<ScheduledRequestItem> InsertToGeneralQueue(ScheduledRequestItem requestItem, LinkedListNode<ScheduledRequestItem> searchFrom)
        {
            var node = searchFrom;
            while (node != null && (node.Value.IsHighPriority || requestItem.TimerIntervalWhenAdded > node.Value.TimerIntervalWhenAdded)) node = node.Next;

            if (node == null) return GeneralQueue.AddLast(requestItem);
            else return GeneralQueue.AddBefore(node, requestItem);
        }

        private LinkedListNode<ScheduledRequestItem> InsertToGeneralQueueHp(ScheduledRequestItem requestItem, LinkedListNode<ScheduledRequestItem> searchFrom)
        {
            var node = searchFrom;
            while (node?.Value.IsHighPriority == true) node = node.Next;

            if (node == null) return GeneralQueue.AddLast(requestItem);
            else return GeneralQueue.AddBefore(node, requestItem);
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
            ManualResetEvent mre;
            lock (QueueLock)
            {
                mre = WaitOneInternalUnlocked(schedulingMethod);
            }
            mre?.WaitOne();
        }

        public void WaitOne(ChatId chatId, SchedulingMethod schedulingMethod)
        {
            if (chatId.Identifier != default) // Chat referenced by ID
            {
                WaitOneInternalLocked(chatId.Identifier, schedulingMethod);
            }
            else
            {
                // It is a channel and referenced by the @Username instead of the ID
                // Fallback to general limits
                // You should not send that much crap to a channel anyway
                ManualResetEvent mre;
                lock (QueueLock)
                {
                    mre = WaitOneInternalUnlocked(schedulingMethod);
                }
                mre?.WaitOne();
            }
        }
        #endregion

        // Returns a ManualResetEvent if the request was enqueued or null if it can be executed immediately
        private ManualResetEvent WaitOneInternalUnlocked(SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod == SchedulingMethod.NoScheduling)
            {
                GeneralRequestCount++;
                return null;
            }
            bool isHighPriority = schedulingMethod == SchedulingMethod.HighPriority;

            ManualResetEvent mre;
            if (GeneralRequestCount <= GeneralMaxBurst)
            {
                GeneralRequestCount++;
                return null;
            }
            else
            {
                var requestItem = new ScheduledRequestItem(TimerIntervals, isHighPriority);
                if (isHighPriority)
                    GeneralQueue.AddFirst(requestItem);
                else
                    GeneralQueue.AddLast(requestItem);
                mre = requestItem.MRE;
            }
            return mre;
        }

        // Used for private chats, groups, supergroups and channels when they are referenced by ID
        private void WaitOneInternalLocked(long chatId, SchedulingMethod schedulingMethod)
        {
            if (schedulingMethod == SchedulingMethod.NoScheduling)
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
                return;
            }
            bool isHighPriority = schedulingMethod == SchedulingMethod.HighPriority;

            ManualResetEvent mre = null;

            if (chatId > 0)
            {
                lock (QueueLock)
                {
                    if (RequestCounts.TryGetValue(chatId, out ChatRequestCount ChatRequestCount))
                    {
                        if (ChatRequestCount.GetRequestCount(PrivateChatIntervals) <= PrivateChatMaxBurst)
                        {
                            ChatRequestCount.Increment();
                            mre = WaitOneInternalUnlocked(schedulingMethod);
                        }
                        else
                        {
                            var requestItem = new ScheduledRequestItem(TimerIntervals, isHighPriority, chatId);
                            if (isHighPriority)
                                PrivateChatQueue.AddFirst(requestItem);
                            else
                                PrivateChatQueue.AddLast(requestItem);
                            mre = requestItem.MRE;
                        }
                    }
                    else
                    {
                        RequestCounts.Add(chatId, new ChatRequestCount(1, PrivateChatIntervals));
                        mre = WaitOneInternalUnlocked(schedulingMethod);
                    }
                }
            }
            else
            {
                lock (QueueLock)
                {
                    if (RequestCounts.TryGetValue(chatId, out ChatRequestCount ChatRequestCount))
                    {
                        if (ChatRequestCount.GetRequestCount(GroupChatIntervals) <= GroupChatMaxBurst)
                        {
                            ChatRequestCount.Increment();
                            mre = WaitOneInternalUnlocked(schedulingMethod);
                        }
                        else
                        {
                            var requestItem = new ScheduledRequestItem(TimerIntervals, isHighPriority, chatId);
                            if (isHighPriority)
                                GroupChatQueue.AddFirst(requestItem);
                            else
                                GroupChatQueue.AddLast(requestItem);
                            mre = requestItem.MRE;
                        }
                    }
                    else
                    {
                        RequestCounts.Add(chatId, new ChatRequestCount(1, GroupChatIntervals));
                        mre = WaitOneInternalUnlocked(schedulingMethod);
                    }
                }
            }

            mre?.WaitOne();
            return;
        }
    }
}