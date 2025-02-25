using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TommDAL.Models;
using TommDAL.ViewModel;
using CryptSharp;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Hosting;
using TommBLL.Interface;
using System.Data;
using System.IO;
using MimeKit;

namespace TommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        #region Dependency injection  

        public IConfiguration _configuration { get; }
        private readonly IStringLocalizer<AccountController> _localizer;
        private IHostingEnvironment _env;
        private IAdmin _usersservice;
        private IAuth _authservice;
        private IServices _services;
        private IUser _userssservice;
        public AdminController(
            IConfiguration configuration,
            IAdmin usersservice,
            IServices services, IAuth authservice, IUser userssservice,
         IStringLocalizer<AccountController> localizer, IHostingEnvironment env)
        {
            _configuration = configuration;
            _usersservice = usersservice;
            _services = services;
            _localizer = localizer;
            _env = env;
            _authservice = authservice;
            _userssservice = userssservice;
        }

        #endregion

        /// <summary>
        /// This is api is used to login Admin
        /// </summary>
        /// <param name="omodel">LoginViewModel</param>

        [HttpPost]
        [AllowAnonymous]
        [Route("AdminLogin")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminUser omodel)
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
                User oUser = await  _usersservice.GetAdmin(omodel);
                bool matches = false;
                if (oUser != null && oUser.FirstName != null)
                {

                    matches = Crypter.CheckPassword(omodel.Password, oUser.Token);

                    if (matches)
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        Dictionary<string, object> res = new Dictionary<string, object>();
                        string Token1 = _authservice.GetToken(oUser.Id, "User");
                        oUser.Token = Token1;
                        res.Add("Success", true);
                        res.Add("Data", new
                        {
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

        [HttpGet]
        [Authorize]
        [Route("GetAllUser")]
        public async Task<IActionResult> GetAllUser(long PageNumber, long PageSize, string username, string firstname, string lastname, string email, string AppVersion, string order, string order_by)
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
                DataTable oUser = await _usersservice.GetAllUser(PageNumber, PageSize, username, firstname, lastname, email, AppVersion, order, order_by);

                Dictionary<string, object> data = new Dictionary<string, object>();
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("Success", true);
                res.Add("Data", new
                {
                    Totalcount = oUser.Rows.Count > 0 ? oUser.Rows[0]["TotalCount"] : 0,
                    User = oUser
                });
                res.Add("Message", _localizer["LoginSuccess"].Value);
                ObjectResult responsedata = new ObjectResult(res);
                return responsedata;

            }
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetAllUser" + Environment.NewLine;
                ExceptionString += "Request : " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Login - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetUserById")]
        public async Task<IActionResult> GetUserById(long UserId)
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
                User oUser =await  _usersservice.GetUserById(UserId);

                if (oUser != null && oUser.FirstName != null)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Success", true);
                    res.Add("Data", new
                    {
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
            catch (Exception ex)
            {
                oResponse.Success = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : Login" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(UserId) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "Login - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        [HttpPut]
        [Authorize]
        [Route("UpdateUserDatils")]
        public async Task<IActionResult> UpdateUserDatils([FromBody] AdminUser model)
        {
            Response oResponse = new Response();
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> res = new Dictionary<string, object>();
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
                int IsUpdate =await _usersservice.UpdateUserDatils(model);
                if (IsUpdate == 2)
                {
                    oResponse.Success = true;
                    oResponse.Message = _localizer["UserAdminSuccess"].Value;

                    var webRoot = _env.WebRootPath;

                    var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "UserDeatils.html";
                    var builder = new MimeKit.BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    string messageBody = string.Format(builder.HtmlBody,
                        model.FirstName,
                        model.LastName,
                        model.Username,
                        "",
                        model.Email
                        );
                    bool send = await _services.SendMail(model.Email, _localizer["UsernameSubject"].Value, messageBody);
                    if (send == false)
                    {
                        oResponse.Message = _localizer["MailError"].Value;
                    }
                    ObjectResult responsedata = new ObjectResult(oResponse);
                    return responsedata;
                }
                else if (IsUpdate == 1)
                {
                    oResponse.Success = false;
                    oResponse.Message = _localizer["NotExist"].Value;
                    bool send = await _services.SendMail(_configuration["Log:ErroAddress"], _localizer["NotExist"].Value, JsonConvert.SerializeObject(model));
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
                string ExceptionString = "Api : UpdateUserDatils" + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateUserDatils - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
                ObjectResult responsedata = new ObjectResult(oResponse);
                return responsedata;
            }
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(input);

            // convert the byte array to a Base64 string

            string strModified = Convert.ToBase64String(byt);
            return strModified;


        }

        /// <summary>
        /// This api is used to Reset Password
        /// </summary>
        /// <param name="model">SetPasswordBindingModel</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Authorize]
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
                        _usersservice.UpdateResetToken(model.Email, ResetToken);
                        var webRoot = _env.WebRootPath;
                        var builder = new MimeKit.BodyBuilder();
                        var pathToFile = _env.WebRootPath
                                + Path.DirectorySeparatorChar.ToString()
                                + "Templates"
                                + Path.DirectorySeparatorChar.ToString()
                                + "Forgot-Password-Admin.html";

                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                        {
                            builder.HtmlBody = SourceReader.ReadToEnd();
                        }

                        string Hexstring = ConvertStringToHex(model.Email, System.Text.Encoding.Unicode);
                        string email = _configuration["BaseUrl"] + Hexstring;
                        string messageBody = string.Format(builder.HtmlBody,
                            oUser.FirstName,
                            oUser.LastName,
                            ResetToken,
                            email
                            );
                        bool send = await _services.SendMail(model.Email, _localizer["ResetPasswordSubject"].Value, messageBody);
                        oResponse.Success = true;
                        oResponse.Message = _localizer["TokenSentAdmin"].Value;
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

        /// <summary>
        /// This api is used to change the password
        /// </summary>
        /// <param name="model">ChangePasswordBindingModel</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordAdmin model)
        {
            string NewPassword = model.NewPassword;
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

                User oUser =await _usersservice.ResetPassword(model.Email.Trim());
                if (oUser != null)
                {
                    ChangePasswordBindingModel modelA = new ChangePasswordBindingModel();
                    modelA.NewPassword = model.NewPassword;
                    modelA.ConfirmPassword = model.ConfirmPassword;
                    modelA.Email = model.Email;
                    Boolean IsChangePass = await _userssservice.ChangePassword(modelA);


                    if (!IsChangePass)
                    {
                        oResponse.Success = false;
                        oResponse.Message = _localizer["Failed"].Value;
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }
                    else
                    {

                        var webRoot = _env.WebRootPath;
                        var pathToFile = _env.WebRootPath
                                + Path.DirectorySeparatorChar.ToString()
                                + "Templates"
                                + Path.DirectorySeparatorChar.ToString()
                                + "Forgot-AdminPassword.html";
                        var builder = new BodyBuilder();
                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                        {
                            builder.HtmlBody = SourceReader.ReadToEnd();
                        }

                        string messageBody = string.Format(builder.HtmlBody,
                            oUser.FirstName,
                            oUser.LastName,
                            oUser.Username,
                            NewPassword,
                            oUser.Email);
                        bool send = await _services.SendMail(model.Email, _localizer["ForgotPasswordSubject"].Value, messageBody);
                        oResponse.Success = true;
                        oResponse.Message = _localizer["PassUpdateSuccess"].Value;
                        if (send == false)
                        {
                            oResponse.Message = _localizer["MailError"].Value;
                        }
                        ObjectResult resuldata = new ObjectResult(oResponse);
                        return resuldata;
                    }


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

                string ExceptionString = "Api : ChangePassword" + Environment.NewLine;
                ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "ChangePassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");

                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);

                ObjectResult resuldata = new ObjectResult(oResponse);
                return resuldata;
            }
        }

    }
}