using MihaZupan.TelegramBotClients.RateLimitedClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
#if DEBUG
using System.Threading;
#endif

namespace MihaZupan.TelegramBotClients.Tests.Unit
{
    public class RateLimitedTelegramRequestSchedulerTests
    {
        private const long TestPrivateChatId = 123;
        private const long TestGroupChatId = -123;

        private readonly SchedulerSettings _schedulerSettings;
        private readonly TelegramRequestScheduler _scheduler;

        public RateLimitedTelegramRequestSchedulerTests()
        {
            _schedulerSettings = SchedulerSettings.Default;
            _scheduler = new TelegramRequestScheduler(_schedulerSettings);
        }

        [Fact]
        public void HasReasonableDefaultSettings()
        {
            SchedulerSettings schedulerSettings = SchedulerSettings.Default;

            Assert.True(schedulerSettings.SafeGeneralInterval <= schedulerSettings.SafePrivateChatInterval);
            Assert.True(schedulerSettings.SafeGeneralInterval <= schedulerSettings.SafeGroupChatInterval);
            Assert.True(schedulerSettings.SafePrivateChatInterval <= schedulerSettings.SafeGroupChatInterval);
        }

        [Fact]
        public void MaintainsAverageTiming()
        {
            Stopwatch s = Stopwatch.StartNew();

            int testCount = 100;

            Task[] tasks = new Task[testCount];

            for (int i = 0; i < testCount; i++)
            {
                int ii = i;
                tasks[i] = Task.Run(async () => {
                    await _scheduler.YieldAsync(ii);
                });
            }

            Task.WaitAll(tasks);

            s.Stop();

            Assert.InRange(
                s.ElapsedMilliseconds,
                (testCount - 5) * _schedulerSettings.SafeGeneralInterval,
                (testCount + 5) * _schedulerSettings.SafeGeneralInterval);

#if DEBUG
            s.Restart();
            while (_scheduler.DEBUG_CURRENT_COUNT != 0) { Thread.Sleep(1); };
            s.Stop();

            Assert.InRange(s.ElapsedMilliseconds,
                _schedulerSettings.SafePrivateChatInterval / 2,
                _schedulerSettings.SafePrivateChatInterval * 2);
#endif
        }

        [Fact]
        public void RespectsPrivateChatTimings()
        {
            Stopwatch s = Stopwatch.StartNew();

            int testCount = 10;

            Task[] tasks = new Task[testCount];

            for (int i = 0; i < testCount; i++)
            {
                tasks[i] = Task.Run(async () => {
                    await _scheduler.YieldAsync(TestPrivateChatId);
                });
            }

            Task.WaitAll(tasks);

            s.Stop();

            Assert.InRange(
                s.ElapsedMilliseconds,
                (testCount - 1) * _schedulerSettings.SafePrivateChatInterval,
                (testCount + 1) * _schedulerSettings.SafePrivateChatInterval);

#if DEBUG
            s.Restart();
            while (_scheduler.DEBUG_CURRENT_COUNT != 0) { Thread.Sleep(1); };
            s.Stop();

            Assert.InRange(s.ElapsedMilliseconds,
                _schedulerSettings.SafePrivateChatInterval / 2,
                _schedulerSettings.SafePrivateChatInterval * 2);
#endif
        }

        [Fact]
        public void RespectsPrivateChatTimings2()
        {
            Stopwatch s = Stopwatch.StartNew();

            int testCount = 10;

            Task[] tasks = new Task[testCount];

            for (int i = 0; i < testCount; i++)
            {
                int ii = i;
                tasks[i] = Task.Run(async () => {
                    await _scheduler.YieldAsync(TestPrivateChatId + ii % 2);
                });
            }

            Task.WaitAll(tasks);

            s.Stop();

            Assert.InRange(
                s.ElapsedMilliseconds,
                (testCount / 2 - 1) * _schedulerSettings.SafePrivateChatInterval,
                (testCount / 2 + 1) * _schedulerSettings.SafePrivateChatInterval);

#if DEBUG
            s.Restart();
            while (_scheduler.DEBUG_CURRENT_COUNT != 0) { Thread.Sleep(1); };
            s.Stop();

            Assert.InRange(s.ElapsedMilliseconds,
                _schedulerSettings.SafePrivateChatInterval / 2,
                _schedulerSettings.SafePrivateChatInterval * 2);
#endif
        }

        [Fact]
        public void RespectsGroupChatTimings()
        {
            Stopwatch s = Stopwatch.StartNew();

            int testCount = 6;

            Task[] tasks = new Task[testCount];

            for (int i = 0; i < testCount; i++)
            {
                tasks[i] = Task.Run(async () => {
                    await _scheduler.YieldAsync(TestGroupChatId);
                });
            }

            Task.WaitAll(tasks);

            s.Stop();

            Assert.InRange(
                s.ElapsedMilliseconds,
                (testCount - 1) * _schedulerSettings.SafeGroupChatInterval,
                (testCount + 1) * _schedulerSettings.SafeGroupChatInterval);

#if DEBUG
            s.Restart();
            while (_scheduler.DEBUG_CURRENT_COUNT != 0) { Thread.Sleep(1); };
            s.Stop();

            Assert.InRange(s.ElapsedMilliseconds,
                _schedulerSettings.SafeGroupChatInterval / 2,
                _schedulerSettings.SafeGroupChatInterval * 2);
#endif
        }

        [Fact]
        public void RespectsGroupChatTimings2()
        {
            Stopwatch s = Stopwatch.StartNew();

            int testCount = 6;

            Task[] tasks = new Task[testCount];

            for (int i = 0; i < testCount; i++)
            {
                int ii = i;
                tasks[i] = Task.Run(async () => {
                    await _scheduler.YieldAsync(TestGroupChatId + ii % 2);
                });
            }

            Task.WaitAll(tasks);

            s.Stop();

            Assert.InRange(
                s.ElapsedMilliseconds,
                (testCount / 2 - 1) * _schedulerSettings.SafeGroupChatInterval,
                (testCount / 2 + 1) * _schedulerSettings.SafeGroupChatInterval);

#if DEBUG
            s.Restart();
            while (_scheduler.DEBUG_CURRENT_COUNT != 0) { Thread.Sleep(1); };
            s.Stop();

            Assert.InRange(s.ElapsedMilliseconds,
                _schedulerSettings.SafeGroupChatInterval / 2,
                _schedulerSettings.SafeGroupChatInterval * 2);
#endif
        }

        [Fact]
        public void AcceptsStringBuckets()
        {
            Stopwatch s = Stopwatch.StartNew();

            int testCount = 3;

            Task[] tasks = new Task[testCount];

            for (int i = 0; i < testCount; i++)
            {
                int ii = i;
                tasks[i] = Task.Run(async () => {
                    await _scheduler.YieldAsync("@tgbots_dotnet");
                });
            }

            Task.WaitAll(tasks);

            s.Stop();

            Assert.InRange(
                s.ElapsedMilliseconds,
                (testCount - 1) * _schedulerSettings.SafeGroupChatInterval,
                (testCount + 1) * _schedulerSettings.SafeGroupChatInterval);

#if DEBUG
            s.Restart();
            while (_scheduler.DEBUG_CURRENT_COUNT != 0) { Thread.Sleep(1); };
            s.Stop();

            Assert.InRange(s.ElapsedMilliseconds,
                _schedulerSettings.SafeGroupChatInterval / 2,
                _schedulerSettings.SafeGroupChatInterval * 2);
#endif
        }
    }
}
