using System;
using System.Threading.Tasks;
using MihaZupan.TelegramBotClients.BlockingClient;
using Xunit;

namespace BlockingClientTests
{
    public class TelegramRequestSchedulerTests
    {
        private readonly TelegramRequestScheduler _scheduler;
        private readonly Random _rnd = new Random();
        private SchedulingMethod RandomPriority => (SchedulingMethod)_rnd.Next(0, 4);

        public TelegramRequestSchedulerTests()
        {
            SchedulerSettings settings = new SchedulerSettings();
            //(
            //    safeGeneralInterval: 1000,
            //    generalMaxBurts: 5,
            //    safePrivateChatInterval: 1000,
            //    privateChatMaxBurst: 5,
            //    safeGroupChatInterval: 1000,
            //    groupChatMaxBurst: 5
            //);

            _scheduler = new TelegramRequestScheduler(settings);
        }

        [Fact]
        public void Test_Private()
        {
            const long chatId = 100;

            for (int i = 0; i < 100; i++)
            {
                _scheduler.WaitOne(chatId, RandomPriority);
                Task.Delay(1000 * _rnd.Next(5));
            }
        }

        [Fact]
        public void Test_Group()
        {
            const long chatId = -100;

            for (int i = 0; i < 100; i++)
            {
                _scheduler.WaitOne(chatId, RandomPriority);
                Task.Delay(1000 * _rnd.Next(5));
            }
        }

        [Fact]
        public void Test_General()
        {
            for (int i = 0; i < 100; i++)
            {
                _scheduler.WaitOne(RandomPriority);
                Task.Delay(1000 * _rnd.Next(5));
            }
        }

        [Fact]
        public void Test_Parallel()
        {
            Parallel.For(0, 100, _ =>
            {
                _scheduler.WaitOne(RandomPriority);
                Task.Delay(1000 * _rnd.Next(5));
            });
        }
    }
}
