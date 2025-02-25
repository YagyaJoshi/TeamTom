using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.Models;

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSubscriptionController : ControllerBase
    {
        HttpClient client = new HttpClient();
        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        public IConfiguration _configuration { get; }
        private IUserSubscription _subscriptionService;
        private IServices _services;
        private IHostingEnvironment _env;
        public UserSubscriptionController( 
            IConfiguration configuration,
            IUserSubscription subscriptionService, IStringLocalizer<AccountController> localizer, IServices services, IHostingEnvironment env) 
        {
            _configuration = configuration;
            _subscriptionService = subscriptionService;
            _localizer = localizer;
            _services = services;
            _env = env;
        }
        #endregion

        /// <summary>
        /// For new app - This api is used to Add User Subscription
        /// </summary>
        /// <param name="userSubscription">UserSubscription</param>
        /// <returns>Status</returns>

        [HttpPost]
        [AllowAnonymous]
        [Route("AddUserSubscription")]
        public async Task<IActionResult> AddUserSubscription(UserSubscription userSubscription)
        {
            Response oResponse = new Response();
            try
            {
                var subscription =await _subscriptionService.AddUserSubscription(userSubscription);
                if (subscription == 0 || subscription == 1)
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
                string ExceptionString = "Api : AddUserSubscription" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "AddUserSubscription - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get User Subsciption for apple and google
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>Status</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetUserSubscription")]
        public async Task<IActionResult> GetUserSubscription(int userId) 
        {
            Response oResponse = new Response();
            try
            {
                var subscription =await _subscriptionService.GetUserSubscription(userId);               
                if (subscription.Id> 0)
                {
                    (bool, bool) data = (false, false);
                    if (subscription.platform == "android")
                        data = await _subscriptionService.GoogleSubscriptionCancellation(subscription.packageName, subscription.subscriptionId, subscription.token);
                    else
                    {
                        data = await _subscriptionService.AppleSubscriptionCancellation(subscription.packageName, subscription.subscriptionId, subscription.token);
                    }

                    oResponse.Success = true;
                    oResponse.Message = _localizer["Success"].Value;
                    oResponse.data = new { Cancelled = data.Item1 , Expired = data.Item2};
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NotFound"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;
                string ExceptionString = "Api : GetUserSubscription" + Environment.NewLine;
                ExceptionString += "Exception : " + userId + " " + ex.Message + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetUserSubscription - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to cancel user subscription
        /// </summary>
        /// <param name="userSubscription">UserSubscription</param>
        /// <returns>Status</returns>
        /// 
        [HttpDelete]
        [AllowAnonymous]
        [Route("CancelUserSubscription")]
        public async Task<IActionResult> CancelUserSubscription(UserSubscription userSubscription) 
        {
            Response oResponse = new Response();
            try
            {
                var subscription =await _subscriptionService.CancelUserSubscription(userSubscription); 
                if (subscription)
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
                string ExceptionString = "Api : CancelUserSubscription" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "CancelUserSubscription - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }            

    }
}
