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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {

        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        public IConfiguration _configuration { get; }
        private INotes _notesservice;
        private IServices _services;
        public NotesController(
            IConfiguration configuration, INotes notesservice,
            IStringLocalizer<AccountController> localizer, IServices services)
        {
            _configuration = configuration;
            _notesservice = notesservice;
            _localizer = localizer;
            _services = services;
        }

        #endregion

        /// <summary>
        /// This api is used to Get User Notes Using UserId
        /// </summary>
        /// <param name="long">UserId</param>
        /// <returns>UserNotes</returns>
        /// <returns>Status</returns>

        [HttpGet]
        [Authorize]
        [Route("GetUserNotes")]
        public async Task<IActionResult> GetUserNotes(long UserId)
        {
            Response oResponse = new Response();
            try
            {
                UserNotes oUserNotes =await _notesservice.GetUserNotes(UserId);
                if (oUserNotes != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();


                    res.Add("Success", true);
                    res.Add("Data", oUserNotes);
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

                string ExceptionString = "Api : GetUserNotes" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserNotes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Save/Updae User Notes Using UserId
        /// </summary>
        /// <param name="model">UserNotes</param>
        /// <returns>Status</returns>

        [HttpPost]
        [Authorize]
        [Route("SaveNotes")]
        public async Task<IActionResult> SaveNotes([FromBody] UserNotes omodel)
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

                Boolean Issave =await _notesservice.SaveNotes(omodel);
                if (Issave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["NoteSuccess"].Value;
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

                string ExceptionString = "Api : SaveNotes" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "SaveNotes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }
    }
}