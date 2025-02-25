using Google.Apis.AndroidPublisher.v3.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Repository
{
   public class ChritmasRepo: IChritmas
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public ChritmasRepo(IConfiguration configuration, IServices services)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        #endregion

        public async Task<UserChristmasHistory> GetChritmasList(long UserId)
        {
            UserChristmasHistory objchristmas = null;
            int IsChristmas = 0;
            try
            {
               
                using (MySqlCommand cmd = new MySqlCommand("sp_GetChristmasTaskList", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);                   
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            objchristmas = new UserChristmasHistory();
                            IsChristmas = Convert.ToInt16(reader["IsChristmas"]);
                            if (IsChristmas >= 1)
                            {
                                objchristmas.Id = Convert.ToInt64(reader["Id"]);
                                objchristmas.UserId = Convert.ToInt64(reader["UserId"]);
                                objchristmas.TasksJson = (reader["Tasks_Json"].ToString());
                                objchristmas.ChristmasJobs = (reader["ChristmasJobs"]).ToString();
                            }
                            else
                            {
                                objchristmas.UserId = Convert.ToInt64(reader["UserId"]);
                                objchristmas.TasksJson = (reader["Tasks_Json"]).ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetChritmasList" + Environment.NewLine + ex.StackTrace + UserId;
                var fileName = "GetChritmasList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objchristmas;
        }

        public async Task<Boolean> UpdateChritmas(UserChristmasHistory model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserChristmasTasks", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@ChristmasJobs", model.ChristmasJobs);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                objCon.Close();
                Issave = true;
                
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateChritmas" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "UpdateChritmas - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<UserChristmasHistory> GetChritmasListV5(long UserId) 
        {
            UserChristmasHistory objChristmas = null; 
            int IsChristmas = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetChristmasTaskList", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();

                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int isChristmas = Convert.ToInt32(reader["IsChristmas"]);

                            objChristmas = new UserChristmasHistory
                            {
                                UserId = Convert.ToInt64(reader["UserId"]),
                                TasksJson = Convert.ToString(reader["Tasks_Json"])
                            };

                            if (isChristmas >= 1)
                            {
                                objChristmas.Id = Convert.ToInt64(reader["Id"]);
                                objChristmas.ChristmasJobs = Convert.ToString(reader["ChristmasJobs"]);

                                if (!string.IsNullOrEmpty(objChristmas.ChristmasJobs))
                                {
                                    JObject tasksJsonObj = JObject.Parse(objChristmas.TasksJson);
                                    JObject christmasJobsObj = JObject.Parse(objChristmas.ChristmasJobs);

                                    JObject jobsObj = null;
                                    foreach (var dayKey in christmasJobsObj)
                                    {
                                        jobsObj = (JObject)tasksJsonObj["Jobs"][dayKey.Key]["Jobs"];
                                        foreach (var jobKey in jobsObj)
                                        {
                                            if (christmasJobsObj[dayKey.Key][jobKey.Key] != null)
                                            {
                                                bool isComplete = (bool)christmasJobsObj[dayKey.Key][jobKey.Key];
                                                jobKey.Value["Complete"] = isComplete ? 1 : 0;
                                            }
                                        }
                                    }

                                    objChristmas.TasksJson = JsonConvert.SerializeObject(tasksJsonObj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetChritmasListV5" + Environment.NewLine + ex.StackTrace + "UserId " + UserId;
                var fileName = "GetChritmasListV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objChristmas;
        }

        public async Task<Boolean> UpdateChritmasV5(UserChristmasHistory model)
        {
            Boolean Issave = false;
            try
            {
                JObject obj = JObject.Parse(model.TasksJson);
                if (obj["Jobs"] is JObject jobs)
                {
                    JObject christmasJobs = new JObject();

                    foreach (var weekProperty in jobs.Properties())
                    {
                        if (weekProperty.Value is JObject week && week["Jobs"] is JObject weekJobs)
                        {
                            var jobCompletionValues = weekJobs.Properties()
                                .ToDictionary(j => j.Name, j => (int)j.Value["Complete"] == 1);

                            christmasJobs[weekProperty.Name] = JObject.FromObject(jobCompletionValues);
                        }
                    }                    

                    model.ChristmasJobs = JsonConvert.SerializeObject( christmasJobs);                   
                }
                else
                {
                    Console.WriteLine("Invalid JSON structure");
                }
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserChristmasTasksV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@ChristmasJobs", model.ChristmasJobs);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                objCon.Close();
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateChritmasV5" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "UpdateChritmasV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<Boolean> AddChritmasTasksV5(AddChristmasTask model)
        {
            Boolean Issave = false;
            try
            {
                JObject taskJsonObject = JObject.Parse(model.tasksJson);

                JObject tasksJsonObj = new JObject();

                string names = taskJsonObject["Jobs"].Children().OfType<JProperty>().FirstOrDefault()?.Name;
                using (MySqlCommand cmd = new MySqlCommand("sp_GetChristmasTaskList", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.userId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            var TasksJson = (reader["Tasks_Json"].ToString());
                            tasksJsonObj = JObject.Parse(TasksJson);
                            tasksJsonObj["Jobs"][names] = taskJsonObject["Jobs"][names];
                        }
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand("sp_AddUserChristmasTasksV5", objCon))
                {
                    var tasksJsons = JsonConvert.SerializeObject(tasksJsonObj);
                    cmd.Parameters.AddWithValue("@userId", model.userId);
                    cmd.Parameters.AddWithValue("@tasksJson", tasksJsons);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                objCon.Close();
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : AddChritmasV5" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "AddChritmasV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }
    }
}
