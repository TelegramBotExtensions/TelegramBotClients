// Copyright (c) Miha Zupan. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE file in the project root for more information.

#nullable enable

using SharpCollections.Generic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MihaZupan.TelegramBotClients.RateLimitedClient
{
    public sealed class TelegramRequestScheduler
    {
        private readonly SchedulerSettings _settings;
        private readonly RequestGroupScheduler _scheduler;

#if DEBUG
        /// <summary>
        /// For testing purposes only
        /// </summary>
        public int DEBUG_CURRENT_COUNT => _scheduler.DEBUG_CURRENT_COUNT;
#endif

        public TelegramRequestScheduler(SchedulerSettings? schedulerSettings = null)
        {
            _settings = schedulerSettings ?? SchedulerSettings.Default;
            _scheduler = new RequestGroupScheduler(TimeSpan.FromMilliseconds(_settings.SafeGeneralInterval), resetThreshold: 5);
        }

        public Task YieldAsync(ChatId chatId, CancellationToken cancellationToken)
        {
            if (chatId?.Username is string username)
            {
                return YieldAsyncCore(username.GetHashCode(), _settings.SafeGroupChatInterval, cancellationToken);
            }
            else
            {
                return YieldAsync(chatId?.Identifier ?? 0, cancellationToken);
            }
        }

        public Task YieldAsync(long bucketId, CancellationToken cancellationToken)
        {
            long timestampIncrement = bucketId == 0 ? _settings.SafeGeneralInterval
                : bucketId > 0 ? _settings.SafePrivateChatInterval
                : _settings.SafeGroupChatInterval;

            return YieldAsyncCore(bucketId, timestampIncrement, cancellationToken);
        }

        public Task YieldAsync(CancellationToken cancellationToken)
        {
            return YieldAsyncCore(bucketId: 0, _settings.SafeGeneralInterval, cancellationToken);
        }

        private Task YieldAsyncCore(long bucketId, long timestampIncrement, CancellationToken cancellationToken)
        {
            return _scheduler.YieldAsync(bucketId, timestampIncrement * 10_000, cancellationToken);
        }

        internal void Stop()
        {
            _scheduler.Stop();
        }
    }

    internal sealed class RequestGroupScheduler
    {
        private readonly struct RequestNode : IComparable<RequestNode>
        {
            public readonly long BucketId;
            public readonly long Timestamp;
            public readonly long TimestampIncrement;

            public RequestNode(long bucketId, long timestamp, long timestampIncrement)
            {
                BucketId = bucketId;
                Timestamp = timestamp;
                TimestampIncrement = timestampIncrement;
            }

            public RequestNode Next(long currentTimestamp)
            {
                return new RequestNode(BucketId, currentTimestamp + TimestampIncrement, TimestampIncrement);
            }

            public int CompareTo(RequestNode other)
            {
                return Timestamp.CompareTo(other.Timestamp);
            }
        }

        private sealed class Bucket : Queue<Request>
        {
            private Request? _singleRequest;

            public new void Enqueue(Request request)
            {
                if (_singleRequest is null && Count == 0)
                {
                    _singleRequest = request;
                }
                else
                {
                    base.Enqueue(request);
                }
            }

            public bool TryCompleteRequest()
            {
                if (_singleRequest is Request request)
                {
                    _singleRequest = null;

                    if (request.TrySetCompleted())
                    {
                        return true;
                    }
                }

                while (Count != 0)
                {
                    if (Dequeue().TrySetCompleted())
                    {
                        return true;
                    }
                }

                return false;
            }

            public void CancelAllRequests()
            {
                if (_singleRequest is Request request)
                {
                    _singleRequest = null;
                    request.ForceCancel();
                }

                while (Count != 0)
                {
                    Dequeue().ForceCancel();
                }
            }
        }

        private sealed class Request : TaskCompletionSource<object?>
        {
            private CancellationToken _cancellationToken;
            private CancellationTokenRegistration _cancellationRegistration;

            public Request(CancellationToken cancellationToken) : base(TaskCreationOptions.RunContinuationsAsynchronously)
            {
                _cancellationToken = cancellationToken;
                // UnsafeRegister in .NET 5.0+
                _cancellationRegistration = _cancellationToken.Register(s =>
                {
                    Request request = (Request)s;
                    request.TrySetCanceled(request._cancellationToken);
                }, this);
            }

            public bool TrySetCompleted()
            {
                if (TrySetResult(null))
                {
                    _cancellationRegistration.Dispose();
                    return true;
                }
                return false;
            }

            public void ForceCancel()
            {
                if (TrySetCanceled())
                {
                    _cancellationRegistration.Dispose();
                }
            }
        }

        private readonly object _lock;
        private readonly BinaryHeap<RequestNode> _requestHeap;
        private readonly Dictionary<long, Bucket?> _buckets;

        private long _lastTimestamp;
        private readonly Timer _timer;

        private readonly int _timerIntervalMs;
        private readonly long _resetThresholdTicks;

        private bool _stopped = false;

#if DEBUG
        /// <summary>
        /// For testing purposes only
        /// </summary>
        public int DEBUG_CURRENT_COUNT => _requestHeap.Count + _buckets.Count;
#endif

        private long Timestamp() => DateTime.UtcNow.Ticks;
        private void IncrementLastTimestamp() => _lastTimestamp += _timerIntervalMs * 10_000L;

        private void OnInterval()
        {
            // Limit the amount of events processed in a single timer callback
            // to avoid taking the lock for too long in case of mass request cancellation
            const int EventLimit = 1_000; // This still means ~30k requests/s at default settings

            lock (_lock)
            {
                if (_stopped)
                    return;

                long timestamp = Timestamp();

                if (timestamp - _lastTimestamp > _resetThresholdTicks)
                    _lastTimestamp = timestamp - _resetThresholdTicks;

                for (int i = 0; i < EventLimit && _lastTimestamp < timestamp && !_requestHeap.IsEmpty && _requestHeap.Top.Timestamp <= timestamp; i++)
                {
                    RequestNode request = _requestHeap.Pop();
                    Bucket? bucket = _buckets[request.BucketId];

                    if (bucket is null || !bucket.TryCompleteRequest())
                    {
                        _buckets.Remove(request.BucketId);
                    }
                    else
                    {
                        _requestHeap.Push(request.Next(timestamp));
                        IncrementLastTimestamp();
                    }
                }
            }

            _timer.Change(_timerIntervalMs, Timeout.Infinite);
        }

        public RequestGroupScheduler(TimeSpan interval, int resetThreshold)
        {
            _stopped = false;
            _timerIntervalMs = (int)interval.TotalMilliseconds;
            _resetThresholdTicks = _timerIntervalMs * 10_000L * resetThreshold;

            _lock = new object();
            _requestHeap = new BinaryHeap<RequestNode>(16);
            _buckets = new Dictionary<long, Bucket?>(16);

            _lastTimestamp = 0;
            _timer = new Timer(s => ((RequestGroupScheduler)s).OnInterval(), this, 0, Timeout.Infinite);
        }

        public Task YieldAsync(long bucketId, long timestampIncrement, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            lock (_lock)
            {
                if (_stopped)
                    return Task.FromException(new ObjectDisposedException(nameof(RequestGroupScheduler)));

                if (!_buckets.TryGetValue(bucketId, out Bucket? bucket))
                {
                    long timestamp = Timestamp();

                    // Fast-path if we have room for requests available right away
                    bool returnSynchronously = _lastTimestamp < timestamp;

                    if (returnSynchronously)
                    {
                        timestamp += timestampIncrement;
                        _buckets[bucketId] = null;
                        IncrementLastTimestamp();
                    }

                    _requestHeap.Push(new RequestNode(bucketId, timestamp, timestampIncrement));

                    if (returnSynchronously)
                        return Task.CompletedTask;
                }

                if (bucket is null)
                {
                    _buckets[bucketId] = bucket = new Bucket();
                }

                var request = new Request(cancellationToken);
                bucket.Enqueue(request);
                return request.Task;
            }
        }

        public void Stop()
        {
            try
            {
                lock (_lock)
                {
                    if (_stopped)
                        return;

                    _stopped = true;

                    _timer.Dispose();

                    foreach (Bucket? bucket in _buckets.Values)
                    {
                        bucket?.CancelAllRequests();
                    }

                    _buckets.Clear();
                }
            }
            catch { }
        }
    }
}
