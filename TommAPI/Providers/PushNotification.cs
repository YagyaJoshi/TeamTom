using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using System.Net;
using System.IO;
using TommDAL.ViewModel;
using MySql.Data.MySqlClient;
using System.Data;
using TommBLL.Interface;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TommAPI.Providers
{
    public static class PushNotification
    {
        public static MySqlConnection objCon;
        public static Task<string> Send(string txtmsg, string objMessage, string DeviceTokens, string Key, IConfiguration _configuration, long UserId)
        {
            string serverKey = _configuration["PushNotification:FCMKey"];
            var result = "-1";
            try
            {
                var webAddr = _configuration["PushNotification:FCMUrl"];
                var body = "";
                //objMessage = "It's " + objMessage + " Day";
                objMessage = objMessage + " ";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Method = "POST";
                //await Task.Yield();
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"to\": \"" + DeviceTokens + "\",\"notification\": {\"title\": \"" + objMessage + "\",\"body\": \"" + body + "\"},\"priority\":10}";
                    //registration_ids, array of strings -  to, single recipient
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }

                //httpWebRequest.Abort();

            }
            catch (Exception ex)
            {
                result = ex.ToString();

            }

            //SavePushNotification(UserId, DeviceTokens, result, _configuration);
            return Task.FromResult(result);

        }
        public static List<UserTaskHisV3> GetUserDetailsforNotificationV2(IConfiguration _configuration)
        {
            List<UserTaskHisV3> user = new List<UserTaskHisV3>();
            try
            {
                objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserDetailsForNotificationV2", objCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Add(new UserTaskHisV3()
                            {
                                UserId = Convert.ToInt32(reader["Id"]),
                                DeviceToken = (reader["DeviceToken"]).ToString(),
                                DeviceType = (reader["DeviceType"]).ToString(),
                                NotifyMe = Convert.ToBoolean((reader["NotifyMe"])),
                                DayTitle = "New guided session just dropped, check it out!",

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();

                throw ex;

            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return user;
        }

        public static List<UserTaskHisV3> GetUserDetailsforNotification(Int32 WeekNumber, Int32 IsDay, IConfiguration _configuration)  
        {
            List<UserTaskHisV3> user = new List<UserTaskHisV3>();
            try
            {
                objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                MySqlDataReader reader;
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserDetailsForNotification", objCon))
                {
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);

                    cmd.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        user.Add(new UserTaskHisV3()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            TasksJson = Convert.ToString(reader["TasksJson"]),
                            DayTitle = Convert.ToString(reader["DayTitle"]),
                            LevelId = Convert.ToString(reader["LevelId"]),
                            DeviceToken = (reader["DeviceToken"]).ToString(),
                            DeviceType = (reader["DeviceType"]).ToString(),
                            NotifyMe = Convert.ToBoolean((reader["NotifyMe"]))                          
                    });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                throw ex;

            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return user;
        }
        public static DateTime GetStartOfWeek(DateTime input)
        {
            int k = ((int)input.DayOfWeek);
            // Using +6 here leaves Monday as 0, Tuesday as 1 etc.
            int dayOfWeek = (((int)input.DayOfWeek) + 6) % 7;
            return input.Date.AddDays(-dayOfWeek);
        }

        public static Int32 GetWeek()
        {
            DateTime periodStart = Convert.ToDateTime("2019-02-25");
            DateTime periodEnd = DateTime.Now;
            // periodStart = GetStartOfWeek(periodStart);
            //periodEnd = GetStartOfWeek(periodEnd);
            int days = (int)(periodEnd - periodStart).TotalDays;
            int weeknum = (days / 7) % 8;
            return weeknum + 1;
        }

        public static int SavePushNotification(long UserId, string DeviceToken, string Result, IConfiguration _configuration)
        {
            int Issave = 0;
            try
            {
                using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    using (MySqlCommand Scmd = new MySqlCommand("sp_SavePushNotification", connection))
                    {
                        Scmd.Parameters.AddWithValue("@UserId", UserId);
                        Scmd.Parameters.AddWithValue("@DeviceToken", DeviceToken);
                        Scmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        Scmd.Parameters.AddWithValue("@Result", Result);
                        Scmd.CommandType = CommandType.StoredProcedure;
                        Scmd.ExecuteNonQuery();
                        Scmd.Dispose();
                        Issave = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public static int SavePushNotification(string Result, IConfiguration _configuration)
        {
            int Issave = 0;
            try
            {
                using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    using (MySqlCommand Scmd = new MySqlCommand("sp_SavePushNotificationV5", connection))
                    {
                        Scmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        Scmd.Parameters.AddWithValue("@Result", Result);
                        Scmd.CommandType = CommandType.StoredProcedure;
                        Scmd.ExecuteNonQuery();
                        Scmd.Dispose();
                        Issave = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                //objCon.Close();
                //objCon.Dispose();
                throw ex;
            }
            finally
            {
                //objCon.Close();
                //objCon.Dispose();
            }
            return Issave;
        }
    }
}
