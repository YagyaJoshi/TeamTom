using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.ViewModel;
using RestSharp;


namespace TommBLL.Repository
{
    public class PlayListRepo : IPlayList
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        private IServices _services;
        public PlayListRepo(IConfiguration configuration, IServices services)
        {
            _configuration = configuration;
            _services = services;
        }
        #endregion

        public async Task<SpotifyViewModel> GetAccessToken()
        {
            SpotifyViewModel model = null;
  
            try
            {
                var client = new RestClient(_configuration["Spotify:TokenUrl"]);

                // Set basic authentication header
                var encodeClientIDClientSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["Spotify:ClientId"]}:{_configuration["Spotify:ClientSecret"]}"));
                client.AddDefaultHeader("Authorization", "Basic " + encodeClientIDClientSecret);

                // Create request
                var request = new RestRequest
                {
                    Method = Method.Post // Set the HTTP method
                };

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("grant_type", "client_credentials");

                // Execute request
                var response = await client.ExecuteAsync(request);
                
                if (response.IsSuccessful)
                {
                    model = JsonConvert.DeserializeObject<SpotifyViewModel>(response.Content);
                }
                else
                {
                    // Handle unsuccessful response
                    var errorMessage = $"Failed to get access token. Status Code: {response.StatusCode}, Content: {response.Content}";
                    await HandleError(errorMessage);
                }
                
            }
            catch (Exception ex)
            {
                var exceptionString = $"Error on Repo: GetAccessToken{Environment.NewLine}{ex.Message} {ex.StackTrace}";
                var fileName = $"GetAccessToken - {System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss")}";
                await _services.SendMail(_configuration["Log:ErrorAddress"], fileName, exceptionString);
            }

            return model;
        }
        private async Task HandleError(string errorMessage)
        {
            // Handle error, log, send email, etc.
            // You can customize this method based on your error handling requirements.
            await _services.SendMail(_configuration["Log:ErrorAddress"], "GetAccessTokenError", errorMessage);
        }


        public async Task<PlayListViewModel> GetPlaylists(SpotifyViewModel model)
        {
            PlayListViewModel playmodel = null;
            try
            {
                var client = new RestClient(_configuration["Spotify:PlayListUrl"]);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", "Bearer " + model.access_token);

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    playmodel = JsonConvert.DeserializeObject<PlayListViewModel>(response.Content);
                }
                else
                {
                    // Handle unsuccessful response
                    var errorMessage = $"Failed to get playlists. Status Code: {response.StatusCode}, Content: {response.Content}";
                    await HandleError(errorMessage);
                }
            }
            catch (Exception ex)
            {
                var exceptionString = $"Repo: GetPlaylists{Environment.NewLine}{ex.Message} {ex.StackTrace} {JsonConvert.SerializeObject(model)}";
                var fileName = $"GetPlaylists - {System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss")}";
                await _services.SendMail(_configuration["Log:ErrorAddress"], fileName, exceptionString);
            }

            return playmodel;
        }
    }
}
