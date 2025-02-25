using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        private IJobs _jobsservice;
        private IServices _services;
        private readonly IStringLocalizer<AccountController> _localizer;
        public JobsController(
            IConfiguration configuration,
            IStringLocalizer<AccountController> localizer,
            IServices services,
            IJobs jobsservice)
        {
            _configuration = configuration;
            _jobsservice = jobsservice;
            _services = services;
            _localizer = localizer;
        }
        #endregion

        /// <summary>
        /// This api is used to Get User Jobs By Day
        /// </summary>
        ///  <param name="long">UserId</param>
        /// <param name="DateTime">CurrentDate</param>
        /// /// <param name="int">WeekNumber,IsDay</param>
        /// <returns>TaskJobLevel</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserJobsByDay")]
        public async Task<IActionResult> GetUserJobsByDay(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            Response oResponse = new Response();
            try
            {
                TaskJobLevel oTasks = null;
                TaskJobLevel oTask =await _jobsservice.GetUserJobsByDay(UserId, IsDay, CurrentDate, WeekNumber);
                //if (CurrentDate.Date == DateTime.Now.Date)
                //{
                oTasks =await _jobsservice.GetNextDayJob(UserId, IsDay, CurrentDate, WeekNumber);
                // }
                if (oTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    res.Add("TommorowData", oTasks);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserJobsByDay" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserJobsByDay - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Get All Day
        /// </summary>
        /// <returns>List<TaskDayList></returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserDayList")]
        public async Task<IActionResult> GetUserDayList()
        {
            Response oResponse = new Response();
            try
            {

                List<TaskDayList> oTask =await _jobsservice.GetUserDayList();
                if (oTask != null && oTask.Count >= 1)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayList" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Get User Job on Daily Basis for customized
        /// </summary>
        /// <param name="long">UserId,TaskId</param>
        ///<returns>Usertask</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserDayJob")]
        public async Task<IActionResult> GetUserDayJob(long UserId, int TaskId)
        {
            Response oResponse = new Response();
            try
            {

                Usertask oUserTask =await _jobsservice.GetUserDayJob(UserId, TaskId);
                if (oUserTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", oUserTask);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayJob" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Update Task Job
        /// </summary>
        /// <param name="model">Usertask</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPut]
        [Route("UpdateTaskJob")]
        public async Task<IActionResult> UpdateTaskJob(Usertask oUsertask)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateTaskJob(oUsertask);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdatedayTaskJob"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateTaskJob" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateTaskJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Update Task JobV5
        /// </summary>
        /// <param name="oUsertask">UsertaskV5</param>
        /// <returns>Status</returns>

        [Authorize]
        [HttpPut]
        [Route("UpdateTaskJobV5")]
        public async Task<IActionResult> UpdateTaskJobV5(UsertaskV5 oUsertask)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await  _jobsservice.UpdateTaskJobV5(oUsertask);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdatedayTaskJob"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateTaskJobV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateTaskJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Bulk Update Task JobV5
        /// </summary>
        /// <param name="oUsertask">UsertaskV5</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPut]
        [Route("BulkUpdateTaskJobV5")]
        public async Task<IActionResult> BulkUpdateTaskJobV5(UsertaskV5 oUsertask)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.BulkUpdateTaskJobV5(oUsertask);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdatedayTaskJob"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : BulkUpdateTaskJobV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "BulkUpdateTaskJobV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Reset Customized Day Job
        /// </summary>
        /// <param name="long">UserId,TaskId</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("RestoreDayTasks")]
        public async Task<IActionResult> RestoreDayTasks(long UserId, int TaskId)
        {
            Response oResponse = new Response();
            try
            {

                Boolean IstaskSave =await _jobsservice.RestoreDayTasks(UserId, TaskId);
                if (IstaskSave)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Message", _localizer["RestoreDay"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : Login" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Login - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Update User Day Job
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateUserDayJobs")]
        public async Task<IActionResult> UpdateUserDayJobs([FromBody] UserTaskHistory model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobs(model);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateTaskJob"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserDayJobs" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserDayJobs - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Refresh Job and Delete the job in updattehistory job to initial job in present task master
        /// </summary>
        /// <param name="long">UserTaskHistoryId</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("DeleteUserHistoryJobs")]
        public async Task<IActionResult> DeleteUserHistoryJobs(long UserTaskHistoryId)
        {
            Response oResponse = new Response();
            try
            {
                Boolean IstaskSave =await _jobsservice.DeleteUserHistoryDayJobs(UserTaskHistoryId);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["Refresdaytask"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : DeleteUserHistoryJobs" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "DeleteUserHistoryJobs - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Get tommorow Job
        /// </summary>
        /// <param name="long">UserId</param>
        /// /// <param name="int">IsDay,WeekNumber</param>
        /// /// <param name="DateTime">CurrentDate</param>
        ///  /// <returns>TaskJobLevel</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetNextDayJob")]
        public async Task<IActionResult> GetNextDayJob(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            Response oResponse = new Response();
            try
            {
                TaskJobLevel oUserTask =await _jobsservice.GetNextDayJob(UserId, IsDay, CurrentDate, WeekNumber);
                if (oUserTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", oUserTask);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetNextDayJob" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetNextDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Roll Over Today Job to tommorow
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateRollOverDayJobs")]
        public async Task<IActionResult> UpdateRollOverDayJobs([FromBody] UserTaskHistory model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobs(model);
                if (IstaskSave)
                {

                    Boolean IsRollOvertaskSave =await _jobsservice.UpdateUserRoolOverDay(model);
                    if (IsRollOvertaskSave)
                    {
                        oResponse.Success = true;
                        oResponse.Message = _localizer["UpdateRolloverday"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Failed"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateRollOverDayJobs" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateRollOverDayJobs - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Get Day Title
        /// </summary>
        /// <param name="int">WeekNumber,IsDay</param>
        /// /// <returns>Displaytitle</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetDayTitle")]
        public async Task<IActionResult> GetDayTitle(int WeekNumber, int IsDay, long UserId)
        {
            Response oResponse = new Response();
            try
            {

                string Displaytitle =await _jobsservice.GetDayTitle(WeekNumber, IsDay, UserId);
                if (Displaytitle != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", Displaytitle);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayJob" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        /// <summary>
        /// This api is used to Swap the Customized Day
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("SaveSwapDayJob")]
        public async Task<IActionResult> SaveSwapDayJob(long UserId, long CurrentTaskId, long SwapTaskId)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }

                Boolean IsJobSwapSave =await _jobsservice.SaveSwapDayJob(UserId, CurrentTaskId, SwapTaskId);
                if (IsJobSwapSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateSwapJobday"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }

            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : SaveSwapDayJob" + Environment.NewLine;
                ExceptionString += "Request : " + (UserId, CurrentTaskId, SwapTaskId) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine + ex.StackTrace;
                var fileName = "SaveSwapDayJob - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        /// <summary>
        /// This api is used to Get User Jobs By Day
        /// </summary>
        ///  <param name="long">UserId</param>
        /// <param name="DateTime">CurrentDate</param>
        /// /// <param name="int">WeekNumber,IsDay</param>
        /// <returns>TaskJobLevel</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserNewJobsByDayV2")]
        public async Task<IActionResult> GetUserNewJobsByDayV2(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            Response oResponse = new Response();
            try
            {
                TaskJobLevelV2 oTask =await _jobsservice.GetUserNewJobsByDayV2(UserId, IsDay, CurrentDate, WeekNumber);
                TaskJobLevelV2 oTasks =await _jobsservice.GetNextDayJobV2(UserId, IsDay, CurrentDate, WeekNumber);
                if (oTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    res.Add("TommorowData", oTasks);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserNewJobsByDayV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserNewJobsByDayV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

       
        [Authorize]
        [HttpGet]
        [Route("GetUserNewJobsByDayV3")]
        public async Task<IActionResult> GetUserNewJobsByDayV3(long UserId, int IsDay, DateTime CurrentDate,DateTime FutureDate, int WeekNumber)
        {
            Response oResponse = new Response();
            try
            {
                TaskJobLevelV2 oTask =await _jobsservice.GetUserNewJobsByDayV2(UserId, IsDay, CurrentDate, WeekNumber);
                TaskJobLevelV2 oTasks =await _jobsservice.GetNextDayJobV3(UserId, IsDay, CurrentDate,FutureDate, WeekNumber);
                if (oTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    res.Add("FutureDayData", oTasks);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserNewJobsByDayV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserNewJobsByDayV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This api is used to Update User Day Job
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateUserDayJobsV2")]
        public async Task<IActionResult> UpdateUserDayJobsV2([FromBody] UserUpdateTaskJobV2 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobsV2(model);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateTaskJob"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserDayJobsV2" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This api is used to Update User Day Job
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        //[AllowAnonymous]
        [HttpPost]
        [Route("UpdateUserDayJobsV3")]
        public async Task<IActionResult> UpdateUserDayJobsV3([FromBody] UserUpdateTaskJobV3 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                var data =await _jobsservice.UpdateUserDayJobsV3(model);
                Boolean IstaskSave = data.Item1;
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateTaskJob"].Value;
                    oResponse.data = data.Item2;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserDayJobsV3" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV3 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Update User Day Job
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        //[AllowAnonymous]
        [HttpPost]
        [Route("UpdateUserDayJobsV4")]
        public async Task<IActionResult> UpdateUserDayJobsV4([FromBody] UserUpdateTaskJobV3 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                var data =await _jobsservice.UpdateUserDayJobsV4(model);
                Boolean IstaskSave = data.Item1;
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateTaskJob"].Value;
                    oResponse.data = data.Item2;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserDayJobsV4" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV4 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        /// <summary>
        /// This api is used to Roll Over Today Job to tommorow
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateRollOverDayJobsV2")]
        public async Task<IActionResult> UpdateRollOverDayJobsV2([FromBody] UserUpdateTaskJobV2 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobsV2(model);
                if (IstaskSave)
                {

                    Boolean IsRollOvertaskSave =await _jobsservice.UpdateUserRoolOverDayV2(model);
                    if (IsRollOvertaskSave)
                    {
                        oResponse.Success = true;
                        oResponse.Message = _localizer["UpdateRolloverday"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Failed"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateRollOverDayJobsV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateRollOverDayJobsV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Roll Over Today Job to tommorow
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateRollOverDayJobsV3")]
        public async Task<IActionResult> UpdateRollOverDayJobsV3([FromBody] UserUpdateTaskJobV2 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobsV3(model);
                if (IstaskSave)
                {

                    Boolean IsRollOvertaskSave =await _jobsservice.UpdateUserRoolOverDayV3(model);
                    if (IsRollOvertaskSave)
                    {
                        oResponse.Success = true;
                        oResponse.Message = _localizer["UpdateRolloverday"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Failed"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateRollOverDayJobsV3" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateRollOverDayJobsV3 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        [Authorize]
        [HttpPost]
        [Route("UpdateRollOverDayJobsV4")]
        public async Task<IActionResult> UpdateRollOverDayJobsV4([FromBody] UserUpdateTaskJobV4 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobsV4(model);
                if (IstaskSave)
                {

                    Boolean IsRollOvertaskSave =await _jobsservice.UpdateUserRoolOverDayV4(model);
                    if (IsRollOvertaskSave)
                    {
                        oResponse.Success = true;
                        oResponse.Message = _localizer["UpdateRolloverday"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Failed"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateRollOverDayJobsV4" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateRollOverDayJobsV4 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }




        /// <summary>
        /// This api is used to Roll Over Today Job to tommorow
        /// </summary>
        /// <param name="model">UserTaskHistory</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateWeekFocusV2")]
        public async Task<IActionResult> UpdateWeekFocusV2([FromBody] UserWeekFocusJobV2 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserWeekFocusV2(model);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["FocusDaySuccess"].Value + model.SetDayL2;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateWeekFocusV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateWeekFocusV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This api is used to Get Day Title
        /// </summary>
        /// <param name="int">WeekNumber,IsDay</param>
        /// /// <returns>Displaytitle</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetDayTitleV2")]
        public async Task<IActionResult> GetDayTitleV2(int WeekNumber, int IsDay, long UserId)
        {
            Response oResponse = new Response();
            try
            {

                string Displaytitle =await _jobsservice.GetDayTitleV2(WeekNumber, IsDay, UserId);
                if (Displaytitle != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", Displaytitle);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetDayTitleV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetDayTitleV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This api is used to Get All Day
        /// </summary>
        /// <returns>List<TaskDayList></returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserDayListV2")]
        public async Task<IActionResult> GetUserDayListV2(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                List<TaskDayList> oTask =await _jobsservice.GetUserDayListV2(UserId);
                if(oTask != null && oTask.Count >= 1)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayListV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayListV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This api is used to Reset Customized Day Job
        /// </summary>
        /// <param name="long">UserId,TaskId</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("RestoreDayTasksV2")]
        public async Task<IActionResult> RestoreDayTasksV2(long UserId, int TaskId)
        {
            Response oResponse = new Response();
            try
            {

                Boolean IstaskSave =await _jobsservice.RestoreDayTasksV2(UserId, TaskId);
                if (IstaskSave)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Message", _localizer["RestoreDay"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : RestoreDayTasksV2" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "RestoreDayTasksV2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Update User Day JobV5
        /// </summary>
        /// <param name="model">UserUpdateTaskJobV3</param>
        /// <returns>Status</returns>

        [Authorize]
        [HttpPost]
        [Route("UpdateUserDayJobsV5")]
        public async Task<IActionResult> UpdateUserDayJobsV5([FromBody] UserUpdateTaskJobV3 model)
        {
            Response oResponse = new Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave =await _jobsservice.UpdateUserDayJobsV5(model);                 
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateTaskJob"].Value;          
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserDayJobsV5" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserDayJobsV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get User Day Streak V5
        /// </summary>
        /// <param name="int">UserId</param>
        /// <param name="DateTime">TaskDate</param>
        /// <returns>Status, count</returns>

        [Authorize]
        [HttpGet] 
        [Route("GetUserDayStreakV5")]
        public async Task<IActionResult> GetUserDayStreakV5(string UserId, DateTime TaskDate)
        {
            Response oResponse = new Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                var count =await _jobsservice.GetUserDayStreakV5(UserId, TaskDate);
                if (count !=0)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["Success"].Value;
                    oResponse.data = new {count = count};  
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    //oResponse.Message = _localizer["Failed"].Value;
                    oResponse.data = new { count = count };
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayStreakV5" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayStreakV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get User New Jobs By DayV5
        /// </summary>
        ///  <param name="long">UserId</param>
        /// <param name="DateTime">CurrentDate</param>
        /// /// <param name="int">WeekNumber,IsDay</param>
        /// <returns>TaskJobLevelV5</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserNewJobsByDayV5")]
        public async Task<IActionResult> GetUserNewJobsByDayV5(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber)
        {
            Response oResponse = new Response();
            try
            {
                TaskJobLevelV5 oTask =await _jobsservice.GetUserNewJobsByDayV5(UserId, IsDay, CurrentDate, WeekNumber);
                //TaskJobLevelV2 oTasks = _jobsservice.GetNextDayJobV5(UserId, IsDay, CurrentDate, WeekNumber);
                if (oTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    //res.Add("TommorowData", oTasks);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserNewJobsByDayV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserNewJobsByDayV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get User Day List V5
        /// </summary>
        ///  <param name="long">UserId</param>
        /// <returns>List<TaskDayListV5></returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserDayListV5")]
        public async Task<IActionResult> GetUserDayListV5(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                List<TaskDayListV5> oTask =await _jobsservice.GetUserDayListV5(UserId); 
                if (oTask != null && oTask.Count >= 1)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oTask);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayListV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayListV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get User JobV5 on Daily Basis for customized
        /// </summary>
        /// <param name="long">UserId,IsDay, WeekNumber</param>            
        ///<returns>Usertask</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserDayJobV5")]
        public async Task<IActionResult> GetUserDayJobV5(long UserId, int IsDay, int? WeekNumber)  
        {
            Response oResponse = new Response();
            try
            {

                List<Usertask> oUserTask =await _jobsservice.GetUserDayJobV5(UserId, IsDay, WeekNumber); 
                if (oUserTask != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", oUserTask);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserDayJobV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserDayJobV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Update Week FocusV5
        /// </summary>
        /// <param name="model">UserWeekFocusJobV2</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateWeekFocusV5")]
        public async Task<IActionResult> UpdateWeekFocusV5([FromBody] UserWeekFocusJobV2 model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                } 
                Boolean IstaskSave =await _jobsservice.UpdateUserWeekFocusV5(model);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["FocusDaySuccess"].Value + model.SetDayL2;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateWeekFocusV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateWeekFocusV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Restore all Day Tasks V5
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("RestoreDayTasksV5")]
        public async Task<IActionResult> RestoreDayTasksV5(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                Boolean IstaskSave =await _jobsservice.RestoreDayTasksV5(UserId);
                if (IstaskSave)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Message", _localizer["RestoreDay"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : RestoreDayTasksV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "RestoreDayTasksV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        [Authorize]
        [HttpPost]
        [Route("SwapUserTaskV5")]
        public async Task<IActionResult> SwapUserTaskV5([FromBody] SwapUserTaskRequest model)
        {
            Response oResponse = new Response();
            try
            {

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave = await _jobsservice.SwapUserTaskV5(model);
                if (IstaskSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateSwapJobday"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : SwapUserTaskV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "SwapUserTaskV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        [Authorize]
        [HttpGet]
        [Route("GetSwapDaysV5")]
        public async Task<IActionResult> GetSwapDaysV5(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                List<GetSwapDaysResponse> oCategory = await _jobsservice.GetSwapDaysV5(UserId);
                if (oCategory != null && oCategory.Count > 0)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oCategory);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecord"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetSwapDaysV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetSwapDaysV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }


        [Authorize]
        [HttpPut]
        [Route("UpdateUserCreatedAt")]
        public async Task<IActionResult> UpdateUserCreatedAt(UpdateUserCreatedAtRequest input)
        {
            Response oResponse = new Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    oResponse.Success = false;
                    oResponse.Message = messages;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;

                }
                Boolean IstaskSave = await _jobsservice.UpdateUserCreatedAt(input);
                if (IstaskSave)
                {

                    oResponse.Success = true;
                    oResponse.Message = _localizer["Success"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["Failed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserCreatedAt" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserCreatedAt - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }
    }



}