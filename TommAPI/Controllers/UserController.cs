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
    public class UserController : ControllerBase
    {

        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        public IConfiguration _configuration { get; }
        private IUser _usersservice;
        private IHoliday _holidayService;
        private IServices _services;
        public UserController(
            IConfiguration configuration,
            IUser usersservice,IHoliday holidayService, IStringLocalizer<AccountController> localizer, IServices services)
        {
            _configuration = configuration;
            _holidayService = holidayService;
            _usersservice = usersservice;
            _localizer = localizer;
            _services = services;
        }

        #endregion


        /// <summary>
        /// This is api is Get User Profile
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>User model</returns>
        /// <returns>Status</returns>
        [HttpGet]
        [Authorize]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                User oUser = await _usersservice.GetUserProfile(UserId);
                if (oUser != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oUser);
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

                string ExceptionString = "Api : GetUserProfile" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserProfile - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }

        /// <summary>
        /// This is api is used to Update User Profile
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpPut]
        [Authorize]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User omodel)
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
                omodel.CreatedAt = DateTime.Now;

                Boolean Issave = await _usersservice.UpdateUser(omodel);
                if (Issave)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    //oResponse.Success = true;
                    //oResponse.Message = _localizer["UserUpdateSuccess"].Value;
                    res.Add("Success", true);
                    res.Add("Data", omodel);
                    res.Add("Message", _localizer["UserUpdateSuccess"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["EmailAlready"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUser" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This is api is used to Update User Profile
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("UpdateMigrateUser")]
        public async Task<IActionResult> UpdateMigrateUser([FromBody] Migrate omodel)
        {
            Response oResponse = new Response();
            try
            {
                Boolean Issave = false;
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
                //if (omodel.IsMigrate)
                //{
                //    Issave = _usersservice.MigrateUserData(omodel);
                //}
                //else
                //{
                Issave = await _usersservice.UpdateMigrateUser(omodel);
                //  }
                if (Issave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["MigrateDataSuccess"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["MigrateErrorData"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUser" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
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
        [Route("GetAllMigrateProfile")]
        public async Task<IActionResult> GetAllMigrateProfile(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                List<MigrateProfile> MigrateProfile = await _usersservice.GetAllMigrateProfile(UserId);
                if (MigrateProfile != null && MigrateProfile.Count >= 1)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", MigrateProfile);
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
        /// This is api is Get User User Tasks History
        /// </summary>
        /// <param name="long">UserId</param>
        ///  <param name="long">Year</param>
        ///   <param name="Int32">Month</param>
        /// <returns>UserUpdateTaskJobV3</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserTasksHistory")]
        public async Task<IActionResult> GetUserTasksHistory(long UserId, long Year, Int32 Month)
        {
            Response oResponse = new Response();
            try
            {
                List<UserUpdateTaskHistory> oUser = await _usersservice.GetUserTasksHistory(UserId, Year, Month);
                if (oUser != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", oUser);
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

                string ExceptionString = "Api : GetUserTasksHistory" + Environment.NewLine;

                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserTasksHistory - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }
        

        [HttpPost]
        [Authorize]
        [Route("SetHolidayStatus")]
        public async Task<IActionResult> SetHolidayStatus([FromBody] HolidayRequestViewModel omodel)
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


                /*this api is getting called in the background with future date of 1 year from now so to stop it being called below condition will work*/
                if (DateTime.Now.AddDays(2) < omodel.CurrentDate)
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["FutureDateHolidayCantbeMarked"].Value;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }

                oResponse.data = await _holidayService.SetHolidayStatus(omodel);

                if (oResponse.data != null && string.IsNullOrEmpty((oResponse.data as SetHolidayResponseViewModel).ErrorMessage))
                {
                    oResponse.Success = true;
                    oResponse.Message = omodel.HasMarkedHoliday ? _localizer["HolidayTurnedOnMsg"].Value: _localizer["HolidayTurnedOffMsg"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = oResponse.data != null ? (oResponse.data as SetHolidayResponseViewModel).ErrorMessage : _localizer["MigrateErrorData"].Value;
                    oResponse.data = null;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;

                }

            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : SetHolidayStatus" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "SetHolidayStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetHolidayDetails")]
        public async Task<IActionResult> GetHolidayDetails(long UserId)
        {
            Response oResponse = new Response();
            try
            {
               
                HolidayDetailsResponseViewModel response = await _holidayService.GetHolidayDetails(UserId);
                if (response != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", response);
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

                string ExceptionString = "Api : GetHolidayDetails" + Environment.NewLine;

                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetHolidayDetails - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }
        
        
        [HttpPost]
        [Authorize]
        [Route("DeleteUserHoliday")]
        public async Task<IActionResult> DeleteUserHoliday([FromBody] DeleteHolidayViewModel omodel)
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

                 bool Isdeleted= await _holidayService.DeleteUserHoliday(omodel);

                if (Isdeleted)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["MigrateDataSuccess"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["MigrateErrorData"].Value;
                    oResponse.data = null;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;

                }

            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : DeleteUserHoliday" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "DeleteUserHoliday - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }
        
        [Authorize]
        [HttpGet]
        [Route("GetHolidayDatesByMonth")]
        public async Task<IActionResult> GetHolidayDatesByMonth(long UserId, byte Month, short Year)
        {
            Response oResponse = new Response();
            try
            {
                List<HolidayDetailsResponseViewModel> response = await _holidayService.GetHolidayDatesByMonth(UserId,Month,Year);
                if (response.Count>0)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", response);
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

                string ExceptionString = "Api : GetHolidayDatesByMonth" + Environment.NewLine;

                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetHolidayDatesByMonth - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }


        [Authorize]
        [HttpGet]
        [Route("GetUserHolidayList")]
        public async Task<IActionResult> GetUserHolidayList(long UserId, int PageNo=1, int PageSize=10)
        {
            Response oResponse = new Response();
            try
            {
                HolidayListResponse response = await  _holidayService.GetUserHolidayList(UserId, PageNo, PageSize);
                if (response!=null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", response);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecordFound"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUserHolidayList" + Environment.NewLine;

                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserHolidayList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }

        /// <summary>
        /// For new app - This api is used to add user's childrens
        /// </summary>
        /// <param name="omodel">Childrens</param>
        /// <returns>status</returns>

        [Authorize]
        [HttpPost]
        [Route("AddUserChildrensV5")]
        public async Task<IActionResult> AddUserChildrensV5(Childrens omodel) 
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

                Boolean Issave = await _usersservice.AddUserChildrensV5(omodel);
                if (Issave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["SaveChildrens"].Value; 
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecordFound"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : AddChildrensV5" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "AddChildrensV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This is api is used to Update User Profile
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpPut]
        [Authorize]
        [Route("UpdateUserV5")]
        public async Task<IActionResult> UpdateUserV5([FromBody] User omodel) 
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
                omodel.CreatedAt = DateTime.Now;

                Boolean Issave = await _usersservice.UpdateUserV5(omodel); 
                if (Issave)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    //oResponse.Success = true;
                    //oResponse.Message = _localizer["UserUpdateSuccess"].Value;
                    res.Add("Success", true);
                    res.Add("Data", omodel);
                    res.Add("Message", _localizer["UserUpdateSuccess"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["EmailAlready"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : UpdateUserV5" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This is api is used to get users for notification
        /// </summary>
        /// <returns>UserDetails</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetUsersForNotification")]
        public IActionResult GetUsersForNotification()
        {
            Response oResponse = new Response();
            try
            {
                var response = TommAPI.Providers.PushNotification.GetUserDetailsforNotificationV2(_configuration);
                if (response != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", response);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecordFound"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUsersForNotification" + Environment.NewLine;

                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetUsersForNotification - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SavePushNotification")]
        public IActionResult SavePushNotification(SavePushNotificationRequest request)
        {
            Response oResponse = new Response();
            try
            {

                var Issave = TommAPI.Providers.PushNotification.SavePushNotification(request.Result, _configuration);
                if (Issave == 1)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["success"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NoRecordFound"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : SavePushNotification -" + Environment.NewLine;

                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "SavePushNotification - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }
    }
}