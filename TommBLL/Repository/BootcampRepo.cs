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

namespace TommBLL.Repository
{
    public class BootcampRepo:IBootcamp
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public BootcampRepo(IConfiguration configuration, IServices services)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        #endregion

        public async Task<UserBootcampHistory> GetBootcampList(long UserId)
        {
            UserBootcampHistory objBootcamp = null;
            int IsBootcamp = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetbootcampTaskList", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            objBootcamp = new UserBootcampHistory();
                            IsBootcamp = Convert.ToInt16(reader["IsBootcamp"]);
                            if (IsBootcamp >= 1)
                            {
                                objBootcamp.Id = Convert.ToInt64(reader["Id"]);
                                objBootcamp.UserId = Convert.ToInt64(reader["UserId"]);
                                objBootcamp.TasksJson = (reader["Tasks_Json"].ToString());
                                objBootcamp.BootcampJobs = (reader["BootcampJobs"]).ToString();
                            }
                            else
                            {
                                objBootcamp.UserId = Convert.ToInt64(reader["UserId"]);
                                objBootcamp.TasksJson = (reader["Tasks_Json"]).ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetBootcampList" + Environment.NewLine + ex.StackTrace + UserId;
                var fileName = "GetBootcampList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objBootcamp;
        }

        public async Task<Boolean> UpdateBootcamp(UserBootcampHistory model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserBootcampTasks", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@BootcampJobs", model.BootcampJobs);
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
                string ExceptionString = "Repo : UpdateBootcamp" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "UpdateBootcamp - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> ResetBootcamp(long UserId)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_ResetBootcamp", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    Issave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : ResetBootcamp" + Environment.NewLine;
                var fileName = "Logout - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> UpdateBootCampExtraDays(UserBootcampHistory model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateBootcampExtraDays", objCon))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@BootcampJobs", model.BootcampJobs);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    Issave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateBootCampExtraDays" + Environment.NewLine;
                var fileName = "Logout - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<UserBootcampHistory> GetBootcampListV5(long UserId) 
        {
            UserBootcampHistory objBootcamp = null;
            int IsBootcamp = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetbootcampTaskList", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            objBootcamp = new UserBootcampHistory();
                            IsBootcamp = Convert.ToInt16(reader["IsBootcamp"]);
                            if (IsBootcamp >= 1)
                            {
                                objBootcamp.Id = Convert.ToInt64(reader["Id"]);
                                objBootcamp.UserId = Convert.ToInt64(reader["UserId"]);
                                objBootcamp.TasksJson = (reader["Tasks_Json"].ToString());
                                objBootcamp.BootcampJobs = (reader["BootcampJobs"]).ToString();


                                JObject tasksJsonObj = JObject.Parse(objBootcamp.TasksJson);
                                if (!string.IsNullOrEmpty(objBootcamp.BootcampJobs))
                                {
                                    JObject bootcampJobsObj = JObject.Parse(objBootcamp.BootcampJobs);

                                    foreach (var dayKey in bootcampJobsObj)
                                    {
                                        JObject jobsObj = (JObject)tasksJsonObj["Jobs"][dayKey.Key]["Jobs"];
                                        foreach (var jobKey in jobsObj)
                                        {
                                            bool isComplete = (bool)bootcampJobsObj[dayKey.Key][jobKey.Key];
                                            jobKey.Value["Complete"] = isComplete ? 1 : 0;
                                        }
                                    }
                                }
                                objBootcamp.TasksJson = JsonConvert.SerializeObject(tasksJsonObj);

                            }
                            else
                            {
                                objBootcamp.UserId = Convert.ToInt64(reader["UserId"]);
                                objBootcamp.TasksJson = (reader["Tasks_Json"]).ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetBootcampListV5" + Environment.NewLine + ex.Message + ex.StackTrace + " " + UserId;
                var fileName = "GetBootcampListV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objBootcamp;
        }

        public async Task<Boolean> UpdateBootcampV5(UserBootcampHistory model) 
        {
            Boolean Issave = false;
            try
            {
                JObject obj = JObject.Parse(model.TasksJson);
                if (obj["Jobs"] is JObject jobs)
                {
                    JObject bootCampJobs = new JObject();

                    foreach (var dayProperty in jobs.Properties())
                    {
                        if (dayProperty.Value is JObject day && day["Jobs"] is JObject weekJobs)
                        {
                            var jobCompletionValues = weekJobs.Properties()
                                .ToDictionary(j => j.Name, j => (int)j.Value["Complete"] == 1);

                            bootCampJobs[dayProperty.Name] = JObject.FromObject(jobCompletionValues);
                        }
                    }

                    model.BootcampJobs = JsonConvert.SerializeObject(bootCampJobs);
                }
                else
                {
                    Console.WriteLine("Invalid JSON structure");
                }

                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserBootcampTasks", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@BootcampJobs", model.BootcampJobs);
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
                string ExceptionString = "Repo : UpdateBootcampV5" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "UpdateBootcampV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
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
