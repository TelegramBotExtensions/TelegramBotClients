using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Telegram.Bot.Types;

namespace MihaZupan.TelegramBotClients
{
    class TelegramRequestScheduler
    {
        private const int SafeGeneralInterval = 35;
        private const int GeneralMaxBurst = 10;

        private const int SafePrivateChatInterval = 1000;
        private const int PrivateChatMaxBurst = 5;

        private const int SafeGroupChatInterval = 3000;
        private const int GroupChatMaxBurst = 5;

        private long TimerIntervals = 0;
        private long PrivateChatIntervals = 0;
        private long GroupChatIntervals = 0;

        // Amount of all sent requests that have not yet been written off
        private int GeneralRequestCount = 0;

        private object QueueLock = new object();

        private Timer Timer;
        private Stopwatch Stopwatch;

        private LinkedList<ScheduledRequestItem> PrivateChatQueue = new LinkedList<ScheduledRequestItem>();
        private LinkedList<ScheduledRequestItem> GroupChatQueue = new LinkedList<ScheduledRequestItem>();
        private LinkedList<ScheduledRequestItem> GeneralQueue = new LinkedList<ScheduledRequestItem>();

        private Dictionary<long, ChatRequestCount> RequestCounts = new Dictionary<long, ChatRequestCount>(50);

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
                    LinkedListNode<ScheduledRequestItem> lastAddedNode = null;
                    var node = PrivateChatQueue.First;
                    do
                    {
                        ChatRequestCount requestCount = RequestCounts[node.Value.ChatId];
                        if (requestCount.GetRequestCount(PrivateChatIntervals) <= PrivateChatMaxBurst)
                        {
                            requestCount.Increment();
                            lastAddedNode = InsertToGeneralQueue(node.Value, lastAddedNode);
                        }
                        node = node.Next;
                    }
                    while (node != null);
                }

                if (CanUpdateGroupChats && GroupChatQueue.Count != 0)
                {
                    LinkedListNode<ScheduledRequestItem> lastAddedNode = null;
                    var node = GroupChatQueue.First;
                    do
                    {
                        ChatRequestCount requestCount = RequestCounts[node.Value.ChatId];
                        if (requestCount.GetRequestCount(GroupChatIntervals) <= GroupChatMaxBurst)
                        {
                            requestCount.Increment();
                            lastAddedNode = InsertToGeneralQueue(node.Value, lastAddedNode);
                        }
                        node = node.Next;
                    }
                    while (node != null);
                }

                if (GeneralRequestCount <= GeneralMaxBurst && GeneralQueue.Count != 0)
                {
                    GeneralQueue.First.Value.MRE.Set();
                    GeneralQueue.RemoveFirst();
                }
                else if (GeneralRequestCount > 0) GeneralRequestCount--;
            }

            int wait = Math.Max(SafeGeneralInterval / 2, (int)((TimerIntervals + 1) * SafeGeneralInterval - msElapsed));
            Timer.Change(wait, Timeout.Infinite);
        }
        private LinkedListNode<ScheduledRequestItem> InsertToGeneralQueue(ScheduledRequestItem requestItem, LinkedListNode<ScheduledRequestItem> searchFrom)
        {
            var node = searchFrom;
            while (node != null && requestItem.TimerIntervalWhenAdded > node.Value.TimerIntervalWhenAdded) node = node.Next;

            if (node == null) return GeneralQueue.AddLast(requestItem);
            else return GeneralQueue.AddBefore(node, requestItem);
        }

        #region Publicly available
        public TelegramRequestScheduler()
        {
            Stopwatch = Stopwatch.StartNew();
            Timer = new Timer(TimerCallback, null, SafeGeneralInterval, Timeout.Infinite);
        }

        public void WaitOne()
        {
            ManualResetEvent mre;
            lock (QueueLock)
            {
                mre = WaitOneInternalUnlocked();
            }
            mre?.WaitOne();
        }
        public void WaitOne(ChatId chatId)
        {
            if (chatId.Identifier != default) // Chat referenced by ID
            {
                WaitOneInternalLocked(chatId.Identifier);
            }
            else
            {
                // It is a channel and referenced by the @Username instead of the ID
                // Fallback to general limits
                // You should not send that much crap to a channel anyway
                ManualResetEvent mre;
                lock (QueueLock)
                {
                    mre = WaitOneInternalUnlocked();
                }
                mre?.WaitOne();
            }            
        }
        #endregion

        // Returns a ManualResetEvent if the request was enqueued or null if it can be executed immediately
        private ManualResetEvent WaitOneInternalUnlocked()
        {
            ManualResetEvent mre;
            if (GeneralRequestCount <= GeneralMaxBurst)
            {
                GeneralRequestCount++;
                return null;
            }
            else
            {
                var requestItem = new ScheduledRequestItem(TimerIntervals);
                GeneralQueue.AddFirst(requestItem);
                mre = requestItem.MRE;
            }
            return mre;
        }

        // Used for private chats, groups, supergroups and channels when they are referenced by ID
        private void WaitOneInternalLocked(long chatId)
        {
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
                            mre = WaitOneInternalUnlocked();
                        }
                        else
                        {
                            var requestItem = new ScheduledRequestItem(TimerIntervals, chatId);
                            PrivateChatQueue.AddLast(requestItem);
                            mre = requestItem.MRE;
                        }
                    }
                    else
                    {
                        RequestCounts.Add(chatId, new ChatRequestCount(1, PrivateChatIntervals));
                        mre = WaitOneInternalUnlocked();
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
                            mre = WaitOneInternalUnlocked();
                        }
                        else
                        {
                            var requestItem = new ScheduledRequestItem(TimerIntervals, chatId);
                            GroupChatQueue.AddLast(requestItem);
                            mre = requestItem.MRE;
                        }
                    }
                    else
                    {
                        RequestCounts.Add(chatId, new ChatRequestCount(1, GroupChatIntervals));
                        mre = WaitOneInternalUnlocked();
                    }
                }
            }

            mre?.WaitOne();
            return;
        }        
    }
}