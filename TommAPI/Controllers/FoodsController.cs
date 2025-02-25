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
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {

        #region Dependency injection  

        public IConfiguration _configuration { get; }
        private readonly IStringLocalizer<AccountController> _localizer;
        private IFoods _foodsservice;
        private IServices _services;
        public FoodsController(
            IConfiguration configuration,
            IServices services,
            IFoods foodsservice,
             IStringLocalizer<AccountController> localizer)
        {
            _configuration = configuration;
            _foodsservice = foodsservice;
            _localizer = localizer;
            _services = services;
        }
        #endregion


        /// <summary>
        /// This api is used to Get Food Category List
        /// </summary>
        /// <param name="model"></param>
        /// <returns>List<CategoryMst></returns>
        /// <returns>Status</returns>

        [HttpGet]
        [Authorize]
        [Route("GetCategoryList")]
        public async Task<IActionResult> GetCategoryList()
        {
            Response oResponse = new Response();
            try
            {

                List<CategoryMst> oCategory =await _foodsservice.CategoryList();
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

                string ExceptionString = "Api : GetCategoryList" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetCategoryList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }

        /// <summary>
        /// This api is used to Get Food Category Url of that Category
        /// </summary>
        /// <param name="int">CateGoryId</param>
        /// <returns>List<FoodView></returns>
        /// <returns>Status</returns>
        [HttpGet]
        [Authorize]
        [Route("GetFoodUrlList")]
        public async Task<IActionResult> GetFoodUrlList(int CateGoryId)
        {
            Response oResponse = new Response();
            try
            {
                List<FoodView> oCategory =await _foodsservice.GetFoodData(CateGoryId);
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

                string ExceptionString = "Api : GetFoodUrlList" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetFoodUrlList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }

        }

        /// <summary>
        /// This api is Called from Php Api and used to Save or update Food Data
        /// </summary>
        /// <param name="model">FoodCategories</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("SaveFood")]
        public async Task<IActionResult> SaveFood([FromBody] FoodCategories model)
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
                Boolean IstaskSave =await _foodsservice.UpdateFood(model);
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

                string ExceptionString = "Api : UpdateRollOverDayJobs" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateRollOverDayJobs - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


    }
}