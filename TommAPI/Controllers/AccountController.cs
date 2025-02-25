using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MimeKit;
using Newtonsoft.Json;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        #region Dependency injection  

        public IConfiguration _configuration { get; }
        private readonly IStringLocalizer<AccountController> _localizer;
        private IHostingEnvironment _env;
        private IUser _usersservice;
        private IAuth _authservice;
        private IServices _services;
        public AccountController(
            IConfiguration configuration,
            IUser usersservice,
            IServices services,
            IAuth authservice,
            IStringLocalizer<AccountController> localizer, IHostingEnvironment env)
        {
            _configuration = configuration;
            _usersservice = usersservice;
            _services = services;
            _authservice = authservice;
            _localizer = localizer;
            _env = env;
        }

        #endregion

        /// <summary>
        /// This is api is used to login
        /// </summary>
        /// <param name="omodel">LoginViewModel</param>
        /// <returns>Status with authentication token</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel omodel)
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
                User oUser = await _usersservice.GetUser(omodel);
                bool matches = false;
                if (oUser != null && oUser.FirstName != null)
                {
                    //if (oUser.Ran == null)
                    //{
                    matches = Crypter.CheckPassword(omodel.Password, oUser.Token);
                    //}
                    //else
                    //{
                    //    matches = BCrypt.Net.BCrypt.Verify(omodel.Password, oUser.Token);
                    //}
                    if (matches)
                    {
                        string Token = _authservice.GetToken(oUser.Id, "User");


                        Dictionary<string, object> data = new Dictionary<string, object>();
                        Dictionary<string, object> res = new Dictionary<string, object>();


                        res.Add("Success", true);
                        res.Add("Data", new
                        {
                            Token = Token.ToString(),
                            User = oUser
                        });
                        res.Add("Message", _localizer["LoginSuccess"].Value);
                        ObjectResult responsedata = new ObjectResult(res);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["UserPassNotMatch"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["UserPassNotMatch"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : Login" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Login - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This is api is used to login
        /// </summary>
        /// <param name="omodel">LoginViewModel</param>
        /// <returns>Status with authentication token</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("LoginV3")]
        public async Task<IActionResult> LoginV3([FromBody] LoginViewModel omodel)
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
                User oUser = await _usersservice.GetUserV3(omodel);
                bool matches = false;
                if (oUser != null && oUser.FirstName != null)
                {
                    //if (oUser.Ran == null)
                    //{
                    matches = Crypter.CheckPassword(omodel.Password, oUser.Token);
                    //}
                    //else
                    //{
                    //    matches = BCrypt.Net.BCrypt.Verify(omodel.Password, oUser.Token);
                    //}
                    if (matches)
                    {
                        string Token = _authservice.GetToken(oUser.Id, "User");


                        Dictionary<string, object> data = new Dictionary<string, object>();
                        Dictionary<string, object> res = new Dictionary<string, object>();


                        res.Add("Success", true);
                        res.Add("Data", new
                        {
                            Token = Token.ToString(),
                            User = oUser
                        });
                        res.Add("Message", _localizer["LoginSuccess"].Value);
                        ObjectResult responsedata = new ObjectResult(res);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["UserPassNotMatch"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["UserPassNotMatch"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : Login_Version_3" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Login - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This is api is used to login to new app
        /// </summary>
        /// <param name="omodel">LoginViewModel</param>
        /// <returns>Status with authentication token</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("LoginV5")]
        public async Task<IActionResult> LoginV5([FromBody] LoginViewModel omodel)
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
                User oUser = await _usersservice.GetUserV5(omodel);
                bool matches = false;
                if (oUser != null && oUser.FirstName != null)
                {
                    //if (oUser.Ran == null)
                    //{
                    matches = Crypter.CheckPassword(omodel.Password, oUser.Token);
                    //}
                    //else
                    //{
                    //    matches = BCrypt.Net.BCrypt.Verify(omodel.Password, oUser.Token);
                    //}
                    if (matches)
                    {
                        string Token = _authservice.GetToken(oUser.Id, "User");


                        Dictionary<string, object> data = new Dictionary<string, object>();
                        Dictionary<string, object> res = new Dictionary<string, object>();


                        res.Add("Success", true);
                        res.Add("Data", new
                        {
                            Token = Token.ToString(),
                            User = oUser
                        });
                        res.Add("Message", _localizer["LoginSuccess"].Value);
                        ObjectResult responsedata = new ObjectResult(res);
                        return responsedata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["UserPassNotMatch"].Value;
                        ObjectResult responsedata = new ObjectResult(oResponse);
                        return responsedata;
                    }
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["UserPassNotMatch"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : Login_VersionV5" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Login_VersionV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This is api is used to Update User Device Token
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <returns>Status</returns>
        [HttpGet]
        [Route("LogOut")]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut(long UserId)
        {
            Response oResponse = new Response();
            try
            {
                Boolean Issave = await _usersservice.Logout(UserId);
                Dictionary<string, object> data = new Dictionary<string, object>();
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("Success", true);
                res.Add("Message", _localizer["UserUpdateSuccess"].Value);
                ObjectResult responsedata = new ObjectResult(res);
                return responsedata;


            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : LogOut" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(UserId) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This is api is used to Register New User
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] User omodel)
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
                var password = omodel.Token;

                int Issave = await _usersservice.RegisterUser(omodel);
                if (Issave == 1)
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NotExist"].Value;
                    bool send = await _services.SendMail(_configuration["Log:ErroAddress"], _localizer["NotExist"].Value, JsonConvert.SerializeObject(omodel));
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else if (Issave == 2)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["Registersuccess"].Value;

                    var webRoot = _env.WebRootPath;

                    var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "Register.html";
                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    string messageBody = string.Format(builder.HtmlBody,
                        omodel.FirstName,
                        omodel.LastName,
                        omodel.Username,
                        password,
                        omodel.Email
                        );
                    bool send = await _services.SendMail(omodel.Email, _localizer["RegisterTommApp"].Value, messageBody);
                    if (send == false)
                    {
                        oResponse.Message = _localizer["MailError"].Value;
                    }
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["RegisterFailed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : Register" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Register - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// This api is used to Reset Password
        /// </summary>
        /// <param name="model">SetPasswordBindingModel</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] SetPasswordBindingModel model)
        {
            Response oResponse = new Response();
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
            else
            {
                try
                {
                    User oUser = await _usersservice.ResetPassword(model.Email);
                    if (oUser != null)
                    {
                        String ResetToken = _authservice.GetPassword(3, 4, 3);
                        //string body = string.Empty;
                        //body = "Hi " + oUser.FirstName + " " + oUser.LastName + ".<br/> " +
                        // "\n\nA password reset has been requested from the TeamTOM App.\n\nYour reset code is: "+ ResetToken + "\n\nPlease enter this code in the App to reset your password.\n\nMany thanks\nTeamTOMM Admin";
                        _usersservice.UpdateResetToken(model.Email, ResetToken);
                        var webRoot = _env.WebRootPath;

                        var pathToFile = _env.WebRootPath
                                + Path.DirectorySeparatorChar.ToString()
                                + "Templates"
                                + Path.DirectorySeparatorChar.ToString()
                                + "Forgot-Password.html";
                        var builder = new BodyBuilder();
                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                        {
                            builder.HtmlBody = SourceReader.ReadToEnd();
                        }

                        string messageBody = string.Format(builder.HtmlBody,
                            oUser.FirstName,
                            oUser.LastName,
                            ResetToken,
                            model.Email
                            );
                     bool send= await _services.SendMail(model.Email, _localizer["ResetPasswordSubject"].Value, messageBody);
                        oResponse.Success = true;
                        oResponse.Message = _localizer["TokenSent"].Value;
                        if (send == false)
                        {
                            oResponse.Message = _localizer["MailError"].Value ;
                        }
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["EmailNotFound"].Value;
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Success = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : ResetPassword" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "ResetPassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");

                    await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }
            }
        }

        /// <summary>
        /// This api is used to Reset Password
        /// </summary>
        /// <param name="model">SetPasswordModel</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("SetPassword")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordModel model)
        {
            Response oResponse = new Response();
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
            else
            {
                try
                {
                    User Ouser = await _usersservice.UpdatePassword(model);
                    if (Ouser != null)
                    {
                        var webRoot = _env.WebRootPath;

                        var pathToFile = _env.WebRootPath
                                + Path.DirectorySeparatorChar.ToString()
                                + "Templates"
                                + Path.DirectorySeparatorChar.ToString()
                                + "Forgot-Password-change.html";
                        var builder = new BodyBuilder();
                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                        {
                            builder.HtmlBody = SourceReader.ReadToEnd();
                        }

                        string messageBody = string.Format(builder.HtmlBody,
                            Ouser.FirstName,
                            Ouser.LastName,
                            Ouser.Username,
                            model.ConfirmPassword,
                            model.Email
                            );
                       bool send= await _services.SendMail(model.Email, _localizer["ForgotPasswordSubject"].Value, messageBody);
                        oResponse.Success = true;
                        oResponse.Message = _localizer["PassUpdate"].Value;
                        if(send==false)
                        {
                            oResponse.Message = _localizer["MailError"].Value;
                        }
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }
                    //else if (IsPassUpdate ==2)
                    //{
                    //    oResponse.Success = false;
                    //    oResponse.Message = _localizer["TokenExpire"].Value;
                    //    ObjectResult resuldata = new ObjectResult(oResponse);
                    //    return resuldata;
                    //}
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Forgotpassfailed"].Value;
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Success = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : ResetPassword" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "ResetPassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);

                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }
            }
        }

        /// <summary>
        /// This api is used to change the password
        /// </summary>
        /// <param name="model">ChangePasswordBindingModel</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordBindingModel model)
        {
            Response oResponse = new Response();
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

            try
            {
                Boolean Ismatches = false;
                Boolean IsCredeintalmatches = false;
                User oUser = await _usersservice.ResetPassword(model.Email);
                if (oUser != null)
                {
                    IsCredeintalmatches = Crypter.CheckPassword(model.OldPassword, oUser.Token);
                    if (IsCredeintalmatches)
                    {
                        Ismatches = Crypter.CheckPassword(model.NewPassword, oUser.Token);
                        //var oUser = DB.OptUsers.Where(r => r.Email == model.Email).FirstOrDefault();
                        if (Ismatches)
                        {
                            oResponse.Success = false;
                            oResponse.Message = _localizer["OldNewPassSame"].Value;
                            ObjectResult resuldata = new ObjectResult(oResponse);
                            return resuldata;
                        }
                        else
                        {
                            Boolean IsChangePass = await _usersservice.ChangePassword(model);
                            //IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword,
                            //model.NewPassword);

                            if (!IsChangePass)
                            {
                                oResponse.Success = false;
                                oResponse.Message = _localizer["Failed"].Value;
                                ObjectResult resuldata = new ObjectResult(oResponse);
                                return resuldata;
                            }
                            else
                            {
                                oResponse.Success = true;
                                oResponse.Message = _localizer["PassUpdate"].Value;
                                ObjectResult resuldata = new ObjectResult(oResponse);
                                return resuldata;
                            }
                        }
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["ChangePassFaild"].Value;
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }
                }
                else
                {
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["EmailNotFound"].Value;
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : ChangePassword" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "ChangePassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");

                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);

                ObjectResult resuldata = new ObjectResult(oResponse);
                return resuldata;
            }
        }



        /// <summary>
        /// This is api is used to login
        /// </summary>
        /// <param name="versionname">LaunchAppVersion</param>
        /// <returns>Status with authentication token</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("LaunchAppVersion")]
        public async Task<IActionResult> LaunchAppVersion(string VersionName)
        {
            Response oResponse = new Response();
            try
            {
                Boolean IsVersionmatches =await  _usersservice.CheckVersion(VersionName); ;
                if (IsVersionmatches)
                {
                    oResponse.Success = true;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : LaunchAppVersion" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(VersionName) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "LaunchAppVersion - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }


        /// <summary>
        /// This is api is used to login
        /// </summary>
        /// <param name="versionname">LaunchAppVersion</param>
        /// <returns>Status with authentication token</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("LaunchAppVersionV3")]
        public async Task<IActionResult> LaunchAppVersionV3(string VersionName, string DeviceToken, string DeviceType, string UserId)
        {
            Response oResponse = new Response();
            try
            {
                Boolean IsVersionmatches = await _usersservice.CheckVersionV3(VersionName, DeviceToken, DeviceType, UserId); ;
                if (IsVersionmatches)
                {
                    oResponse.Success = true;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : LaunchAppVersionV3" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(VersionName) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "LaunchAppVersionV3 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This is api is used to login
        /// </summary>
        /// <param name="versionname">LaunchAppVersion</param>
        /// <returns>Status with authentication token</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("LaunchAppVersionV5")]
        public async Task<IActionResult> LaunchAppVersionV5(string VersionName, string DeviceToken, string DeviceType, string UserId)
        {
            Response oResponse = new Response();
            try
            {
                var data = await _usersservice.CheckVersionV5(VersionName, DeviceToken, DeviceType, UserId); ;
                if (data.Item1)
                {
                    oResponse.Success = true;
                    oResponse.data = new{IsMandatory= data.Item2};
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.data = new { IsMandatory = data.Item2 };
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : LaunchAppVersionV5" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(VersionName) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "LaunchAppVersionV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }



        /// <summary>
        /// This api is used to Get Username
        /// </summary>
        /// <param name="Email">Email</param>
        /// <returns>Status</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetUsername")]
        public async Task<IActionResult> GetUsername(string Email)
        {
            Response oResponse = new Response();
            try
            {
                User oUser = await _usersservice.ResetPassword(Email);
                if (oUser != null)
                {

                    var webRoot = _env.WebRootPath;

                    var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "Forgot-Username.html";
                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    string messageBody = string.Format(builder.HtmlBody,
                        oUser.FirstName,
                        oUser.LastName,
                        oUser.Username,
                        Email
                        );
                    bool send = await _services.SendMail(Email, _localizer["UsernameSubject"].Value, messageBody);
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UsernameSent"].Value;
                    if (send == false)
                    {
                        oResponse.Message = _localizer["MailError"].Value;
                    }

                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["EmailNotFound"].Value;
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }

            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetUsername" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(Email) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine + ex.StackTrace;
                var fileName = "GetUsername - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult resuldata = new ObjectResult(oResponse);
                return resuldata;
            }
        }

        /// <summary>
        /// For new app - This api is used to check using email login user exist or not 
        /// </summary>
        /// <param name="Email">Email</param>
        /// <return>User</return>
        /// <returns>Status with authentication token</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("CheckUserByEmailV5")]
        public async Task<IActionResult> CheckUserbyEmailV5(string Email)   
        {
            Response oResponse = new Response();
            try
            {
                User oUser = await _usersservice.ResetPasswordV5(Email);
                if (oUser != null)
                {                   
                    oResponse.Success = true;
                    oResponse.Message = _localizer["Success"].Value;
                    oResponse.data = new { IsUserExist = true };                   

                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["EmailNotFound"].Value;
                    oResponse.data = new { IsUserExist = false };
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }

            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : CheckUserbyEmailV5" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(Email) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine + ex.StackTrace;
                var fileName = "CheckUserbyEmailV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                ObjectResult resuldata = new ObjectResult(oResponse);
                return resuldata;
            }
        }

        /// <summary>
        /// For new app - This is api is used to Register New User
        /// </summary>
        /// <param name="omodel">User</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterV5")]
        public async Task<IActionResult> RegisterV5([FromBody] UserV5 omodel)  
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
                var password = omodel.Password;

                var (IsSave, user)= await _usersservice.RegisterUserV5(omodel); 
                if (IsSave == 1)
                {                    
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NotExist"].Value;
                    bool send = await _services.SendMail(_configuration["Log:ErroAddress"], _localizer["NotExist"].Value, JsonConvert.SerializeObject(omodel));
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else if (IsSave == 2)
                {
                    string token = _authservice.GetToken(user.Id, "User");
                    oResponse.Success = true;
                    oResponse.Message = _localizer["Registersuccess"].Value;
                    oResponse.data = new {Token = token, User = user};
                    var webRoot = _env.WebRootPath;

                    var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "RegisterV5.html";
                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    string messageBody = string.Format(builder.HtmlBody,
                        omodel.Name,                       
                        password,
                        omodel.Email
                        );
                    bool send = await _services.SendMail(omodel.Email, _localizer["RegisterTommApp"].Value, messageBody);
                    if (send == false)
                    {
                        oResponse.Message = _localizer["MailError"].Value;
                    }
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["RegisterFailed"].Value;
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : RegisterV5" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "RegisterV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        /// <summary>
        /// For new app - This api is used to Reset Password
        /// </summary>
        /// <param name="model">SetPasswordBindingModel</param>
        /// <returns>Status</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPasswordV5")] 
        public async Task<IActionResult> ResetPasswordV5([FromBody] SetPasswordBindingModel model)
        {
            Response oResponse = new Response();
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
            else
            {
                try
                {
                    User oUser = await _usersservice.ResetPasswordV5(model.Email);
                    if (oUser != null)
                    {
                        String ResetToken = _authservice.GetPassword(3, 4, 3);
                        //string body = string.Empty;
                        //body = "Hi " + oUser.FirstName + " " + oUser.LastName + ".<br/> " +
                        // "\n\nA password reset has been requested from the TeamTOMM App.\n\nYour reset code is: "+ ResetToken + "\n\nPlease enter this code in the App to reset your password.\n\nMany thanks\nTeamTOM Admin";
                        _usersservice.UpdateResetToken(oUser.Email, ResetToken);
                        var webRoot = _env.WebRootPath;

                        var pathToFile = _env.WebRootPath
                                + Path.DirectorySeparatorChar.ToString()
                                + "Templates"
                                + Path.DirectorySeparatorChar.ToString()
                                + "Forgot-Password.html";
                        var builder = new BodyBuilder();
                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                        {
                            builder.HtmlBody = SourceReader.ReadToEnd();
                        }

                        string messageBody = string.Format(builder.HtmlBody,
                            oUser.FirstName,
                            oUser.LastName,
                            ResetToken,
                            oUser.Email
                            );
                        bool send = await _services.SendMail(oUser.Email, _localizer["ResetPasswordSubject"].Value, messageBody);
                        oResponse.Success = true;
                        oResponse.Message = _localizer["TokenSent"].Value;
                        if (send == false)
                        {
                            oResponse.Message = _localizer["MailError"].Value;
                        }
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }
                    else
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["EmailNotFound"].Value;
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Success = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : ResetPassword" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "ResetPassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");

                    await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                    ObjectResult resuldata = new ObjectResult(oResponse);
                    return resuldata;
                }
            }
        }
    }
}