using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChristmasController : ControllerBase
    {

        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        public IConfiguration _configuration { get; }
        private IChritmas _chritmasservice;
        private IServices _services;
        public ChristmasController(
            IConfiguration configuration,
            IServices services,
            IChritmas chritmasservice,
            IStringLocalizer<AccountController> localizer)
        {
            _configuration = configuration;
            _chritmasservice = chritmasservice;
            _localizer = localizer;
            _services = services;
        }
        #endregion


        /// <summary>
        /// This api is used to Get Chritmas Data of that  User
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>UserChristmasHistory</returns>
        /// <returns>Status</returns>
        [HttpGet]
        [Authorize]
        [Route("GetChritmasList")]
        public async Task<IActionResult> GetChritmasList(long UserId)
        {
            Response oResponse = new Response();
            try
            {
                UserChristmasHistory oChritmas =await _chritmasservice.GetChritmasList(UserId);
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
                string ExceptionString = "Api : GetChritmasList" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetChritmasList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }

        /// <summary>
        /// This api is used to Update Chritmas Checkbox Json When User Click on Chritmas CheckBox
        /// </summary>
        /// <param name="model">UserChristmasHistory</param>
        /// <returns>Status</returns>

        [HttpPut]
        [Authorize]
        [Route("UpdateChritmas")]
        public async Task<IActionResult> UpdateChritmas([FromBody] UserChristmasHistory model)
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
                Boolean IsUpdate =await _chritmasservice.UpdateChritmas(model);
                if (IsUpdate)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["ChritmasUpdateSuccess"].Value;
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
                string ExceptionString = "Api : UpdateChritmas" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateChritmas - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }
        /// <summary>
        /// for new app - This api is used to Get Chritmas Data of that  User
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>UserChristmasHistory</returns>
        /// <returns>Status</returns>
        [HttpGet]
        [Authorize]
        [Route("GetChritmasListV5")]
        public async Task<IActionResult> GetChritmasListV5(long UserId)
        {
            Response oResponse = new Response();
            try
            {
                UserChristmasHistory oChritmas =await _chritmasservice.GetChritmasListV5(UserId); 
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
                string ExceptionString = "Api : GetChritmasListV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetChritmasListV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }


        /// <summary>
        /// for new app - This api is used to Update Chritmas Checkbox Json When User Click on Chritmas CheckBox
        /// </summary>
        /// <param name="model">UserChristmasHistory</param>
        /// <returns>Status</returns>
        /// 
        [HttpPut]
        [Authorize]
        [Route("UpdateChritmasV5")]
        public async Task<IActionResult> UpdateChritmasV5([FromBody] UserChristmasHistory model) 
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
                Boolean IsUpdate =await _chritmasservice.UpdateChritmasV5(model);
                if (IsUpdate)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["ChritmasUpdateSuccess"].Value;
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
                string ExceptionString = "Api : UpdateChritmasV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateChritmasV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// for new app - This api is used to add chritmas new task job json when user add week's new task.
        /// </summary>
        /// <param name="model">AddChristmasTask</param>
        /// <returns>Status</returns>

        [HttpPut]
        [AllowAnonymous]
        [Route("AddChritmasV5")]
        public async Task<IActionResult> AddChritmasV5([FromBody] AddChristmasTask model)  
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
                Boolean IsUpdate =await _chritmasservice.AddChritmasTasksV5(model);
                if (IsUpdate)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["ChritmasUpdateSuccess"].Value;
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
                string ExceptionString = "Api : AddChritmasV5" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "AddChritmasV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }

    }
}