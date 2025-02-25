using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        #region Dependency injection  

        public IConfiguration _configuration { get; }
        private IPlayList _playlistservice;
        private IHostingEnvironment _hostingEnvironment;
        private IServices _services;
        private readonly IStringLocalizer<AccountController> _localizer;
        public PlayListController(
            IConfiguration configuration,
            IPlayList playlistservice, IHostingEnvironment environment, 
            IStringLocalizer<AccountController> localizer, IServices services)
        {
            _configuration = configuration;
            _playlistservice = playlistservice;
            _hostingEnvironment = environment;
            _localizer = localizer;
            _services = services;
        }

        #endregion

        /// <summary>
        /// This api is used to Get Spotify PlayList
        /// </summary>
        /// <param name="model"></param>
        /// <returns>PlayListViewModel</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetSpotifyPlayList")]
        public async Task<IActionResult> GetSpotifyPlayList()
        {
            Response oResponse = new Response();
            try
            {
                //SpotifyViewModel model = new SpotifyViewModel();
                //model.access_token = "BQDWgL2KLOuCXJJyImpfSoA1UahiW2GLqFZ_t1CqLVdygVorf3nx8aQpan2qckVdFmcLPPthCdrsBfQtW-A";
                SpotifyViewModel model =await _playlistservice.GetAccessToken();
                if (model != null)
                {
                    if (model.access_token != null)
                    {
                        PlayListViewModel Playlist =await _playlistservice.GetPlaylists(model);

                        if (Playlist != null)
                        {
                            Dictionary<string, object> data = new Dictionary<string, object>();
                            Dictionary<string, object> res = new Dictionary<string, object>();

                            res.Add("Success", true);
                            res.Add("Data", Playlist);
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
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["SpotifyTokenFailed"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["SpotifyTokenFailed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;
                string ExceptionString = "Get Api GetSpotifyPlayList" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetSpotifyPlayList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Get Random Audio for Motivate User
        /// </summary>
        /// <param name="model"></param>
        /// <returns>playlisturl</returns>
        /// <returns>Status</returns>
        [Authorize]
        [HttpGet]
        [Route("GetRandomAudioFile")]
        public async Task<IActionResult> GetRandomAudioFile()
        {
            Response oResponse = new Response();
            try
            {
                var rand = new Random();
                string BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                //var file = new ;
                int index = new Random().Next(0, 27);
                string playlisturl = _configuration["File:AudioFilePath"]+index +".wav";
                if (playlisturl != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", playlisturl);
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

                string ExceptionString = "Api : GetRandomAudioFile" + Environment.NewLine;
                // ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetRandomAudioFile - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }
    }
}