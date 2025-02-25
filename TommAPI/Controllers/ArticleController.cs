using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TommBLL.Interface;
using TommDAL.Models;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using TommDAL.ViewModel;
using StackExchange.Redis;

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {

        HttpClient client = new HttpClient();
        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly ICacheService _cacheService;

        public IConfiguration _configuration { get; }
        private IArticle _articleservice;
        private IServices _services;   
        private IHostingEnvironment _env;
        

        public ArticleController(
            IConfiguration configuration, ICacheService cacheService, 
            IArticle articleservice, IStringLocalizer<AccountController> localizer, IServices services, IHostingEnvironment env)
        {
            _configuration = configuration;
            _articleservice = articleservice;
            _localizer = localizer;
            _services = services;        
            _env = env;
            _cacheService = cacheService;
        }
        #endregion
 

        [HttpGet]
        [Authorize]
        [Route("GetArticleCategories")]
        public async Task<IActionResult> GetArticleCategories()
        {
            Response oResponse = new Response();
            try
            {
                var cacheKey = "ArticleCategory";
                var articleCategory = _cacheService.GetData<List<ArticleCategoryMst>>(cacheKey);

                if (articleCategory != null && articleCategory.Any())
                {
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("data", articleCategory);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                var model = await _articleservice.GetArticleCategories();
                if (model.Count > 0)
                {                    
                    _cacheService.SetData(cacheKey, model);
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("data", model);
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
                string ExceptionString = "Api : GetArticleCategories" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetArticleCategories - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("GetArticleData")]
        public async Task<IActionResult> GetArticleData(ArticlesPagination obj)
        {
            Response oResponse = new Response();
            
            try
            {
                var cacheKey = "Articles";

                // Attempt to retrieve data from Redis Cache
                var cachedArticleData = _cacheService.GetData<List<ArticleData>>(cacheKey);

                if (cachedArticleData != null && cachedArticleData.Any())
                {
                    // Data found in the cache, return it
                    var articles = _articleservice.GetArticleList(cachedArticleData, obj);

                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("data", articles);
                    res.Add("Message", _localizer["Success"].Value);

                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }


                // Data not found in the cache, fetch from the service
                var articledata = await _articleservice.GetArticleData();

                if (articledata != null && articledata.Any())
                {
                    // Set data in Redis Cache
                    _cacheService.SetData(cacheKey, articledata);

                    // Process the data and return the result
                    var articles = _articleservice.GetArticleList(articledata, obj);

                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("data", articles);
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

                string ExceptionString = "Api : GetArticleData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetArticleData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("SaveArticleData")]
        public async Task<IActionResult> SaveArticleData(Articles article)
        {
            Response oResponse = new Response();
            try
            {
                var ArticleId = await _articleservice.SaveArticleData(article);
                if (ArticleId > 0)
                {
               
                    _cacheService.RemoveData("Articles");
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

                string ExceptionString = "Api : SaveArticleData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "SaveArticleData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        [Authorize]
        [HttpDelete]
        [Route("DeleteArticle")]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            Response oResponse = new Response();
            try
            {             
                _cacheService.RemoveData("Articles");
                Boolean Isdeleted = await _articleservice.DeleteArticle(articleId);
                if (Isdeleted)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["Success"].Value;
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

                string ExceptionString = "Api : DeleteArticle" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "DeleteArticle - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        [HttpGet]
        [Authorize]
        [Route("GetArticleDataById")]
        public async Task<IActionResult> GetArticleDataById(int articleId)
        {
            Response oResponse = new Response();
            try
            {
                var ArticleData = await _articleservice.GetArticleDataById(articleId);
                if (ArticleData.Id > 0)
                {
                    oResponse.Success = true;
                    oResponse.data = ArticleData;
                    oResponse.Message = _localizer["Success"].Value;
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

                string ExceptionString = "Api : GetArticleDataById" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetArticleDataById - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);


                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [HttpPost]
        [Route("UpdateArticleBookMark")]
        [Authorize]
        public async Task<IActionResult> UpdateArticleBookMark(ArticleBookMarks articlebookMark)
        {
            Response oResponse = new Response();
            try
            {
                Boolean IsSave = false;
                IsSave = await _articleservice.UpdateArticleBookMark(articlebookMark);
                if (IsSave)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["BookMark"].Value;
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

                string ExceptionString = "Api : UpdateArticleBookMark" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateArticleBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        [HttpGet]
        [Route("GetArticleBookMark")]
        [Authorize]
        public async Task<IActionResult> GetArticleBookMark(int UserId)
        {
            Response oResponse = new Response();
            try
            {
                var articlebookMarkData = new List<ArticleBookMarkResponse>();

                articlebookMarkData = await _articleservice.GetUserArticleBookMarks(UserId);

                var cacheKey = "ArticleDataList";
                var articleList = _cacheService.GetData<ArticlesResponse>(cacheKey);

                if (articleList != null)
                {
                    var articles = _articleservice.GetArticleBookMark(articleList, articlebookMarkData);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("data", articles);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                var articleData = await _articleservice.GetArticleDataList();
                List<ArticleData> bookMarks = new List<ArticleData>();
                bookMarks = _articleservice.GetArticleBookMark(articleData, articlebookMarkData);
                ArticleData[] mark = bookMarks.ToArray();

                if (mark.Length > 0)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("data", mark);
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

                string ExceptionString = "Api : GetArticleBookMark" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetArticleBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetArticleDataCategoryWise")]
        public async Task<IActionResult> GetArticleDataCategoryWise(long userId, string searchText)
        {
            Response oResponse = new Response();
            try
            {
                var cacheKey = "Articles";
                var cachedArticleData = _cacheService.GetData<List<ArticleData>>(cacheKey);

                if (cachedArticleData != null && cachedArticleData.Any())
                {
                    var articles = await _articleservice.GetArticleDataCategoryWise(cachedArticleData, userId, searchText);
                    oResponse.Success = true;
                    oResponse.data = articles;
                    oResponse.Message = _localizer["Success"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    var articleData = await _articleservice.GetArticleData();
                    if (articleData != null)
                    {
                        var articles = await _articleservice.GetArticleDataCategoryWise(articleData, userId, searchText);
                        Dictionary<string, object> res = new Dictionary<string, object>();
                        res.Add("Success", true);
                        res.Add("data", articles);
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
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetArticleDataCategoryWise" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetArticleDataCategoryWise - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }


        }

        [HttpGet]
        [Authorize]
        [Route("GetArticlesToApp")]
        public async Task<IActionResult> GetArticlesToApp(long userId, int skipId, int categoryId, int pageSize = 2, int pageNumber = 1, bool isSkipThree = false)
        {
            Response oResponse = new Response();
            try
            {
                var cacheKey = "Articles";
                var cachedArticleData = _cacheService.GetData<List<ArticleData>>(cacheKey);

                if (cachedArticleData != null && cachedArticleData.Any())
                {
                    var articles = await _articleservice.GetArticles(cachedArticleData, userId, skipId, categoryId, pageSize, pageNumber, isSkipThree);
                    oResponse.Success = true;
                    oResponse.data = articles;
                    oResponse.Message = _localizer["Success"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    var articleData = await _articleservice.GetArticleData();
                    if (articleData != null)
                    {
                        var articles = await _articleservice.GetArticles(articleData, userId, skipId, categoryId, pageSize, pageNumber, isSkipThree);
                        Dictionary<string, object> res = new Dictionary<string, object>();

                        res.Add("Success", true);
                        res.Add("data", articles);
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
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetArticlesToApp" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetArticlesToApp - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);

                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }
        
    }
}
