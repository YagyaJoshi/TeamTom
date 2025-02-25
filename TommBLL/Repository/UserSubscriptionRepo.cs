using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using TommBLL.Interface;
using TommDAL.Models;
using Microsoft.AspNetCore.Hosting;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Microsoft.Extensions.Hosting;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using Org.BouncyCastle.Bcpg.Sig;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Jose;
using System.Linq;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using RestSharp;
using MySql.Data.MySqlClient.Memcached;
using Newtonsoft.Json.Linq;

namespace TommBLL.Repository
{
    public class UserSubscriptionRepo : IUserSubscription
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        private IHostingEnvironment _env;
        public UserSubscriptionRepo(IServices services, IConfiguration configuration, IHostingEnvironment env) 
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            _env = env;

        }
        #endregion

        public async Task<UserSubscription> GetUserSubscription(int userId)
        {
            var response = new UserSubscription();
            using (MySqlConnection objCon1 = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    using (var cmd = new MySqlCommand("sp_GetUserSubscription", objCon1))
                    {
                        cmd.Parameters.AddWithValue("userId", userId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        await objCon1.OpenAsync();
                        using (var reader =await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Id = Convert.ToInt32(reader["id"]);
                                response.userId = Convert.ToInt32(reader["UserId"]);
                                response.packageName = Convert.ToString(reader["PackageName"]);
                                response.subscriptionId = Convert.ToString(reader["SubscriptionId"]);
                                response.token = Convert.ToString(reader["Token"]);
                                response.platform = Convert.ToString(reader["platform"]);
                                response.status = Convert.ToString(reader["Status"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //objCon.Close();
                    //objCon.Dispose();
                    string ExceptionString = "Repo : GetUserSubscription" + Environment.NewLine + ex.Message + " " + userId + ex.StackTrace;
                    var fileName = "GetUserSubscription - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                }
                finally
                {
                    //objCon.Close();
                    //objCon.Dispose();
                }
            }
            return response;
        }

        public async Task<int> AddUserSubscription(UserSubscription userSubscription)
        {
            var isSave = 2;
            try
            {
                using (var cmd = new MySqlCommand("sp_SaveUserSubscription", objCon))
                {
                    cmd.Parameters.AddWithValue("userId", userSubscription.userId);
                    cmd.Parameters.AddWithValue("packageName", userSubscription.packageName);
                    cmd.Parameters.AddWithValue("subscriptionId", userSubscription.subscriptionId);
                    cmd.Parameters.AddWithValue("token", userSubscription.token);
                    cmd.Parameters.AddWithValue("platform", userSubscription.platform);
                    cmd.Parameters.AddWithValue("status", userSubscription.status);
                    cmd.Parameters.AddWithValue("createdDate", userSubscription.createdDate);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            isSave = Convert.ToInt32(reader["isExist"]);
                        }
                    }           
                }
            }
            catch(Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : AddUserSubscription" + Environment.NewLine + ex.StackTrace + " " + ex.Message + " " + JsonConvert.SerializeObject(userSubscription);

                var fileName = "AddUserSubscription - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return isSave;
        }

        public async Task<bool> CancelUserSubscription(UserSubscription userSubscription) 
        {
            var isSave = false;
            try
            {
                using (var cmd = new MySqlCommand("sp_CancelUserSubscription", objCon))
                {
                    cmd.Parameters.AddWithValue("userId", userSubscription.userId);
                    cmd.Parameters.AddWithValue("packageName", userSubscription.packageName);
                    cmd.Parameters.AddWithValue("subscriptionId", userSubscription.subscriptionId);
                    cmd.Parameters.AddWithValue("token", userSubscription.token);
                    cmd.Parameters.AddWithValue("status", userSubscription.status);                  
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    isSave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CancelUserSubscription" + Environment.NewLine + ex.StackTrace;
                var fileName = "CancelUserSubscription - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return isSave;
        }

        public async Task<(bool, bool)> GoogleSubscriptionCancellation(string packageName, string subscriptionId, string token)
        {
            try
            {
                string fileName = "pc-api-6770412591175612408-157-dcf3ee9d5708.json";
                string directoryName = "CredentialJson";
                string pathToFile = Path.Combine(_env.ContentRootPath, directoryName, fileName);

                // Create a credential using the service account key
                GoogleCredential credential;

                using (var stream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(AndroidPublisherService.Scope.Androidpublisher);
                }

                // Create the Android Publisher Service
                var androidPublisherService = new AndroidPublisherService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential
                });

                // Make the API request to get subscription details
                var result = androidPublisherService.Purchases.Subscriptions.Get(
                         packageName, subscriptionId, token).Execute();

                // Parse response and determine the subscription status
                SubscriptionPurchase subscriptionPurchase = result;

                DateTime expiryTime = DateTimeOffset.FromUnixTimeMilliseconds((long)subscriptionPurchase.ExpiryTimeMillis).UtcDateTime;

                var isCancelled = false;
                var isExpired = false;

                if (subscriptionPurchase.CancelReason == 1)
                    isCancelled = true;

                if (DateTime.UtcNow > expiryTime)
                    isExpired = true;

                return (isCancelled, isExpired);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Gone"))
                    return (true, true); 
                throw new Exception(ex.Message);
                
            }
        }

        public async Task<(bool, bool)> AppleSubscriptionCancellation(string packageName, string subscriptionId, string purchaseToken)
        {
            var isCancelled = false;
            var isExpired = false;

            var restClient = new RestClient();

            try
            {
                var tokenApi = _configuration["AuthKey:apiUrl"] + purchaseToken;

                var request = new RestRequest(tokenApi);
                RestResponse result = await restClient.ExecuteAsync(request); // Make the request asynchronously

                if (result.IsSuccessStatusCode)
                {
                    JObject responseJson = JsonConvert.DeserializeObject<JObject>(result.Content);
                    var status = (int)responseJson["status"];
                    if (status == 2)
                    {
                        isCancelled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                return(false, false);
            }

            return (isCancelled, isExpired);
        }

            //string fileName = "AuthKey_T54LFMVNJ3.p8";
            //string directoryName = "SubscriptionAuthKey";
            //string pathToFile = Path.Combine(_env.ContentRootPath, directoryName, fileName);

            //var file = File.ReadAllText(pathToFile);

            //// Convert the header to IDictionary<string, object>
            //var header = new Dictionary<string, object>
            //{
            //     { "alg",  _configuration["AuthKey:alg"] },
            //     { "kid", _configuration["AuthKey:keyId"]},
            //     { "Typ",  _configuration["AuthKey:Typ"]}
            //};

            //// Payload
            //DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            //var payload = new
            //{
            //    iss = _configuration["AuthKey:issId"],
            //    iat = currentTime.ToUnixTimeSeconds(),
            //    exp = currentTime.AddMinutes(20).ToUnixTimeSeconds(),
            //    aud = _configuration["AuthKey:aud"],
            //    bid = _configuration["AuthKey:bid"]
            //};

            //string privateKey = file.Replace("-----BEGIN PRIVATE KEY-----", "")
            //                           .Replace("-----END PRIVATE KEY-----", "")
            //                           .Replace("\n", "");

            ////  // Convert the PEM-encoded private key to bytes
            //byte[] privateKeyBytes = Convert.FromBase64String(privateKey);

            //// Create a CngKey from the private key bytes
            //CngKey privateKeys = CngKey.Import(privateKeyBytes, CngKeyBlobFormat.Pkcs8PrivateBlob);

            ////  Sign the JWT token
            //string token = Jose.JWT.Encode(payload, privateKeys, JwsAlgorithm.ES256, extraHeaders: header);
            //var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            ////transactions/2000000400140834";
            //string apiUrl = _configuration["AuthKey:appleUrl"] + purchaseToken;

            //var response = await httpClient.GetAsync(apiUrl);
            //if (response.IsSuccessStatusCode)
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Root root = JsonConvert.DeserializeObject<Root>(content);

            //    var statusValues = root.data
            //        .SelectMany(datum => datum.lastTransactions)
            //        .Any(lastTransaction => lastTransaction.status == 2);
            //    if (statusValues)
            //    {
            //        isCancelled = true;
            //    }

            //}
            //var token = "";         
        
    }
}
