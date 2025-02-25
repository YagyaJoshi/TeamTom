//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net.Mail;
//using System.Net;
//using System.IO;
//using Microsoft.Extensions.Configuration;
//using TommBLL.Interface;
//using TommDAL.ViewModel;
//using Newtonsoft.Json;
//using TommAPI.Providers;
//using TommBLL.Repository;
//using MySql.Data.MySqlClient;
//using System.Data;

//namespace TommAPI.Scheduler
//{
//    public class ScheduleTask : ScheduledProcessor
//    {

//        public IConfiguration _configuration;

//        public ScheduleTask(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration) : base(serviceScopeFactory, configuration)
//        {
//            _configuration = configuration;

//        }

//        //protected override string Schedule => "*/1 * * * *";
//        //protected override string Schedule => "0 7 * * *"; // 7 am 
//        protected override string Schedule => "0 12 * * *"; // 7 PM 
//        public override Task ProcessInScope(IServiceProvider serviceProvider)
//        {
//            try
//            {
//                Console.WriteLine("Processing starts here");
//                //GetUsers();
//                PushNotification.SavePushNotification("New guided session just dropped, check it out!", _configuration);
//                GetUsersV2();
//                Console.WriteLine("Processing");
//            }
//            catch (Exception ex)
//            {
//                string ExceptionString = "Job - " + Environment.NewLine + ex.Message;
//                Console.WriteLine(ExceptionString);
//                //var fileName = "Job - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
//                // _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
//            }
//            return Task.CompletedTask;
//        }

//        public async Task SendNotificationInParallelInWithBatches(List<UserTaskHisV3> userIds)
//        {
//            var usrlist = await Task.Run(() => Parallel.ForEach(userIds, item =>
//                                    {
//                                        PushNotification.Send("", item.DayTitle, item.DeviceToken, "Job status", _configuration, item.UserId);
//                                    }));

//            Console.WriteLine("Processing ends here");
//        }

//        public string GetUsers()
//        {
//            try
//            {

//                Int32 WeekNumber = PushNotification.GetWeek();
//                Int32 IsDay = (int)System.DateTime.Now.DayOfWeek;
//                List<UserTaskHisV3> objUserTaskHisV3 = PushNotification.GetUserDetailsforNotification(WeekNumber, IsDay, _configuration);
//                var testA = SendNotificationInParallelInWithBatches(objUserTaskHisV3);

//            }
//            catch (System.Net.Mail.SmtpException ex)
//            {
//                ServicesRepo objrepo = new ServicesRepo(_configuration);
//                objrepo.SendMail(_configuration["Log:ErroAddress"], "GetUsers -Notification", ex.StackTrace + ex.Message);

//            }
//            catch (Exception exs)
//            {
//                ServicesRepo objrepo = new ServicesRepo(_configuration);
//                objrepo.SendMail(_configuration["Log:ErroAddress"], "GetUsers -Notification", exs.StackTrace + exs.Message);
//            }
//            return "";
//        }

//        public string GetUsersV2()
//        {
//            try
//            {

//                Int32 WeekNumber = PushNotification.GetWeek();
//                Int32 IsDay = (int)System.DateTime.Now.DayOfWeek;
//                List<UserTaskHisV3> objUserTaskHisV3 = PushNotification.GetUserDetailsforNotificationV2(_configuration);
//                var testA = SendNotificationInParallelInWithBatches(objUserTaskHisV3);

//            }
//            catch (System.Net.Mail.SmtpException ex)
//            {
//                ServicesRepo objrepo = new ServicesRepo(_configuration);
//                objrepo.SendMail(_configuration["Log:ErroAddress"], "GetUsers -Notification", ex.StackTrace + ex.Message);

//            }
//            catch (Exception exs)
//            {
//                ServicesRepo objrepo = new ServicesRepo(_configuration);
//                objrepo.SendMail(_configuration["Log:ErroAddress"], "GetUsers -Notification", exs.StackTrace + exs.Message);
//            }
//            return "";
//        }

//    }
//}
