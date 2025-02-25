//using TommAPI.BackgroundService;
//using Microsoft.Extensions.DependencyInjection;
//using NCrontab;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using TommBLL.Interface;

//namespace TommAPI.Scheduler
//{
//    public abstract class ScheduledProcessor : ScopedProcessor
//    {
//        private CrontabSchedule _schedule;
//        private DateTime _nextRun;
//        protected abstract string Schedule { get; }
//        public IConfiguration _configuration;
       
//        public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration) : base(serviceScopeFactory, configuration)
//        {
//            _schedule = CrontabSchedule.Parse(configuration["PushNotification:STime"]);
//            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
//            _configuration = configuration;  
//        }
//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            do
//            {
//                var now = DateTime.Now;
//                var nextrun = _schedule.GetNextOccurrence(now);
//                if (now > _nextRun)
//                {
//                    await Process();
//                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
//                }
//                await Task.Delay(5000, stoppingToken); //5 seconds delay
//            }
//            while (!stoppingToken.IsCancellationRequested);
//        }
//    }
//}
