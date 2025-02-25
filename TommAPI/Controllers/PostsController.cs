
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

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        HttpClient client = new HttpClient();
        #region Dependency injection  
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly ICacheService _cacheService;

        public IConfiguration _configuration { get; }
        private IPost _postservice;
        private IServices _services;        
        private IHostingEnvironment _env;
        public PostsController(
            IConfiguration configuration, ICacheService cacheService,
            IPost postservice, IStringLocalizer<AccountController> localizer, IServices services, IHostingEnvironment env)
        {
            _configuration = configuration;
            _postservice = postservice;
            _localizer = localizer;
            _services = services;
            _env = env;
            _cacheService = cacheService;
        }
        #endregion

        [HttpPost]
        [Authorize]
        [Route("GetPostData_old")]       // ---****--- not in use    ---***---
        public async Task<IActionResult> GetPostData_old(PostsPagination obj)
        {
            Response oResponse = new Response();
            try
            {
                var postdata =await  _postservice.GetPostData_old(obj); 
                if (postdata != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", postdata);
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

                string ExceptionString = "Api : GetPostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("GetPostDataList_old")]    // ---***--  not in use  ---***---
        public async Task<IActionResult> GetPostDataList_old(PostsPagination obj)
        {
            Response oResponse = new Response();
            try
            {
                //string subcategoryId = "";
                //if (obj.SubCategoryId != null)
                //{
                //    foreach (var item in obj.SubCategoryId)
                //    {
                //        subcategoryId = subcategoryId + ',' + item;
                //    }
                //}
                //if (subcategoryId.Length > 0)
                //    subcategoryId = subcategoryId.Substring(1);

                var postdata =await _postservice.GetPostDataList_old(obj);
                if (postdata != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", postdata);
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

                string ExceptionString = "Api : GetPostDataList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostDataList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Post Data to admin pannel
        /// </summary>
        /// <param name="obj">PostsPagination</param>
        /// <return>PostList</return>
        /// <returns>Status</returns>

        [HttpPost]
        [Authorize]
        [Route("GetPostData")]
        public async Task<IActionResult> GetPostData(PostsPagination obj)
        {
            Response oResponse = new Response();
            try
            {
                var cacheKey = "Posts";
                var cachedPostData = _cacheService.GetData<List<Posts>>(cacheKey);
                
                if (cachedPostData != null && cachedPostData.Any())
                {          
                    var posts =await _postservice.GetPostList(cachedPostData, obj);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", posts);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                  
                }
                var postdata = await _postservice.GetPostData();
                if (postdata != null && postdata.Any())
                {
                    
                    _cacheService.SetData(cacheKey, postdata);
                    var posts =await _postservice.GetPostList(postdata, obj);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", posts);
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

                string ExceptionString = "Api : GetPostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Post Data List to app
        /// </summary>
        /// <param name="obj">PostsPagination</param>
        /// <return>PostList</return>
        /// <returns>Status</returns>


        [HttpPost]
        [Authorize]
        [Route("GetPostDataList")]
        public async Task<IActionResult> GetPostDataList(PostsPagination obj)
        {
            Response oResponse = new Response();
            try
            {
                var bookMarkData = new List<BookMarkResponse>();
                if (obj.UserId.HasValue)
                {
                    bookMarkData =await _postservice.GetUserBookMarks(obj.UserId.Value);
                }
                var cacheKey = "PostDataList";
                var postList = _cacheService.GetData<PostResponse>(cacheKey);

                if (postList != null)
                {          
                    var posts =await _postservice.GetPostDataListToApp(postList, obj, bookMarkData);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", posts);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                
                }
                var postdata =await _postservice.GetPostDataList();
                if (postdata != null)
                {
                    
                    _cacheService.SetData(cacheKey, postdata);
                    var posts =await _postservice.GetPostDataListToApp(postdata, obj, bookMarkData);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", posts);
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

                string ExceptionString = "Api : GetPostDataList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostDataList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Post Data using id
        /// </summary>
        /// <param name="postId">PostId</param>
        /// <return>PostData</return>
        /// <returns>Status</returns>

        [HttpGet]
        [Authorize]
        [Route("GetPostDataById")]
        public async Task<IActionResult> GetPostDataById(int postId)
        {
            Response oResponse = new Response();
            try
            {
                var postdata = await _postservice.GetPostDataById(postId);
                if (postdata.Id>0)
                {
                    oResponse.Success = true;
                    oResponse.data = postdata;
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

                string ExceptionString = "Api : GetPostDataById" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostDataById - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to insert Post Data
        /// </summary>
        /// <param name="post">Posts</param>      
        /// <returns>Status</returns>

        [HttpPost]
        [Authorize]
        [Route("SavePostData")]
        public async Task<IActionResult> SavePostData(Posts post)
        {
            Response oResponse = new Response();
            try
            {
                var IsSave = false;
                var postId =await _postservice.SavePostData(post);
                if (postId > 0)
                {                  
                    _cacheService.RemoveData("Posts");                   
                    _cacheService.RemoveData("PostDataList");                    
                    _cacheService.RemoveData("PostCategoriesList");
                    if (post.SubCategoryId != null)
                    {
                        for (var i = 0; i < post.SubCategoryId.Length; i++)
                        {
                            IsSave =await _postservice.AddPostCategory(postId, post.SubCategoryId[i]);
                        }
                        if (IsSave)
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
                    oResponse.Success = true;
                    oResponse.Message = _localizer["success"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : SavePostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "SavePostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// For new app - This api is used to update Post Data
        /// </summary>
        /// <param name="post">Posts</param>
        /// <returns>Status</returns>

        [HttpPut]
        [Authorize]
        [Route("UpdatePostData")]
        public async Task<IActionResult> UpdatePostData(Posts post)
        {
            Response oResponse = new Response();
            try
            {
                var isSave = false;
                var postdata =await _postservice.UpdatePostData(post);
                if (postdata)
                {
                    _cacheService.RemoveData("Posts");
                    _cacheService.RemoveData("PostDataList");
                    _cacheService.RemoveData("PostCategoriesList");
                    isSave =await _postservice.DeletePostCategory(post.Id);
                    if (isSave)
                    {
                        for (var i = 0; i < post.SubCategoryId.Length; i++)
                        {
                            isSave =await _postservice.AddPostCategory(post.Id, post.SubCategoryId[i]);
                        }
                    }
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdateSuccess"].Value;
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

                string ExceptionString = "Api : UpdatePostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdatePostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Post Categories to app
        /// </summary>        
        /// <returns>PostCategoryMst</returns>
        [HttpGet]
        [Authorize]
        [Route("GetPostCategories")]
        public async Task<IActionResult> GetPostCategories()
        {
            Response oResponse = new Response();
            try
            {
                var cacheKey = "PostCategories";
                var postCategories = _cacheService.GetData<List<PostCategoryMst>>(cacheKey); 
                if (postCategories != null && postCategories.Any())
                {
                   
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", postCategories);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                var model =await _postservice.GetPostCategoriesToApp(); 
                if (model.Count > 0)
                {                   
                    _cacheService.SetData(cacheKey, model);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", model);
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

                string ExceptionString = "Api : GetPostCategories" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostCategories - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Post Category List to admin panel
        /// </summary>        
        /// <returns>PostCategoryResponse</returns>

        [HttpGet]
        [Authorize]
        [Route("GetPostCategoryList")]
        public async Task<IActionResult> GetPostCategoryList(string PostId = "")
        {
            Response oResponse = new Response();
            try
            {
                var cacheKey = "PostCategoriesList";
                var postCategories = _cacheService.GetData<PostResponse>(cacheKey);


                if (postCategories != null)
                {
                    var categorydata = await _postservice.GetPostCategoryList(postCategories, PostId);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", categorydata);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                var postData =await _postservice.GetPostDataList();
                if (postData!= null)
                {
                    _cacheService.SetData(cacheKey, postData);
                    var categorydata =await _postservice.GetPostCategoryList(postData, PostId); 
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", categorydata);
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

                string ExceptionString = "Api : GetPostCategoryList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPostCategoryList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Filtered Categories like - "Mood", "Classic TOMM", "Time" to app 
        /// </summary>       
        /// <returns>PostSubCategoryMst</returns>
        [HttpGet]
        [Authorize]
        [Route("GetFilteredCategories")]
        public async Task<IActionResult> GetFilterCategoryList()
        {
            Response oResponse = new Response();
            try
            {

                var cacheKey = "PostCategoriesList";
                var postCategories = _cacheService.GetData<PostResponse>(cacheKey);

                if (postCategories != null)
                {
                    var categorydata =await _postservice.GetFilteredCategories(postCategories);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", categorydata);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                var postData =await _postservice.GetPostDataList();
                if (postData != null)
                {
                    _cacheService.SetData(cacheKey, postData);
                    var categorydata =await _postservice.GetFilteredCategories(postData);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", categorydata);
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

                string ExceptionString = "Api : GetFilteredCategories" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetFilteredCategories - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get BookMark post to app
        /// </summary>
        /// <param name="obj">BookMarkData</param>
        /// <returns>status</returns>
        [HttpPost]
        [Route("GetBookMark")]
        [Authorize]
        public async Task<IActionResult> GetBookMark(BookMarkData obj) 
        {
            Response oResponse = new Response();
            try
            {
                var bookMarkData = new List<BookMarkResponse>();
                
                bookMarkData =await _postservice.GetUserBookMarks(obj.UserId);
                
                var cacheKey = "PostDataList";
                var postList = _cacheService.GetData<PostResponse>(cacheKey);

                if (postList != null)
                {
                    var posts =await _postservice.GetBookMark(obj, postList, bookMarkData);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", posts);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }
                var postData =await _postservice.GetPostDataList();
                List<Posts> bookMarks = new List<Posts>();
                bookMarks =await _postservice.GetBookMark(obj, postData, bookMarkData);
                Posts[] mark = bookMarks.ToArray();

                if (mark.Length > 0)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", mark);
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

                string ExceptionString = "Api : GetBookMark" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to update Book Mark to post  to app
        /// </summary>
        /// <param name="bookMark">BookMarks</param>
        /// <returns>status</returns>
        [HttpPost]
        [Route("UpdateBookMark")]
        [Authorize]
        public async Task<IActionResult> UpdateBookMark(BookMarks bookMark)
        {
            Response oResponse = new Response();
            try
            {
                Boolean IsSave = false;
                IsSave =await _postservice.UpdateBookMark(bookMark);
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

                string ExceptionString = "Api : UpdateBookMark" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Update Post AudioUrl to admin panel
        /// </summary>
        /// <param name="post">PostAudio</param>
        /// <returns>status</returns>
        [Authorize]
        [HttpPut]
        [Route("UpdatePostAudioUrl")]
        public async Task<IActionResult> UpdatePostAudioUrl(PostAudio post)  
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
                Boolean IstaskSave =await _postservice.UpdatePostAudioUrl(post); 
                if (IstaskSave)
                {
                    _cacheService.RemoveData("Posts");
                    _cacheService.RemoveData("PostDataList");
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UpdatePost"].Value;
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

                string ExceptionString = "Api : UpdatePostAudioUrl" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdatePostAudioUrl - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Get Books to app
        /// </summary>      
        /// <returns>Books</returns>
        [HttpGet]
        [Route("GetBooks")]
        [Authorize]
        public async Task<IActionResult> GetBooks() 
        {
            Response oResponse = new Response();
            try
            {
                // Attempt to retrieve the data from the cache
                var cacheKey = "Books";
                var cachedBooks = _cacheService.GetData<List<Books>>(cacheKey);

                if (cachedBooks != null && cachedBooks.Any())
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    // Data found in the cache, return it
                    res.Add("Success", true);
                    res.Add("Data", cachedBooks);
                    res.Add("Message", _localizer["Success"].Value);
                    ObjectResult responsedata = new ObjectResult(res);
                    return responsedata;
                }

                var books =await _postservice.GetBooks();             

                if (books.Count>0)
                {
                    //set data in cache
                    _cacheService.SetData(cacheKey, books);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", books);
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

                string ExceptionString = "Api : GetBooks" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetBooks - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Delete Post using post id
        /// </summary>
        /// <param name="postId">postId</param>
        /// <returns>status</returns>
        [Authorize]
        [HttpDelete]
        [Route("DeletePost")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            Response oResponse = new Response();
            try
            {
                _cacheService.RemoveData("Posts");
                _cacheService.RemoveData("PostDataList");
                _cacheService.RemoveData("PostCategoriesList");
                Boolean Isdeleted =await _postservice.DeletePost(postId);
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

                string ExceptionString = "Api : DeletePost" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "DeletePost - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to insert bulk Post Data at admin panel using csv file
        /// </summary>
        /// <param name="SyncPost">List<SyncPost></param>
        /// <returns>status</returns>

        [AllowAnonymous]
        [HttpPost]
        [Route("SyncPostData")]
        public async Task<IActionResult> SyncPostData(List<SyncPost> SyncPost)  
        {
            Console.WriteLine("syncpost", SyncPost); 
            var oResponse = new Response();
            try
            {
                _cacheService.RemoveData("Posts");
                _cacheService.RemoveData("PostDataList");
                _cacheService.RemoveData("PostCategoriesList");

                foreach (var postData  in SyncPost)
                {
                  var isSave =await _postservice.SaveSyncPostData(postData);
                    if (!isSave)
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Failed"].Value;
                        return new ObjectResult(oResponse);
                    }
                }                
                oResponse.Success = true;
                oResponse.Message = _localizer["Success"].Value;
                return new ObjectResult(oResponse);
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = $"Api : SyncPostData{Environment.NewLine}" + ex.Message + " " + ex.StackTrace;
                ExceptionString += $"Exception : {JsonConvert.SerializeObject(oResponse)}{Environment.NewLine}";

                var fileName = $"SyncPostData - {DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss")}";
                await _services.SendMail(_configuration["Log:ErrorAddress"], fileName, ExceptionString);

                return new ObjectResult(oResponse);
            }
        }


        //[AllowAnonymous]
        //[HttpGet]
        //[Route("ImportExcelSheetData")]
        //public IActionResult ImportExcelSheetData()  
        //{
        //    Response oResponse = new Response();
        //    try
        //    {
        //        string filePath = "C:\\Users\\PHP-13\\Documents\\Downloads\\Upload Test 10 Samples.csv";

        //        var csvDataList = _postservice.ImportExcelsheetData(filePath);

        //        foreach(var csvData in csvDataList) 
        //        {
        //            //var data = _postservice.InsertPostData(csvData);
        //        }

        //        oResponse.Success = true;
        //        oResponse.Message = _localizer["Success"].Value;
        //        ObjectResult responsedata = new ObjectResult(oResponse);
        //        return responsedata;
        //    }
        //    catch(Exception ex)
        //    {
        //        oResponse.Success = false;
        //        oResponse.Message = ex.Message;

        //        string ExceptionString = "Api : ImportExcelSheetData" + Environment.NewLine;
                
        //        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
        //        var fileName = "ImportExcelSheetData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
        //        _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
        //        ObjectResult responsedata = new ObjectResult(oResponse);
        //        return responsedata;
        //    }
        //}            
        
    }
}
