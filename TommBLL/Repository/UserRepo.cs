using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;
using Newtonsoft.Json;
using CryptSharp;
using BCrypt.Net;
using System.Net;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace TommBLL.Repository
{
    public class UserRepo : IUser
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public UserRepo(IServices services, IConfiguration configuration)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            
        }
        #endregion

        public async Task<User> GetUser(LoginViewModel model)
        {
            User userdata = new User();

            //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetLoginUser", objCon))
                {
                    userdata = new User();
                    //  model.Password = Crypter.Blowfish.Crypt(model.Password);
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    //      cmd.Parameters.AddWithValue("@token", model.Password);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            userdata.FirstName = (reader["FirstName"]).ToString();
                            userdata.LastName = (reader["LastName"]).ToString();
                            userdata.Country = (reader["Country"]).ToString();
                            userdata.Email = (reader["Email"]).ToString();
                            userdata.Token = (reader["Token"]).ToString();
                            userdata.Id = Convert.ToInt64(reader["Id"]);
                            userdata.App_Version = Convert.ToInt32(reader["App_Version"]);
                            userdata.IsMultiprofile = Convert.ToInt32(reader["IsMultiprofile"]);
                            userdata.DataSyn = Convert.ToBoolean(reader["DataSyn"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUser" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "GetUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return userdata;
        }

        public async Task<User> GetUserV5(LoginViewModel model)
        {
            User userdata = new User();
            using (MySqlConnection objCon1 = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_GetLoginUserV5", objCon1))
                    {
                        userdata = new User();
                        cmd.Parameters.AddWithValue("@username", model.Username);
                        cmd.Parameters.AddWithValue("@DeviceToken", model.DeviceToken);
                        cmd.Parameters.AddWithValue("@DeviceType", model.DeviceType);
                        cmd.CommandType = CommandType.StoredProcedure;
                        await objCon1.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            if (reader.HasRows)
                            {
                                userdata.FirstName = (reader["FirstName"]).ToString();
                                userdata.LastName = (reader["LastName"]).ToString();
                                userdata.Country = (reader["Country"]).ToString();
                                userdata.Email = (reader["Email"]).ToString();
                                userdata.Token = (reader["Token"]).ToString();
                                userdata.Id = Convert.ToInt64(reader["Id"]);
                                userdata.App_Version = Convert.ToInt32(reader["App_Version"]);
                                userdata.IsMultiprofile = Convert.ToInt32(reader["IsMultiprofile"]);
                                userdata.DataSyn = Convert.ToBoolean(reader["DataSyn"]);
                                userdata.Childrens = (reader["Childrens"]).ToString();
                                userdata.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                                userdata.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //objCon.Close();
                    //objCon.Dispose();
                    string ExceptionString = "Repo : GetUser_Version_5-new" + Environment.NewLine + ex.Message + ex.StackTrace + JsonConvert.SerializeObject(model);
                    var fileName = "GetUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
                }
            }
            //finally
            //{
            //    objCon.Close();
            //    objCon.Dispose();
            //}
            return userdata;
        }

        public async Task<User> GetUserV3(LoginViewModel model)
        {
            User userdata = new User();

            //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetLoginUserV3", objCon))
                {
                    userdata = new User();
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@DeviceToken", model.DeviceToken);
                    cmd.Parameters.AddWithValue("@DeviceType", model.DeviceType);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            userdata.FirstName = (reader["FirstName"]).ToString();
                            userdata.LastName = (reader["LastName"]).ToString();
                            userdata.Country = (reader["Country"]).ToString();
                            userdata.Email = (reader["Email"]).ToString();
                            userdata.Token = (reader["Token"]).ToString();
                            userdata.Id = Convert.ToInt64(reader["Id"]);
                            userdata.App_Version = Convert.ToInt32(reader["App_Version"]);
                            userdata.IsMultiprofile = Convert.ToInt32(reader["IsMultiprofile"]);
                            userdata.DataSyn = Convert.ToBoolean(reader["DataSyn"]);
                            userdata.Childrens = (reader["Childrens"]).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUser_Version_3" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "GetUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return userdata;
        }

        public async Task<Boolean> Logout(long UserId)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_Logout", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    Issave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : Logout" + Environment.NewLine;
                var fileName = "Logout - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<int> RegisterUser(User model)
        {
            int Issave = 3;
            long UserId = 0;
            try
            {
               // var childrens = JsonConvert.DeserializeObject(model.Childrens);
                using (MySqlCommand cmd = new MySqlCommand("sp_Registeruser", objCon))
                {
                    model.Ran = BCrypt.Net.BCrypt.GenerateSalt();
                    model.Token = Crypter.Blowfish.Crypt(model.Token);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@Token", model.Token);
                    cmd.Parameters.AddWithValue("@Ran", model.Ran);
                    cmd.Parameters.AddWithValue("@Country", model.Country);                  
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            Issave = Convert.ToInt16(reader["IsUserExist"]);
                            if (Issave == 2)
                            {
                                UserId = Convert.ToInt64(reader["UserId"]);
                            }
                        }
                    }
                }

                if (Issave == 2)
                {
                    objCon.Close();
                    objCon.Dispose();
                    Createusertask(UserId);
                }
                //using (var DB = new organizedmumContext())
                //{
                //    var result = DB.User.Where(r => r.Email == model.Email).FirstOrDefault();
                //    if (result == null)
                //    {
                //        model.Ran = BCrypt.Net.BCrypt.GenerateSalt();
                //        model.Token = Crypter.Blowfish.Crypt(model.Token);
                //        DB.User.Add(model);
                //        DB.SaveChanges();
                //        Createusertask(model.Id);
                //        Issave = 2;
                //    }
                //    else
                //    {
                //        Issave = 1;
                //    }
                //}

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : RegisterUser" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "RegisterUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();

            }
            return Issave;
        }

        public void Createusertask(long Id)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_InsertUsertask", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", Id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    cmd.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : Createusertask" + Environment.NewLine;
                var fileName = "Createusertask - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
        }

        public void CreateusertaskV5(long Id) 
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_InsertUsertaskV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", Id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    cmd.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CreateusertaskV5" + Environment.NewLine;
                var fileName = "CreateusertaskV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
        }

        public async Task<User> GetUserProfile(long UserId)
        {
            User user = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserProfile", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            user = new User();
                            user.FirstName = (reader["FirstName"]).ToString();
                            user.Username = (reader["Username"]).ToString();
                            user.LastName = (reader["LastName"]).ToString();
                            user.Country = (reader["Country"]).ToString();
                            user.Email = (reader["Email"]).ToString();
                            user.NotifyMe = Convert.ToBoolean((reader["NotifyMe"]));
                            user.Childrens = (reader["Childrens"]).ToString();
                            user.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                            user.Id = UserId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserProfile" + Environment.NewLine;
                var fileName = "GetUserProfile - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return user;

        }

        public async Task<Boolean> UpdateUser(User model)
        {
            Boolean Issave = false;
            try
            {
                int IsUserExist = 1;
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUser", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.Id);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@Country", model.Country);
                    cmd.Parameters.AddWithValue("@Email", model.Email);       
                    cmd.Parameters.AddWithValue("@NotifyMe", model.NotifyMe);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsUserExist = Convert.ToInt16(reader["IsUserExist"]);
                        }
                        if (IsUserExist == 2)
                        {
                            Issave = true;
                        }
                        else
                        {
                            Issave = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUser" + Environment.NewLine;
                var fileName = "UpdateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<User> ResetPassword(string Email) 
        {
            User Ouser = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_CheckUserEmailExist", objCon))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            Ouser = new User();
                            Ouser.FirstName = (reader["FirstName"]).ToString();
                            Ouser.LastName = (reader["LastName"]).ToString();
                            Ouser.Username = (reader["username"]).ToString();
                            Ouser.Token = (reader["Token"]).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //objCon.Close();
                //objCon.Dispose();
                string ExceptionString = "Repo : ResetPassword" + Environment.NewLine;
                var fileName = "ResetPassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Ouser;
        }

        public async Task<User> ResetPasswordV5(string Email)
        {
            User Ouser = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_CheckUserEmailExistV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            Ouser = new User();
                            Ouser.Email = (reader["Email"]).ToString();
                            Ouser.FirstName = (reader["FirstName"]).ToString();
                            Ouser.LastName = (reader["LastName"]).ToString();
                            Ouser.Username = (reader["username"]).ToString();
                            Ouser.Token = (reader["Token"]).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : ResetPasswordV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "ResetPasswordV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Ouser;
        }

        public void UpdateResetToken(string Email, string ResetToken)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateResetToken", objCon))
                {

                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@ResetToken", ResetToken);
                    cmd.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //objCon.Close();
                //objCon.Dispose();
                string ExceptionString = "Repo : UpdateResetToken-new" + Environment.NewLine + ex.Message + ex.StackTrace + "Email- " + Email + "ResetToken-" + ResetToken;
                var fileName = "UpdateResetToken - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
        }

        public async Task<User> UpdatePassword(SetPasswordModel model)
        {
            int IsPassUpdate = 3;
            User Ouser = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_ForgotPassword", objCon))
                {
                    model.Password = Crypter.Blowfish.Crypt(model.Password);
                    //  model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@resettoken", model.Token);
                    cmd.Parameters.AddWithValue("@token", model.Password);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsPassUpdate = Convert.ToInt16(reader["IsPassUpdate"]);
                            if (IsPassUpdate == 1)
                            {
                                Ouser = new User();
                                Ouser.FirstName = (reader["FirstName"]).ToString();
                                Ouser.LastName = (reader["LastName"]).ToString();
                                Ouser.Username = (reader["Username"]).ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //objCon.Close();
                //objCon.Dispose();
                string ExceptionString = "Repo : UpdatePassword-new" + Environment.NewLine + ex.Message + ex.StackTrace + "Request-" + JsonConvert.SerializeObject(model);
                var fileName = "UpdatePassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                //objCon.Close();
                //objCon.Dispose();
                if (objCon != null)
                {
                    if (objCon.State == ConnectionState.Open)
                    {
                        objCon.Close();
                    }
                    objCon.Dispose();
                }
            }
            return Ouser;
        }


        public async Task<Boolean> ChangePassword(ChangePasswordBindingModel model)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_ChangePassword", objCon))
                {
                    model.NewPassword = Crypter.Blowfish.Crypt(model.NewPassword.Trim());
                    cmd.Parameters.AddWithValue("@Email", model.Email.Trim());
                    cmd.Parameters.AddWithValue("@token", model.NewPassword.Trim());
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    Issave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : ChangePassword" + Environment.NewLine;
                var fileName = "ChangePassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }


        public async Task<Boolean> MigrateUserData(Migrate omodel)
        {
            Boolean Issave = false;
            Response oResponse = new Response();
            var getprintresponse = "";
            try
            {
                var UserName = await GetUsername(omodel.UserId);
                if (UserName != "")
                {
                    Uri requestUri = new Uri(_configuration["MigrateData:MigrateUrl"] + WebUtility.UrlEncode(UserName) + "&CurrentUserId=" + omodel.CurrentUserId);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri) as HttpWebRequest;
                    request.Method = "GET";
                    //   request.ContentType = "application/json";
                    //Stream requestStream = request.GetRequestStream();
                    //requestStream.Close();
                    HttpWebResponse responseAPI = (HttpWebResponse)request.GetResponse();
                    Stream stream = responseAPI.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    var getResponse = reader.ReadToEnd();
                    getprintresponse = getResponse;
                    oResponse = JsonConvert.DeserializeObject<Response>(getResponse);
                    if (oResponse.Success)
                    {
                        UpdateMigrateUser(omodel);
                        Issave = true;
                    }
                }
                else
                {
                    Issave = false;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : MigrateUserData" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(omodel) + getprintresponse;
                var fileName = "MigrateUserData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {

            }
            return Issave;
        }


        public async Task<Boolean> UpdateMigrateUser(Migrate omodel)
        {
            Boolean Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateMigrate", objCon))
                {
                    // cmd.Parameters.AddWithValue("@IsMigrate", omodel.IsMigrate);
                    cmd.Parameters.AddWithValue("@UserId", omodel.CurrentUserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    //if (!omodel.IsMigrate)
                    //{
                    objCon.Close();
                    objCon.Dispose();
                    Createusertask(omodel.CurrentUserId);
                    //}
                    Issave = true;
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateMigrateUser" + Environment.NewLine;
                var fileName = "UpdateMigrateUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<List<MigrateProfile>> GetAllMigrateProfile(long UserId)
        {
            List<MigrateProfile> objList = new List<MigrateProfile>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetallProfile", objCon))
                {
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new MigrateProfile()
                            {
                                UserId = Convert.ToInt64(reader["Id"]),
                                Email = reader["Email"].ToString(),
                                Username = reader["username"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetAllMigrateProfile" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetAllMigrateProfile - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objList;

        }


        public async Task<Boolean> CheckVersion(string VersionName)
        {
            Boolean IsMatch = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_CheckAppversion", objCon))
                {
                    cmd.Parameters.AddWithValue("@VersionName", VersionName);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsMatch = Convert.ToBoolean(reader["IsMatchVersion"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CheckVersion" + Environment.NewLine;
                var fileName = "CheckVersion - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return IsMatch;

        }

        public async Task<string> GetUsername(long UserId)
        {
            var username = "";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUsername", objCon))
                {
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            username = (reader["username"]).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CheckVersion" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "CheckVersion - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return username;
        }




        public async Task<Boolean> CheckVersionV3(string VersionName, string DeviceToken, string DeviceType, string UserId)
        {
            Boolean IsMatch = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_CheckAppversionV3", objCon))
                {
                    cmd.Parameters.AddWithValue("@VersionName", VersionName);
                    cmd.Parameters.AddWithValue("@DeviceToken", DeviceToken);
                    cmd.Parameters.AddWithValue("@DeviceType", DeviceType);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsMatch = Convert.ToBoolean(reader["IsMatchVersion"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CheckVersionV3" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "CheckVersionV3 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return IsMatch;
        }

        public async Task<Tuple<bool, bool>> CheckVersionV5(string VersionName, string DeviceToken, string DeviceType, string UserId)
        {
            Boolean IsMandatory = false;
            Boolean IsMatch = false;
            try
            {
                if (VersionName.Length > 10)
                {
                    var str = VersionName.Split('&');
                    if(str.Length > 0)
                    {
                        VersionName = str[0];
                        DeviceToken = str[1].Split('=')[1];
                        DeviceType = str[2].Split('=')[1];
                        UserId = str[3].Split('=')[1];
                    }
                    
                }
                using (MySqlCommand cmd = new MySqlCommand("sp_CheckAppversionV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@VersionName", VersionName);
                    cmd.Parameters.AddWithValue("@DeviceToken", DeviceToken);
                    cmd.Parameters.AddWithValue("@DeviceType", DeviceType);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsMatch = Convert.ToBoolean(reader["IsMatchVersion"]);
                            IsMandatory = Convert.ToBoolean(reader["IsMandatory"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CheckVersionV5- new" + Environment.NewLine + ex.Message + " " + ex.StackTrace + " VersionName- " + VersionName + " DeviceToken- " + DeviceToken + " DeviceType- " + DeviceType + " UserId- " + UserId;
                var fileName = "CheckVersionV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Tuple.Create(IsMatch, IsMandatory);

        }

        public async Task<UserTaskHisV3> GetUserDetailsforNotification(Int32 WeekNumber, Int32 IsDay)
        {
            UserTaskHisV3 user = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserDetailsForNotification", objCon))
                {
                    cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                    cmd.Parameters.AddWithValue("@IsDay", IsDay);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            user = new UserTaskHisV3();
                            user.Id = Convert.ToInt32(reader["Id"]);
                            user.UserId = Convert.ToInt32(reader["UserId"]);
                            user.TasksJson = (reader["TasksJson"]).ToString();
                            user.DeviceToken = (reader["DeviceToken"]).ToString();
                            user.DeviceType = (reader["DeviceType"]).ToString();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserDetailsforNotification" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetUserDetailsforNotification - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return user;

        }

        public async Task<List<UserUpdateTaskHistory>> GetUserTasksHistory(long UserId, long year, Int32 month)
        {
            List<UserUpdateTaskHistory> objList = new List<UserUpdateTaskHistory>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserTasksHistory", objCon))
                {
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    cmd.Parameters.AddWithValue("year", year);
                    cmd.Parameters.AddWithValue("month", month);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new UserUpdateTaskHistory()
                            {
                                TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                                DayTitle = reader["DayTitle"].ToString(),
                                IsComplete = Convert.ToBoolean(reader["IsComplete"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserTasksHistory" + Environment.NewLine + ex.Message + " " + ex.StackTrace + " UserId-" + UserId;
                var fileName = "GetUserTasksHistory - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objList;
        }

        public async Task<bool> AddUserChildrensV5(Childrens model)
        {
            bool isSave = false;
            try
            { 
                int IsUserExist = 1; 
                using(MySqlCommand cmd = new MySqlCommand("sp_SaveUserChildrensDataV5", objCon))
                {
                    cmd.Parameters.AddWithValue("@Id", model.UserId);
                    cmd.Parameters.AddWithValue("@Childrens", model.Children);
                    cmd.CommandType = CommandType.StoredProcedure;                
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsUserExist = Convert.ToInt16(reader["IsUserExist"]);
                        }
                        if (IsUserExist == 2)
                        {
                            isSave = true;
                        }
                        else
                        {
                            isSave = false;
                        }
                    }
                }             
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : AddChildrensV5" + Environment.NewLine;
                var fileName = "AddChildrensV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return isSave;
        }

        public async Task<(int, User)> RegisterUserV5(UserV5 model)
        {
            int Issave = 3;           
            User user = null;
            try
            {                
                using (MySqlCommand cmd = new MySqlCommand("sp_RegisterUserV5", objCon))
                {
                   
                    var Ran = BCrypt.Net.BCrypt.GenerateSalt();
                    model.Password = Crypter.Blowfish.Crypt(model.Password);
                    cmd.Parameters.AddWithValue("@Username", model.Email);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@FirstName", model.Name);           
                    cmd.Parameters.AddWithValue("@Token", model.Password);
                    cmd.Parameters.AddWithValue("@Ran", Ran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            Issave = Convert.ToInt16(reader["IsUserExist"]);
                            if (Issave == 2)
                            {
                                user = new User
                                {
                                    Id = Convert.ToInt64(reader["Id"]),
                                    Username = (reader["UserName"]).ToString(),
                                    FirstName = (reader["FirstName"]).ToString(),
                                    Email = (reader["Email"]).ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                                };
                            }
                        }
                    }
                }
                if (Issave == 2)
                {
                    objCon.Close();
                    objCon.Dispose();
                    CreateusertaskV5(user.Id);
                }             

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : RegisterUserV5" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "RegisterUserV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();

            }

            return (Issave, user);
        }
        public async Task<Boolean> UpdateUserV5(User model) 
        {
            Boolean Issave = false;
            try
            {
                int IsUserExist = 1;
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserV5", objCon))
                {
                    model.Token= Crypter.Blowfish.Crypt(model.Token);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);             
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@UserId", model.Id);
                    cmd.Parameters.AddWithValue("@Childrens", model.Childrens);
                    cmd.Parameters.AddWithValue("@Token", model.Token);
                    cmd.Parameters.AddWithValue("@NotifyMe", model.NotifyMe);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            IsUserExist = Convert.ToInt16(reader["IsUserExist"]);
                        }
                        if (IsUserExist == 2)
                        {
                            Issave = true;
                        }
                        else
                        {
                            Issave = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserV5" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdateUserV5 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }
    }
}
