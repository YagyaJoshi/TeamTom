using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TommBLL.Interface;
using TommDAL.Models;
using System.Linq;
using TommDAL.ViewModel;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;

namespace TommBLL.Repository
{
    public class JobsRepo : IJobs
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public JobsRepo(IConfiguration configuration, IServices services)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        #endregion

        public async Task<TaskJobLevel> GetUserJobsByDay(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            TaskJobLevel userTasksMst = new TaskJobLevel();
            try
            {


                DataSet ds = new DataSet();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserAllJobsByDay", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", CurrentDate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    // reader = cmd.ex();
                    adapter.Fill(ds);
                    //  ds = reader;
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];
                    var day = CurrentDate.DayOfWeek.ToString();
                    //  var day = dateValue.ToString("dddd");
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        var DisplayText = "";
                        if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 2 || Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 3)
                        {
                            DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);

                        }
                        if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 1)
                        {
                            DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);
                        }

                        userTasksMst.ListUserTask.Add(new Usertask()
                        {

                            Id = Convert.ToInt64(dt1.Rows[i]["Id"]),
                            UserId = Convert.ToInt64(dt1.Rows[i]["UserId"]),
                            TasksJson = (dt1.Rows[i]["TasksJson"]).ToString(),
                            LevelId = Convert.ToUInt16(dt1.Rows[i]["LevelId"]),
                            DisplayText = DisplayText,
                            DayTitle = (dt1.Rows[i]["DayTitle"]).ToString(),
                            DisplayTitle = (dt1.Rows[i]["DisplayTitle"]).ToString(),
                        });
                    }
                    if (dt2.Rows.Count >= 1)
                    {
                        int data = Convert.ToInt16(dt2.Rows[0]["isData"]);
                        if (data != 0)
                        {
                            userTasksMst.UserTaskHistory = new UserTasksHis();
                            userTasksMst.UserTaskHistory.Id = Convert.ToInt64(dt2.Rows[0]["Id"]);
                            userTasksMst.UserTaskHistory.UserId = Convert.ToInt64(dt2.Rows[0]["UserId"]);
                            userTasksMst.UserTaskHistory.UserTasksUpdate = (dt2.Rows[0]["UserTasksUpdate"]).ToString();
                            userTasksMst.UserTaskHistory.TaskDate = Convert.ToDateTime(dt2.Rows[0]["TaskDate"]);
                            userTasksMst.UserTaskHistory.DisplayTitle = (dt2.Rows[0]["DisplayTitle"]).ToString();
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserJobsByDay" + Environment.NewLine;
                var fileName = "GetUserJobsByDay - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMst;

        }


        public async Task<List<TaskDayList>> GetUserDayList()
        {
            List<TaskDayList> objList = new List<TaskDayList>();
            try
            {
               
                using (MySqlCommand cmd = new MySqlCommand("sp_GetDayList", objCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            objList.Add(new TaskDayList()
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                Day = reader["DayText"].ToString(),
                                LevelId = Convert.ToInt16(reader["LevelId"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserDayList" + Environment.NewLine;
                var fileName = "GetUserDayList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return objList;

        }

        public async Task<Usertask> GetUserDayJob(long UserId, int TaskId)
        {
            Usertask objList = null;
            try
            {
               
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserDayJob", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@TaskId", TaskId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            objList = new Usertask();
                            objList.Id = Convert.ToInt64(reader["Id"]);
                            objList.UserId = Convert.ToInt64(reader["UserId"]);
                            objList.TasksJson = (reader["Tasks_Json"]).ToString();
                            objList.LevelId = Convert.ToInt16(reader["LevelId"]);
                            objList.DayTitle = (reader["DayTitle"]).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserDayJob" + Environment.NewLine;
                var fileName = "GetUserDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return objList;

        }


        public async Task<Boolean> UpdateTaskJob(Usertask model)
        {
            Boolean Issave = false;
            try
            {
                MySqlCommand cmd = new MySqlCommand("sp_SaveUserTask", objCon);
                cmd.Parameters.AddWithValue("@TaskId", model.Id);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                cmd.CommandType = CommandType.StoredProcedure;
                await objCon.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateTaskJob" + Environment.NewLine;
                var fileName = "UpdateTaskJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> UpdateTaskJobV5(UsertaskV5 model)
        {
            Boolean Issave = false;
            try
            {
                long id = 0;
                List<UpdateJson> updatedJson = new List<UpdateJson>();
                JObject tasksJson = null;
                MySqlDataReader reader = null;
                MySqlCommand cmd = new MySqlCommand("sp_SaveUserTaskV5", objCon);
                cmd.Parameters.AddWithValue("@TaskId", model.Id);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                cmd.Parameters.AddWithValue("@IsDay", model.IsDay);
                cmd.Parameters.AddWithValue("@WeekNumber", model.WeekNumber);
                cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                cmd.Parameters.AddWithValue("@IsUpdateForAllLevel", model.IsUpdateForAllLevel);             
                cmd.CommandType = CommandType.StoredProcedure;
                await objCon.OpenAsync();
                await cmd.ExecuteNonQueryAsync();         

                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateTaskJobV5" + Environment.NewLine;
                var fileName = "UpdateTaskJobV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> BulkUpdateTaskJobV5(UsertaskV5 model) 
        {
            Boolean Issave = false;
            try
            {
                long id = 0;
                List<UpdateJson> updatedJson = new List<UpdateJson>();
                JObject tasksJson = null;
                using (MySqlCommand cmd = new MySqlCommand("sp_newSaveUserTaskV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var isExist = Convert.ToInt64(reader["isExist"]);
                            if (isExist == 2)
                            {
                                var existingJson = reader["Tasks_Json"].ToString();
                                id = Convert.ToInt64(reader["Id"]);

                                // Deserialize the existing JSON into a JObject
                                tasksJson = JsonConvert.DeserializeObject<JObject>(existingJson);

                                var jobProperties = tasksJson != null ? tasksJson.Properties().Where(p => p.Name.StartsWith("Job")) : null;
                                var lastNumericProperty = jobProperties != null && jobProperties.Any() ? jobProperties.Select(p => int.Parse(p.Name.Substring(3))).Max() : 0;

                                var newPropertyName = "Job" + (lastNumericProperty + 1);

                                // Add the new task to tasksJson
                                tasksJson[newPropertyName] = model.NewTask;

                                updatedJson.Add(new UpdateJson
                                {
                                    Id = id,
                                    TaskJson = JsonConvert.SerializeObject(tasksJson)
                                });
                            }
                        }
                    }
                }
                objCon.Close();
                BulkUpdateTaskJob(updatedJson, model.UserId, model.LevelId);
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : BulkUpdateTaskJobV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace + "UserId- " + model.UserId;
                var fileName = "BulkUpdateTaskJobV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }
   
        public Boolean BulkUpdateTaskJob(List<UpdateJson> updatedJson, long UserId, long LevelId) 
        {
            bool IsSave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_BulkUpdateUserTaskV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    for (int i = 0; i < updatedJson.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i + 1}", updatedJson[i].Id);
                        cmd.Parameters.AddWithValue($"@TasksJson{i + 1}", updatedJson[i].TaskJson);
                    }

                    for (int i = updatedJson.Count; i < 14; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i + 1}", 0);
                        cmd.Parameters.AddWithValue($"@TasksJson{i + 1}", string.Empty);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    cmd.ExecuteNonQuery();
                    IsSave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : BulkUpdateTaskJob" + Environment.NewLine + ex.Message + "StackTrace - " + ex.StackTrace;
                var fileName = "BulkUpdateTaskJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return IsSave;
        }
        public async Task<Boolean> RestoreDayTasks(long UserId, int TaskId)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_RestoreDayTasks", objCon))
                {
                    cmd.Parameters.AddWithValue("@TaskId", TaskId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : RestoreDayTasks" + Environment.NewLine;
                var fileName = "RestoreDayTasks - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<Boolean> UpdateUserDayJobs(UserTaskHistory model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobs", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDayJobs" + Environment.NewLine;
                var fileName = "UpdateUserDayJobs - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }




        public async Task<Boolean> DeleteUserHistoryDayJobs(long UserTaskHistoryId)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_DeleteUserTaskHistoryJob", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserTaskHistoryId", UserTaskHistoryId);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDayJobs" + Environment.NewLine;
                var fileName = "UpdateUserDayJobs - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<TaskJobLevel> GetNextDayJob(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            TaskJobLevel userTasksMst = new TaskJobLevel();
            try
            {

                DataSet ds = new DataSet();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetNextDayTask", objCon))
                {
                    DateTime Tomorrowdate = CurrentDate.AddDays(1);
                    IsDay = IsDay == 6 ? 0 : IsDay + 1;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", Tomorrowdate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    // reader = cmd.ex();
                    adapter.Fill(ds);
                    //  ds = reader;
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];

                    if (dt1.Rows.Count >= 1)
                    {
                        int data = Convert.ToInt16(dt1.Rows[0]["isData"]);
                        if (data != 1)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                userTasksMst.ListUserTask.Add(new Usertask()
                                {
                                    Id = Convert.ToInt64(dt2.Rows[i]["Id"]),
                                    UserId = Convert.ToInt64(dt2.Rows[i]["UserId"]),
                                    TasksJson = (dt2.Rows[i]["TasksJson"]).ToString(),
                                    LevelId = Convert.ToUInt16(dt2.Rows[i]["LevelId"]),
                                    DisplayText = (dt2.Rows[i]["DisplayText"]).ToString(),
                                    DisplayTitle = (dt2.Rows[i]["DisplayTitle"]).ToString(),
                                });
                            }
                        }
                        else
                        {
                            userTasksMst.UserTaskHistory = new UserTasksHis();
                            userTasksMst.UserTaskHistory.Id = Convert.ToInt64(dt2.Rows[0]["Id"]);
                            userTasksMst.UserTaskHistory.UserId = Convert.ToInt64(dt2.Rows[0]["UserId"]);
                            userTasksMst.UserTaskHistory.UserTasksUpdate = (dt2.Rows[0]["UserTasksUpdate"]).ToString();
                            userTasksMst.UserTaskHistory.TaskDate = Convert.ToDateTime(dt2.Rows[0]["TaskDate"]);
                            userTasksMst.UserTaskHistory.DisplayTitle = (dt2.Rows[0]["DisplayTitle"]).ToString();
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetNextDayJob" + Environment.NewLine;
                var fileName = "GetNextDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMst;

        }

        public async Task<Boolean> UpdateUserRoolOverDay(UserTaskHistory model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobs", objCon))
                {
                    model.TaskDate = model.TaskDate.AddDays(1);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TommorowTasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserRoolOverDay" + Environment.NewLine;
                var fileName = "UpdateUserRoolOverDay - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<string> GetDayTitle(int WeekNumber, int IsDay, long UserId)
        {
            string DisplayTitle = null;
            try
            {
                
                using (MySqlCommand cmd = new MySqlCommand("sp_GetDayTitle", objCon))
                {
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            DisplayTitle = (reader["DisplayTitle"]).ToString();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetDayTitle" + Environment.NewLine;
                var fileName = "GetDayTitle - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return DisplayTitle;

        }


        public async Task<Boolean> SaveSwapDayJob(long UserId, long CurrentTaskId, long SwapTaskId)
        {
            Boolean Issave = false;
            try
            {
                MySqlCommand cmd = new MySqlCommand("sp_JobDaySwapV2", objCon);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@CurrentTaskId", CurrentTaskId);
                cmd.Parameters.AddWithValue("@SwapTaskId", SwapTaskId);
                cmd.CommandType = CommandType.StoredProcedure;
                await objCon.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : SaveSwapDayJob" + Environment.NewLine + ex.StackTrace + (UserId, CurrentTaskId, SwapTaskId);
                var fileName = "SaveSwapDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<TaskJobLevelV2> GetUserNewJobsByDayV2(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            TaskJobLevelV2 userTasksMstV2 = new TaskJobLevelV2();
            try
            {
                DataSet ds = new DataSet();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserAllJobsByDayV2", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", CurrentDate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    //  cmd.Parameters.AddWithValue("@isDatas",1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    // reader = cmd.ex();
                    adapter.Fill(ds);
                    //  ds = reader;
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];
                    int data = Convert.ToInt16(dt2.Rows[0]["isData"]);
                    var day = CurrentDate.DayOfWeek.ToString();
                    //  var day = dateValue.ToString("dddd");
                    if (data == 1)
                    {
                        if (dt1.Rows.Count >= 1)
                        {
                            userTasksMstV2.UserTaskHistory = new UserTaskHisV2();
                            int isTitleblank = Convert.ToInt16(dt2.Rows[0]["isTitleblank"]);
                            if (isTitleblank == 1)
                            {

                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt2.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt2.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt2.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt2.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt2.Rows[0]["DayTitle"]).ToString();
                            }
                            else
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt1.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt1.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt1.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt1.Rows[0]["DayTitle"]).ToString();
                            }


                            userTasksMstV2.UserTaskHistory.Id = Convert.ToInt64(dt1.Rows[0]["Id"]);
                            userTasksMstV2.UserTaskHistory.UserId = Convert.ToInt64(dt1.Rows[0]["UserId"]);
                            userTasksMstV2.UserTaskHistory.UserTasksUpdate = (dt1.Rows[0]["UserTasksUpdate"]).ToString();
                            userTasksMstV2.UserTaskHistory.TaskDate = Convert.ToDateTime(dt1.Rows[0]["TaskDate"]);
                        }
                    }
                    else
                    {
                        if (dt1.Rows.Count >= 1)
                        {
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                var DisplayText = "";
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 2 || Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 3)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);

                                }
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 1)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);
                                }

                                userTasksMstV2.ListUserTask.Add(new UsertaskV2()
                                {

                                    Id = Convert.ToInt64(dt1.Rows[i]["Id"]),
                                    UserId = Convert.ToInt64(dt1.Rows[i]["UserId"]),
                                    TasksJson = (dt1.Rows[i]["TasksJson"]).ToString(),
                                    LevelId = Convert.ToUInt16(dt1.Rows[i]["LevelId"]),
                                    DisplayText = DisplayText,
                                    DayTitle = (dt1.Rows[i]["DayTitle"]).ToString(),
                                    DisplayTitle = (dt1.Rows[i]["DisplayTitle"]).ToString(),
                                    FocusTitle = (dt1.Rows[i]["FocusTitle"]).ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                //  string ExceptionString = "Repo : GetUserNewJobsByDayV2" + Environment.NewLine;
                string ExceptionString = "Repo : GetUserNewJobsByDayV2" + Environment.NewLine + ex.StackTrace + UserId + IsDay + CurrentDate + WeekNumber;
                var fileName = "GetUserNewJobsByDayV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMstV2;

        }


        public async Task<TaskJobLevelV2> GetNextDayJobV2(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
       {
            TaskJobLevelV2 userTasksMstV2 = new TaskJobLevelV2();
            try
            {

                DataSet ds = new DataSet();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserAllJobsByDayV2", objCon))
                {
                    DateTime Tomorrowdate = CurrentDate.AddDays(1);
                    IsDay = IsDay == 6 ? 0 : IsDay + 1;
                    var day = Tomorrowdate.DayOfWeek.ToString();
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", Tomorrowdate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    // reader = cmd.ex();
                    adapter.Fill(ds);
                    //  ds = reader;
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];
                    int data = Convert.ToInt16(dt2.Rows[0]["isData"]);
                    if (dt1.Rows.Count >= 1)
                    {

                        if (data != 1)
                        {
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                var DisplayText = "";
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 2 || Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 3)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);

                                }
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 1)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);
                                }

                                userTasksMstV2.ListUserTask.Add(new UsertaskV2()
                                {
                                    Id = Convert.ToInt64(dt1.Rows[i]["Id"]),
                                    UserId = Convert.ToInt64(dt1.Rows[i]["UserId"]),
                                    TasksJson = (dt1.Rows[i]["TasksJson"]).ToString(),
                                    LevelId = Convert.ToUInt16(dt1.Rows[i]["LevelId"]),
                                    DisplayText = DisplayText,
                                    DayTitle = (dt1.Rows[i]["DayTitle"]).ToString(),
                                    DisplayTitle = (dt1.Rows[i]["DisplayTitle"]).ToString(),
                                    FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString()
                                });
                            }
                        }
                        else
                        {
                            userTasksMstV2.UserTaskHistory = new UserTaskHisV2();

                            int isTitleblank = Convert.ToInt16(dt2.Rows[0]["isTitleblank"]);
                            if (isTitleblank == 1)
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt2.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt2.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt2.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt2.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt2.Rows[0]["DayTitle"]).ToString();
                            }
                            else
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt1.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt1.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt1.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt1.Rows[0]["DayTitle"]).ToString();
                            }

                            userTasksMstV2.UserTaskHistory.Id = Convert.ToInt64(dt1.Rows[0]["Id"]);
                            userTasksMstV2.UserTaskHistory.UserId = Convert.ToInt64(dt1.Rows[0]["UserId"]);
                            userTasksMstV2.UserTaskHistory.UserTasksUpdate = (dt1.Rows[0]["UserTasksUpdate"]).ToString();
                            userTasksMstV2.UserTaskHistory.TaskDate = Convert.ToDateTime(dt1.Rows[0]["TaskDate"]);
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetNextDayJobV2" + Environment.NewLine + ex.StackTrace + UserId + IsDay + CurrentDate + WeekNumber;
                //  string ExceptionString = "Repo : GetNextDayJobV2" + Environment.NewLine;
                var fileName = "GetNextDayJobV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMstV2;

        }
        public TaskJobLevelV2 GetNextDayJobV5(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            TaskJobLevelV2 userTasksMstV2 = new TaskJobLevelV2(); 
            try
            {

                DataSet ds = new DataSet();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserAllJobsByDayV5", objCon))
                {
                    DateTime Tomorrowdate = CurrentDate.AddDays(1);
                    IsDay = IsDay == 6 ? 0 : IsDay + 1;
                    var day = Tomorrowdate.DayOfWeek.ToString();
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", Tomorrowdate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    objCon.Open();
                    // reader = cmd.ex();
                    adapter.Fill(ds);
                    //  ds = reader;
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];
                    int data = Convert.ToInt16(dt2.Rows[0]["isData"]);
                    if (dt1.Rows.Count >= 1)
                    {

                        if (data != 1)
                        {
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                var DisplayText = "";
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 2 || Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 3)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);

                                }
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 1)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);
                                }

                                userTasksMstV2.ListUserTask.Add(new UsertaskV2()
                                {
                                    Id = Convert.ToInt64(dt1.Rows[i]["Id"]),
                                    UserId = Convert.ToInt64(dt1.Rows[i]["UserId"]),
                                    TasksJson = (dt1.Rows[i]["TasksJson"]).ToString(),
                                    LevelId = Convert.ToUInt16(dt1.Rows[i]["LevelId"]),
                                    DisplayText = DisplayText,
                                    DayTitle = (dt1.Rows[i]["DayTitle"]).ToString(),
                                    DisplayTitle = (dt1.Rows[i]["DisplayTitle"]).ToString(),
                                    FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString()
                                });
                            }
                        }
                        else
                        {
                            userTasksMstV2.UserTaskHistory = new UserTaskHisV2();

                            int isTitleblank = Convert.ToInt16(dt2.Rows[0]["isTitleblank"]);
                            if (isTitleblank == 1)
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt2.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt2.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt2.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt2.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt2.Rows[0]["DayTitle"]).ToString();
                            }
                            else
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt1.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt1.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt1.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt1.Rows[0]["DayTitle"]).ToString();
                            }

                            userTasksMstV2.UserTaskHistory.Id = Convert.ToInt64(dt1.Rows[0]["Id"]);
                            userTasksMstV2.UserTaskHistory.UserId = Convert.ToInt64(dt1.Rows[0]["UserId"]);
                            userTasksMstV2.UserTaskHistory.UserTasksUpdate = (dt1.Rows[0]["UserTasksUpdate"]).ToString();
                            userTasksMstV2.UserTaskHistory.TaskDate = Convert.ToDateTime(dt1.Rows[0]["TaskDate"]);
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetNextDayJobV5" + Environment.NewLine + ex.StackTrace + UserId + IsDay + CurrentDate + WeekNumber;
                //  string ExceptionString = "Repo : GetNextDayJobV2" + Environment.NewLine;
                var fileName = "GetNextDayJobV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMstV2;

        }

        public async Task<Boolean> UpdateUserDayJobsV2(UserUpdateTaskJobV2 model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobsV2", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.DayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.Level1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.Level2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusDayTitle);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UserTaskHistoryV2" + Environment.NewLine;
                var fileName = "UserTaskHistoryV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<Tuple<Boolean, Int64>> UpdateUserDayJobsV3(UserUpdateTaskJobV3 model)
        {
            Boolean Issave = false;
            Int64 count = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobsV3", objCon))
                {

                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.DayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.Level1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.Level2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusDayTitle);
                    cmd.Parameters.AddWithValue("@IsComplete", model.IsComplete);

                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();

                }
                if (model.IsComplete == true)
                {
                    
                    using (MySqlCommand Ccmd = new MySqlCommand("sp_Getcount", objCon))
                    {

                        Ccmd.Parameters.AddWithValue("@UserId", model.UserId);
                        Ccmd.Parameters.AddWithValue("@LastUpdated", model.TaskDate);
                        Ccmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await Ccmd.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            if (reader.HasRows)
                            {
                                count = Convert.ToInt64(reader["Counter"] != DBNull.Value ? reader   ["Counter"] : 0);
                            }
                        }
                    }

                }
                Issave = true;


            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDayJobsV3" + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV3 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Tuple.Create(Issave, count);
        }



        public async Task<Boolean> UpdateUserRoolOverDayV2(UserUpdateTaskJobV2 model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobsV2", objCon))
                {
                    model.TaskDate = model.TaskDate.AddDays(1);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TommorowTasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);

                    cmd.Parameters.AddWithValue("@DisplayTitle", model.TomorrowDisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.TomorrowDayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.TomorrowLevel1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.TomorrowLevel2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusTomorrowTitle);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserRoolOverDayV2" + Environment.NewLine;
                var fileName = "UpdateUserRoolOverDayV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<Boolean> UpdateUserDayJobsV3(UserUpdateTaskJobV2 model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserRollOverDayJobsV3", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.DayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.Level1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.Level2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusDayTitle);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UserTaskHistoryV2" + Environment.NewLine;
                var fileName = "UserTaskHistoryV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> UpdateUserRoolOverDayV3(UserUpdateTaskJobV2 model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserRollOverDayJobsV3_NextDay", objCon))
                {
                    model.TaskDate = model.TaskDate.AddDays(1);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TommorowTasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);

                    cmd.Parameters.AddWithValue("@DisplayTitle", model.TomorrowDisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.TomorrowDayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.TomorrowLevel1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.TomorrowLevel2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusTomorrowTitle);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserRoolOverDayV3" + Environment.NewLine;
                var fileName = "UpdateUserRoolOverDayV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> UpdateUserWeekFocusV2(UserWeekFocusJobV2 model)
        {
            Boolean Issave = false;
            try
            {
                model.SetL2query = changeWeek(model.SetDayL2);
                model.SetL3query = changeWeek(model.SetDayL3);
                using (MySqlCommand cmd = new MySqlCommand("sp_ChangeWeekFocusV2", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TaskId", model.TaskId);
                    cmd.Parameters.AddWithValue("@SetDayL3", model.SetL3query);
                    cmd.Parameters.AddWithValue("@SetDayL2", model.SetL2query);
                    cmd.Parameters.AddWithValue("@L2Day", model.SetDayL2);
                    cmd.Parameters.AddWithValue("@L3Day", model.SetDayL3);

                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserWeekFocusV2" + Environment.NewLine;
                var fileName = "UpdateUserWeekFocusV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> UpdateUserWeekFocusV5(UserWeekFocusJobV2 model) 
        {
            Boolean Issave = false;
            try
            {
                model.SetL2query = changeWeek(model.SetDayL2);
                model.SetL3query = changeWeek(model.SetDayL3);
                using (MySqlCommand cmd = new MySqlCommand("sp_ChangeWeekFocusV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TaskId", model.TaskId);
                    cmd.Parameters.AddWithValue("@SetDayL3", model.SetL3query);
                    cmd.Parameters.AddWithValue("@SetDayL2", model.SetL2query);
                    cmd.Parameters.AddWithValue("@L2Day", model.SetDayL2);
                    cmd.Parameters.AddWithValue("@L3Day", model.SetDayL3);

                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserWeekFocusV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdateUserWeekFocusV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<List<TaskDayList>> GetUserDayListV2(long UserId)
        {
            List<TaskDayList> objList = new List<TaskDayList>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetDayListV2", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new TaskDayList()
                            {
                                Id = Convert.ToInt64(reader["TaskId"]),
                                Day = reader["DayText"].ToString(),
                                LevelId = Convert.ToInt16(reader["LevelId"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserDayListV2" + Environment.NewLine;
                var fileName = "GetUserDayListV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return objList;

        }


        public async Task<string> GetDayTitleV2(int WeekNumber, int IsDay, long UserId)
        {
            string DisplayTitle = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetDayTitleV2", objCon))
                {
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            DisplayTitle = (reader["DisplayTitle"]).ToString();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetDayTitleV2" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                var fileName = "GetDayTitleV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return DisplayTitle;

        }

        public async Task<Boolean> RestoreDayTasksV2(long UserId, int TaskId)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_RestoreDayTasksV2", objCon))
                {
                    cmd.Parameters.AddWithValue("@TaskId", TaskId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : RestoreDayTasksV2" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                var fileName = "RestoreDayTasksV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<Boolean> RestoreDayTasksV5(long UserId)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_RestoreDayTasksV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : RestoreDayTasksV5" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                var fileName = "RestoreDayTasksV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public string changeWeek(string WeekDay)
        {
            string FocusDay = "";
            if (WeekDay == "Monday")
            {
                FocusDay = "IsMonday=1,IsTuesday=0,IsWednesday=0,IsThursday=0,IsFriday=0,IsSaturday=0,IsSunday=0,IsDay=1,Sortby=1";
            }
            else if (WeekDay == "Tuesday")
            {
                FocusDay = "IsMonday=0,IsTuesday=1,IsWednesday=0,IsThursday=0,IsFriday=0,IsSaturday=0,IsSunday=0,IsDay=2,Sortby=2";
            }
            else if (WeekDay == "Wednesday")
            {
                FocusDay = "IsMonday=0,IsTuesday=0,IsWednesday=1,IsThursday=0,IsFriday=0,IsSaturday=0,IsSunday=0,IsDay=3,Sortby=3";

            }
            else if (WeekDay == "Thursday")
            {
                FocusDay = "IsMonday=0,IsTuesday=0,IsWednesday=0,IsThursday=1,IsFriday=0,IsSaturday=0,IsSunday=0,IsDay=4,Sortby=4";
            }
            else if (WeekDay == "Friday")
            {
                FocusDay = "IsMonday=0,IsTuesday=0,IsWednesday=0,IsThursday=0,IsFriday=1,IsSaturday=0,IsSunday=0,IsDay=5,Sortby=5";
            }
            else if (WeekDay == "Saturday")
            {
                FocusDay = "IsMonday=0,IsTuesday=0,IsWednesday=0,IsThursday=0,IsFriday=0,IsSaturday=1,IsSunday=0,IsDay=6,Sortby=6";
            }
            else
            {
                FocusDay = "IsMonday=0,IsTuesday=0,IsWednesday=0,IsThursday=0,IsFriday=0,IsSaturday=0,IsSunday=1,IsDay=0,Sortby=7";
            }
            return FocusDay;
        }

        public async Task<bool> UpdateUserDayJobsV4(UserUpdateTaskJobV4 model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserRollOverDayJobsV3", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.DayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.Level1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.Level2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusDayTitle);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDayJobsV4" + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV4 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<bool> UpdateUserRoolOverDayV4(UserUpdateTaskJobV4 model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserRollOverDayJobsV3_NextDay", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    // cmd.Parameters.AddWithValue("@LevelId", model.LevelId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.FutureDayTasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.FutureDate);

                    cmd.Parameters.AddWithValue("@DisplayTitle", model.FutureDayDisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.FutureDayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.FutureDayLevel1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.FutureDayLevel2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusFutureDayTitle);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserRoolOverDayV4" + Environment.NewLine;
                var fileName = "UpdateUserRoolOverDayV4 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<TaskJobLevelV2> GetNextDayJobV3(long UserId, int IsDay, DateTime CurrentDate, DateTime FutureDate, int WeekNumber)
        {
            TaskJobLevelV2 userTasksMstV2 = new TaskJobLevelV2();
            try
            {

                DataSet ds = new DataSet();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserAllJobsByDayV2", objCon))
                {
                  
                    string day = FutureDate.DayOfWeek.ToString();
                    IsDay = (int)System.Enum.Parse(typeof(EDay), day);
                    
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", FutureDate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    // reader = cmd.ex();
                    adapter.Fill(ds);
                    //  ds = reader;
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];
                    int data = Convert.ToInt16(dt2.Rows[0]["isData"]);
                    if (dt1.Rows.Count >= 1)
                    {

                        if (data != 1)
                        {
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                var DisplayText = "";
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 2 || Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 3)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);

                                }
                                if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 1)
                                {
                                    DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);
                                }

                                userTasksMstV2.ListUserTask.Add(new UsertaskV2()
                                {
                                    Id = Convert.ToInt64(dt1.Rows[i]["Id"]),
                                    UserId = Convert.ToInt64(dt1.Rows[i]["UserId"]),
                                    TasksJson = (dt1.Rows[i]["TasksJson"]).ToString(),
                                    LevelId = Convert.ToUInt16(dt1.Rows[i]["LevelId"]),
                                    DisplayText = DisplayText,
                                    DayTitle = (dt1.Rows[i]["DayTitle"]).ToString(),
                                    DisplayTitle = (dt1.Rows[i]["DisplayTitle"]).ToString(),
                                    FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString()
                                });
                            }
                        }
                        else
                        {
                            userTasksMstV2.UserTaskHistory = new UserTaskHisV2();

                            int isTitleblank = Convert.ToInt16(dt2.Rows[0]["isTitleblank"]);
                            if (isTitleblank == 1)
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt2.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt2.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt2.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt2.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt2.Rows[0]["DayTitle"]).ToString();
                            }
                            else
                            {
                                userTasksMstV2.UserTaskHistory.FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.Level1Title = (dt1.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.Level2Title = (dt1.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                userTasksMstV2.UserTaskHistory.DisplayTitle = (dt1.Rows[0]["WeekFocusTitle"]).ToString();
                                userTasksMstV2.UserTaskHistory.DayTitle = (dt1.Rows[0]["DayTitle"]).ToString();
                            }

                            userTasksMstV2.UserTaskHistory.Id = Convert.ToInt64(dt1.Rows[0]["Id"]);
                            userTasksMstV2.UserTaskHistory.UserId = Convert.ToInt64(dt1.Rows[0]["UserId"]);
                            userTasksMstV2.UserTaskHistory.UserTasksUpdate = (dt1.Rows[0]["UserTasksUpdate"]).ToString();
                            userTasksMstV2.UserTaskHistory.TaskDate = Convert.ToDateTime(dt1.Rows[0]["TaskDate"]);
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetNextDayJobV2" + Environment.NewLine + ex.StackTrace + UserId + IsDay + CurrentDate + WeekNumber;
                //  string ExceptionString = "Repo : GetNextDayJobV2" + Environment.NewLine;
                var fileName = "GetNextDayJobV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMstV2;

        }

        public async Task<Tuple<bool, long>> UpdateUserDayJobsV4(UserUpdateTaskJobV3 model)
        {
            Boolean Issave = false;
            Int64 count = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobsV3", objCon))
                {

                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.DayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.Level1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.Level2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusDayTitle);
                    cmd.Parameters.AddWithValue("@IsComplete", model.IsComplete);

                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();

                }
                if (model.IsComplete == true)
                {
                   
                    using (MySqlCommand Ccmd = new MySqlCommand("sp_Getcountv2", objCon))
                    {

                        Ccmd.Parameters.AddWithValue("@UserId", model.UserId);
                        Ccmd.Parameters.AddWithValue("@LastUpdated", model.TaskDate);
                        Ccmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await Ccmd.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            if (reader.HasRows)
                            {
                                count = Convert.ToInt64(reader["Counter"] != DBNull.Value ? reader["Counter"] : 0);
                            }
                        }
                    }

                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDayJobsV4" + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV4 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Tuple.Create(Issave, count);
        }

        public async Task<bool> UpdateUserDayJobsV5(UserUpdateTaskJobV3 model)
        {
            Boolean Issave = false;           
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDayJobsV3", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@TasksJson", model.TasksJson);
                    cmd.Parameters.AddWithValue("@TaskDate", model.TaskDate);
                    cmd.Parameters.AddWithValue("@DisplayTitle", model.DisplayTitle);
                    cmd.Parameters.AddWithValue("@DayTitle", model.DayTitle);
                    cmd.Parameters.AddWithValue("@Level1Title", model.Level1Title);
                    cmd.Parameters.AddWithValue("@Level2Title", model.Level2Title);
                    cmd.Parameters.AddWithValue("@FocusTitle", model.FocusDayTitle);
                    cmd.Parameters.AddWithValue("@IsComplete", model.IsComplete);

                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }               
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDayJobsV5" + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<long> GetUserDayStreakV5(string UserId , DateTime TaskDate)  
        {
            Int64 count = 0;
            using (MySqlConnection objCon1 = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_Getcountv2", objCon1))
                    {
                        cmd.Parameters.AddWithValue("@LastUpdated", TaskDate);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        await objCon1.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader =await cmd.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            if (reader.HasRows)
                            {
                                count = Convert.ToInt64(reader["Counter"] != DBNull.Value ? reader["Counter"] : 0);

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //objCon.Close();
                    //objCon.Dispose();
                    string ExceptionString = "Repo : GetUserDayStreakV5" + Environment.NewLine + UserId + " " + ex.Message + " " + ex.StackTrace;
                    var fileName = "GetUserDayStreakV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                }
                finally
                {
                    //objCon.Close();
                    //objCon.Dispose();
                }
            }
            return count;
        }

        public async Task<TaskJobLevelV5> GetUserNewJobsByDayV5(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber) 
        {
            TaskJobLevelV5 userTasksMstV5 = new TaskJobLevelV5(); 
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserAllJobsByDayV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@CurrentDate", CurrentDate);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    //  cmd.Parameters.AddWithValue("@isDatas",1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            await objCon.OpenAsync();
                            // reader = cmd.ex();
                            adapter.Fill(ds);
                            //  ds = reader;
                            DataTable dt1 = ds.Tables[0];
                            DataTable dt2 = ds.Tables[1];
                            int data = Convert.ToInt16(dt2.Rows[0]["isData"]);
                            var day = CurrentDate.DayOfWeek.ToString();
                            //  var day = dateValue.ToString("dddd");
                            if (data == 1)
                            {
                                if (dt1.Rows.Count >= 1)
                                {
                                    userTasksMstV5.UserTaskHistory = new List<UserTaskHisV5>();
                                    var UserTaskHistory = new UserTaskHisV2();
                                    int isTitleblank = Convert.ToInt16(dt2.Rows[0]["isTitleblank"]);
                                    if (isTitleblank == 1)
                                    {
                                        UserTaskHistory.FocusTitle = (dt2.Rows[0]["FocusTitle"]).ToString();
                                        UserTaskHistory.Level1Title = (dt2.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                        UserTaskHistory.Level2Title = (dt2.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                        UserTaskHistory.DisplayTitle = (dt2.Rows[0]["WeekFocusTitle"]).ToString();
                                        UserTaskHistory.DayTitle = (dt2.Rows[0]["DayTitle"]).ToString();
                                    }
                                    else
                                    {
                                        UserTaskHistory.FocusTitle = (dt1.Rows[0]["FocusTitle"]).ToString();
                                        UserTaskHistory.Level1Title = (dt1.Rows[0]["Level1Title"]).ToString().Replace("#day", day);
                                        UserTaskHistory.Level2Title = (dt1.Rows[0]["Level2Title"]).ToString().Replace("#day", day);
                                        UserTaskHistory.DisplayTitle = (dt1.Rows[0]["WeekFocusTitle"]).ToString();
                                        UserTaskHistory.DayTitle = (dt1.Rows[0]["DayTitle"]).ToString();
                                    }

                                    UserTaskHistory.Id = Convert.ToInt64(dt1.Rows[0]["Id"]);
                                    UserTaskHistory.UserId = Convert.ToInt64(dt1.Rows[0]["UserId"]);
                                    UserTaskHistory.UserTasksUpdate = (dt1.Rows[0]["UserTasksUpdate"]).ToString();
                                    UserTaskHistory.TaskDate = Convert.ToDateTime(dt1.Rows[0]["TaskDate"]);

                                    // Parse the JSON string into a JObject
                                    JObject jsonObject = JObject.Parse(UserTaskHistory.UserTasksUpdate);

                                    // Extract the 'Jobs' object
                                    JObject jobsObject = jsonObject["Jobs"] as JObject;

                                    // Extract the 'Level1Jobs' object
                                    JObject level1JobsObject = jsonObject["Level1Jobs"] as JObject;

                                    // Convert the objects back to JSON strings
                                    string jobsJson = JsonConvert.SerializeObject(jobsObject);
                                    string level1JobsJson = JsonConvert.SerializeObject(level1JobsObject);

                                    userTasksMstV5.UserTaskHistory.Add(new UserTaskHisV5
                                    {
                                        Id = UserTaskHistory.Id,
                                        UserId = UserTaskHistory.UserId,
                                        TasksJson = level1JobsJson,
                                        FocusTitle = UserTaskHistory.FocusTitle,
                                        LevelId = 1,
                                        DisplayText = UserTaskHistory.Level1Title,
                                        DisplayTitle = UserTaskHistory.DisplayTitle,
                                        DayTitle = UserTaskHistory.DayTitle,
                                        TaskDate = UserTaskHistory.TaskDate,

                                    });
                                    userTasksMstV5.UserTaskHistory.Add(new UserTaskHisV5
                                    {
                                        Id = UserTaskHistory.Id,
                                        UserId = UserTaskHistory.UserId,
                                        TasksJson = jobsJson,
                                        FocusTitle = UserTaskHistory.FocusTitle,
                                        LevelId = 2,
                                        DisplayText = UserTaskHistory.Level2Title,
                                        DisplayTitle = UserTaskHistory.DisplayTitle,
                                        DayTitle = UserTaskHistory.DayTitle,
                                        TaskDate = UserTaskHistory.TaskDate
                                    });
                                }
                            }
                            else
                            {
                                if (dt1.Rows.Count >= 1)
                                {
                                    for (int i = 0; i < dt1.Rows.Count; i++)
                                    {
                                        var DisplayText = "";
                                        if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 2 || Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 3)
                                        {
                                            DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);

                                        }
                                        if (Convert.ToUInt16(dt1.Rows[i]["LevelId"]) == 1)
                                        {
                                            DisplayText = (dt1.Rows[i]["DisplayText"]).ToString().Replace("#day", day);
                                        }

                                        userTasksMstV5.ListUserTask.Add(new UsertaskV2()
                                        {

                                            Id = Convert.ToInt64(dt1.Rows[i]["Id"]),
                                            UserId = Convert.ToInt64(dt1.Rows[i]["UserId"]),
                                            TasksJson = (dt1.Rows[i]["TasksJson"]).ToString(),
                                            LevelId = Convert.ToUInt16(dt1.Rows[i]["LevelId"]),
                                            DisplayText = DisplayText,
                                            DayTitle = (dt1.Rows[i]["DayTitle"]).ToString(),
                                            DisplayTitle = (dt1.Rows[i]["DisplayTitle"]).ToString(),
                                            FocusTitle = (dt1.Rows[i]["FocusTitle"]).ToString(),
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();             
                string ExceptionString = "Repo : GetUserNewJobsByDayV5" + Environment.NewLine + ex.StackTrace + UserId + IsDay + CurrentDate + WeekNumber;
                var fileName = "GetUserNewJobsByDayV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return userTasksMstV5;

        }

        public async Task<List<TaskDayListV5>> GetUserDayListV5(long UserId) 
        {   
           List<TaskDayListV5> objList = new List<TaskDayListV5>();
            List<TaskDayListV5> list = new List<TaskDayListV5>();

            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetDayListV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        var focusTitle = string.Empty;
                        while (await reader.ReadAsync())
                        {
                            focusTitle = Convert.ToString(reader["FocusTitle"]); // Specify the focus title
                            objList.Add(new TaskDayListV5()
                            {
                                Id = Convert.ToInt64(reader["TaskId"]),

                                //  Day = reader["DayText"].ToString(),

                                Day = reader["WeekNumber"] == DBNull.Value ? reader["DayText"].ToString() : "Week " + reader["WeekNumber"] + " focus day",

                                LevelId = Convert.ToInt16(reader["LevelId"]),
                                IsFocusDay = false, // Initialize to false by default
                                SortBy = Convert.ToInt16(reader["SortBy"]),
                                WeekNumber = reader["WeekNumber"] != DBNull.Value ? Convert.ToInt32(reader["WeekNumber"]) : (int?)null,
                                IsDay = Convert.ToInt32(reader["IsDay"])
                            });
                        }
                        List<Week> weeks = new List<Week>();


                        var focusDays = objList.Where(x => x.Day.Contains("Week")).ToList();

                        foreach (var item in focusDays)
                        {
                            weeks.Add(new Week()
                            {
                                Id = item.Id,
                                Day = item.Day,
                                LevelId = item.LevelId,
                                WeekNumber = item.WeekNumber,
                                IsDay = item.IsDay
                            });
                        }

                        var newList = objList.GroupBy(e => e.SortBy).ToList();
                        list = newList.Select(e => e.First()).ToList();
                        foreach (var item in list)
                        {
                            if (item.Day.Contains("Week"))
                            {
                                item.Day = focusTitle;
                                item.IsFocusDay = true;
                                item.Weeks = weeks;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserDayListV5" + Environment.NewLine;
                var fileName = "GetUserDayListV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
           return list;

        }

        public async Task<List<Usertask>> GetUserDayJobV5(long UserId, int IsDay, int? WeekNumber) 
        {
            List<Usertask> objList = new List<Usertask>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserDayJobV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new Usertask()
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                UserId = Convert.ToInt64(reader["UserId"]),
                                TasksJson = Convert.ToString(reader["TasksJson"]),
                                LevelId = Convert.ToInt16(reader["LevelId"]),
                                DayTitle = Convert.ToString(reader["DayTitle"]),
                                // DisplayText = (reader["DisplayText"].ToString()),
                                DisplayTitle = (reader["DisplayTitle"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserDayJobV5" + Environment.NewLine;
                var fileName = "GetUserDayJobV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Clone();
                objCon.Dispose();
            }
            return objList;

        }


        public async Task<Boolean> SwapUserTaskV5(SwapUserTaskRequest model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_JobDaySwapV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@SwapDayId", model.SwapDayId);
                    cmd.Parameters.AddWithValue("@CurrentDayId", model.CurrentDayId);
                  
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : SwapUserTaskV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "SwapUserTaskV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<List<GetSwapDaysResponse>> GetSwapDaysV5(long UserId)
        {
            List<GetSwapDaysResponse> objList = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetSwapDaysV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    objList = new List<GetSwapDaysResponse>();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new GetSwapDaysResponse()
                            {
                                IsDay = Convert.ToInt32(reader["IsDay"]),
                                DayText = (reader["DayText"]).ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetSwapDaysV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetSwapDaysV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objList;
        }

        public async Task<Boolean> UpdateUserCreatedAt(UpdateUserCreatedAtRequest input)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserCreatedAt", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", input.UserId);
                    cmd.Parameters.AddWithValue("@new_datetime", input.new_datetime);                  

                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Clone();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserCreatedAt" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdateUserCreatedAt - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
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

