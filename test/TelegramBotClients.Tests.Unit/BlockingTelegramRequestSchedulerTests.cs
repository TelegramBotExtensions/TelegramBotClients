using System.Diagnostics;
using System.Threading.Tasks;
using MihaZupan.TelegramBotClients.BlockingClient;
using Xunit;

namespace MihaZupan.TelegramBotClients.Tests.Unit
{
    public class BlockingTelegramRequestSchedulerTests
    {
        private const long TestPrivateChatId = 123;
        private const long TestGroupChatId = -123;
        
        private readonly SchedulerSettings _schedulerSettings;
        private readonly TelegramRequestScheduler _scheduler;

        public BlockingTelegramRequestSchedulerTests()
        {
            _schedulerSettings = SchedulerSettings.Default;
            _scheduler = new TelegramRequestScheduler(_schedulerSettings);
        }

        [Fact]
        public void DefaultSchedulerSettings_AreReasonable()
        {
            SchedulerSettings schedulerSettings = SchedulerSettings.Default;

            Assert.True(schedulerSettings.SafeGeneralInterval <= schedulerSettings.SafePrivateChatInterval);
            Assert.True(schedulerSettings.SafeGeneralInterval <= schedulerSettings.SafeGroupChatInterval);
            Assert.True(schedulerSettings.GeneralMaxBurst >= schedulerSettings.PrivateChatMaxBurst);
            Assert.True(schedulerSettings.GeneralMaxBurst >= schedulerSettings.GroupChatMaxBurst);
        }

        #region Average Timing

        [Fact]
        public void Average_Timing_General_Consecutive()
        {
            // We only want the average timing after the limit has been hit
            for (int i = 0; i <= _schedulerSettings.GeneralMaxBurst; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            }

            int iterations = 250;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            }

            Assert.InRange(
                stopwatch.ElapsedMilliseconds,
                _schedulerSettings.SafeGeneralInterval * (iterations * 0.9f),
                _schedulerSettings.SafeGeneralInterval * (iterations * 1.1f));
        }

        [Fact]
        public void Average_Timing_General_Parallel()
        {
            // We only want the average timing after the limit has been hit
            for (int i = 0; i <= _schedulerSettings.GeneralMaxBurst; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            }

            int iterations = 250;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Parallel.For(0, iterations, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            });
            
            Assert.InRange(
                stopwatch.ElapsedMilliseconds,
                _schedulerSettings.SafeGeneralInterval * (iterations * 0.9f),
                _schedulerSettings.SafeGeneralInterval * (iterations * 1.1f));
        }

        [Fact]
        public void Average_Timing_Private_Consecutive()
        {
            // We only want the average timing after the limit has been hit
            for (int i = 0; i <= _schedulerSettings.PrivateChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            }

            int iterations = 10;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            }

            Assert.InRange(
                stopwatch.ElapsedMilliseconds,
                _schedulerSettings.SafePrivateChatInterval * (iterations * 0.9f),
                _schedulerSettings.SafePrivateChatInterval * (iterations * 1.1f));
        }

        [Fact]
        public void Average_Timing_Private_Parallel()
        {
            // We only want the average timing after the limit has been hit
            for (int i = 0; i <= _schedulerSettings.PrivateChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            }

            int iterations = 10;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Parallel.For(0, iterations, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            });

            Assert.InRange(
                stopwatch.ElapsedMilliseconds,
                _schedulerSettings.SafePrivateChatInterval * (iterations * 0.9f),
                _schedulerSettings.SafePrivateChatInterval * (iterations * 1.1f));
        }

        [Fact]
        public void Average_Timing_Group_Consecutive()
        {
            // We only want the average timing after the limit has been hit
            for (int i = 0; i <= _schedulerSettings.GroupChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            }

            int iterations = 10;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            }

            Assert.InRange(
                stopwatch.ElapsedMilliseconds,
                _schedulerSettings.SafeGroupChatInterval * (iterations * 0.9f),
                _schedulerSettings.SafeGroupChatInterval * (iterations * 1.1f));
        }

        [Fact]
        public void Average_Timing_Group_Parallel()
        {
            // We only want the average timing after the limit has been hit
            for (int i = 0; i <= _schedulerSettings.GroupChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            }

            int iterations = 10;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Parallel.For(0, iterations, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            });

            Assert.InRange(
                stopwatch.ElapsedMilliseconds,
                _schedulerSettings.SafeGroupChatInterval * (iterations * 0.9f),
                _schedulerSettings.SafeGroupChatInterval * (iterations * 1.1f));
        }

        #endregion Average Timing

        #region Burst no block

        [Fact]
        public void Bursts_No_Block_General()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < _schedulerSettings.GeneralMaxBurst - 1; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            }
            Assert.True(stopwatch.ElapsedMilliseconds < 50);
        }

        [Fact]
        public void Bursts_No_Block_Private()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < _schedulerSettings.PrivateChatMaxBurst - 1; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            }
            Assert.True(stopwatch.ElapsedMilliseconds < 50);
        }

        [Fact]
        public void Bursts_No_Block_Group()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < _schedulerSettings.GroupChatMaxBurst - 1; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            }
            Assert.True(stopwatch.ElapsedMilliseconds < 50);
        }

        #endregion Burst no block

        #region General burst blocks private and group

        [Fact]
        public void General_Burst_Blocks_Private()
        {
            for (int i = 0; i <= _schedulerSettings.GeneralMaxBurst * 2; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.NoScheduling);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);

            Assert.True(stopwatch.ElapsedMilliseconds > 50);
        }

        [Fact]
        public void General_Burst_Blocks_Group()
        {
            for (int i = 0; i <= _schedulerSettings.GeneralMaxBurst * 2; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.NoScheduling);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);

            Assert.True(stopwatch.ElapsedMilliseconds > 50);
        }

        #endregion General burst blocks private and group

        #region HighPriority takes presedence

        [Fact]
        public void HighPriority_Takes_Precedence_General()
        {
            for (int i = 0; i < _schedulerSettings.GeneralMaxBurst; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            }

            // Fill the queue with pending requests
            for (int i = 0; i < 10; i++)
            {
                Task.Run(()=>
                {
                    _scheduler.WaitOne(SchedulingMethod.Normal);
                });
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(SchedulingMethod.HighPriority);
            Assert.True(stopwatch.ElapsedMilliseconds < _schedulerSettings.SafeGeneralInterval * 2);
        }

        [Fact]
        public void HighPriority_Takes_Precedence_Private()
        {
            for (int i = 0; i < _schedulerSettings.PrivateChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            }

            // Fill the queue with pending requests
            for (int i = 0; i < 10; i++)
            {
                Task.Run(() =>
                {
                    _scheduler.WaitOne(SchedulingMethod.Normal);
                });
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(SchedulingMethod.HighPriority);
            Assert.True(stopwatch.ElapsedMilliseconds < _schedulerSettings.SafePrivateChatInterval * 2);
        }

        [Fact]
        public void HighPriority_Takes_Precedence_Group()
        {
            for (int i = 0; i < _schedulerSettings.GroupChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            }

            // Fill the queue with pending requests
            for (int i = 0; i < 10; i++)
            {
                Task.Run(() =>
                {
                    _scheduler.WaitOne(SchedulingMethod.Normal);
                });
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(SchedulingMethod.HighPriority);
            Assert.True(stopwatch.ElapsedMilliseconds < _schedulerSettings.SafeGroupChatInterval * 2);
        }

        #endregion HighPriority takes presedence

        #region NoScheduling

        [Fact]
        public void NoScheduling_General()
        {
            for (int i = 0; i < _schedulerSettings.GeneralMaxBurst * 2; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.NoScheduling);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(SchedulingMethod.Normal);

            Assert.True(stopwatch.ElapsedMilliseconds > _schedulerSettings.SafeGeneralInterval * 2);
        }

        [Fact]
        public void NoScheduling_Private()
        {
            for (int i = 0; i < _schedulerSettings.PrivateChatMaxBurst * 2; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.NoScheduling);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);

            Assert.True(stopwatch.ElapsedMilliseconds > _schedulerSettings.SafePrivateChatInterval * 2);
        }

        [Fact]
        public void NoScheduling_Group()
        {
            for (int i = 0; i < _schedulerSettings.GroupChatMaxBurst * 2; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.NoScheduling);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);

            Assert.True(stopwatch.ElapsedMilliseconds > _schedulerSettings.SafeGroupChatInterval * 2);
        }

        #endregion NoScheduling

        #region Ignore

        [Fact]
        public void Ignore_General()
        {
            for (int i = 0; i < _schedulerSettings.GeneralMaxBurst; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Ignore);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < _schedulerSettings.GeneralMaxBurst; i++)
            {
                _scheduler.WaitOne(SchedulingMethod.Normal);
            }

            Assert.True(stopwatch.ElapsedMilliseconds < 50);
        }

        [Fact]
        public void Ignore_Private()
        {
            for (int i = 0; i < _schedulerSettings.PrivateChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Ignore);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < _schedulerSettings.PrivateChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestPrivateChatId, SchedulingMethod.Normal);
            }

            Assert.True(stopwatch.ElapsedMilliseconds < 50);
        }

        [Fact]
        public void Ignore_Group()
        {
            for (int i = 0; i < _schedulerSettings.GroupChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Ignore);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < _schedulerSettings.GroupChatMaxBurst; i++)
            {
                _scheduler.WaitOne(TestGroupChatId, SchedulingMethod.Normal);
            }

            Assert.True(stopwatch.ElapsedMilliseconds < 50);
        }

        #endregion Ignore        
    }
}
