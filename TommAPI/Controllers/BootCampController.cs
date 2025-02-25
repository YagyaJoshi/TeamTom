using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using TommBLL.Interface;
using TommDAL.Models;

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BootCampController : ControllerBase
    {
        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        public IConfiguration _configuration { get; }
        private IBootcamp _bootcampservice;
        private IServices _services;
        public BootCampController(
            IConfiguration configuration,
            IServices services,
            IBootcamp bootcampservice,
            IStringLocalizer<AccountController> localizer)
        {
            _configuration = configuration;
            _bootcampservice = bootcampservice;
            _localizer = localizer;
            _services = services;
        }
        #endregion

        /// <summary>
        /// This api is used to Get BootCamp Data of that  User
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>UserBootcampHistory</returns>
        /// <returns>Status</returns>
        [HttpGet]
       [Authorize]
        //[AllowAnonymous]
        [Route("GetBootcampList")]
        public async Task<IActionResult> GetBootcampList(long UserId)
       {
            Response oResponse = new Response();
            try
            {

                UserBootcampHistory oChritmas =await _bootcampservice.GetBootcampList(UserId);
                if (oChritmas != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", oChritmas);
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
                string ExceptionString = "Api : GetBootcampList" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetBootcampList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }


        /// <summary>
        /// This api is used to Update Bootcamp Checkbox Json When User Click on Bootcamp CheckBox
        /// </summary>
        /// <param name="model">UserBootcampHistory</param>
        /// <returns>Status</returns>

        [HttpPut]
        [Authorize]
        //[AllowAnonymous]
        [Route("UpdateBootcamp")]
        public async Task<IActionResult> UpdateBootcamp([FromBody] UserBootcampHistory model)
        {
            Response oResponse = new Response();
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> res = new Dictionary<string, object>();
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
                Boolean IsUpdate =await _bootcampservice.UpdateBootcamp(model);
                if (IsUpdate)
                {
                    UserBootcampHistory oBootcamp = new UserBootcampHistory();
                    oBootcamp.UserId = model.UserId;
                    oBootcamp.TasksJson = model.TasksJson;
                    oBootcamp.BootcampJobs = model.BootcampJobs;
                    res.Add("Success", true);
                    res.Add("Data", oBootcamp);
                    res.Add("Message", _localizer["BootcampUpdateSuccess"].Value);
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
                string ExceptionString = "Api : UpdateBootcamp" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateBootcamp - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// for new app - This api is used to Update Bootcamp Checkbox Json When User Click on Bootcamp CheckBox
        /// </summary>
        /// <param name="model">UserBootcampHistory</param>
        /// <returns>Status</returns>

        [HttpPut]
        [Authorize]       
        [Route("UpdateBootCampV5")]
        public async Task<IActionResult> UpdateBootCampV5([FromBody] UserBootcampHistory model) 
        {
            Response oResponse = new Response();
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> res = new Dictionary<string, object>();
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
                Boolean IsUpdate =await _bootcampservice.UpdateBootcampV5(model); 
                if (IsUpdate)
                {
                    UserBootcampHistory oBootcamp = new UserBootcampHistory();
                    oBootcamp.UserId = model.UserId;
                    oBootcamp.TasksJson = model.TasksJson;
                    oBootcamp.BootcampJobs = model.BootcampJobs;
                    res.Add("Success", true);
                    res.Add("Data", oBootcamp);
                    res.Add("Message", _localizer["BootcampUpdateSuccess"].Value);
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
                string ExceptionString = "Api : UpdateBootcamp" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateBootcamp - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This is api is used to Update User Device Token
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpGet]
        [Authorize]      
        [Route("ResetBootcamp")]
        public async Task<IActionResult> ResetBootcamp(long UserId)
        {
            Response oResponse = new Response();
            try
            {
                Boolean Issave =await _bootcampservice.ResetBootcamp(UserId);
                Dictionary<string, object> data = new Dictionary<string, object>();
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("Success", true);
                res.Add("Message", _localizer["BootcampUpdateSuccess"].Value);
                ObjectResult responsedata = new ObjectResult(res);
                return responsedata;


            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : ResetBootcamp" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(UserId) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        /// <summary>
        /// This is api is used to Update User Boot Camp ExtraDays
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpPut]
        [Authorize]
        [Route("UpdateBootCampExtraDays")]
        public async Task<IActionResult> UpdateBootCampExtraDays(UserBootcampHistory model)
        {
            Response oResponse = new Response();
            try
            {
                Boolean Issave =await _bootcampservice.UpdateBootCampExtraDays(model);
                Dictionary<string, object> data = new Dictionary<string, object>();
                Dictionary<string, object> res = new Dictionary<string, object>();
                if (Issave == true)
                {
                    UserBootcampHistory oBootcamp = new UserBootcampHistory();
                    oBootcamp.Id = model.Id;
                    oBootcamp.TasksJson = model.TasksJson;
                    oBootcamp.BootcampJobs = model.BootcampJobs;
                    res.Add("Success", true);
                    res.Add("Data", oBootcamp);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }else
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

                string ExceptionString = "Api : UpdateBootCampExtraDays" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(model.Id) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get BootCamp Data of that  User
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>UserBootcampHistory</returns>
        /// <returns>Status</returns>

        [HttpGet]
        [Authorize]
        //[AllowAnonymous]
        [Route("GetBootcampListV5")]
        public async Task<IActionResult> GetBootcampListV5(long UserId)
        {
            Response oResponse = new Response();
            try
            {

                UserBootcampHistory oChritmas =await _bootcampservice.GetBootcampListV5(UserId);
                if (oChritmas != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Success", true);
                    res.Add("Data", oChritmas);
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
                string ExceptionString = "Api : GetBootcampListV5" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetBootcampListV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }
    }
}