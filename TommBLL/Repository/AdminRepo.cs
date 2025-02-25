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
using System.Threading.Tasks;

namespace TommBLL.Repository
{
    public class AdminRepo : IAdmin
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public AdminRepo(IServices services, IConfiguration configuration)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        }
        #endregion



        public async Task<User> GetAdmin(AdminUser model)
        {
            User userdata = new User();

            //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetLoginAdmin", objCon))
                {
                    userdata = new User();
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            userdata.FirstName = (reader["FirstName"]).ToString();
                            userdata.LastName = (reader["LastName"]).ToString();
                            userdata.Email = (reader["Email"]).ToString();
                            userdata.Token = (reader["Token"]).ToString();
                            userdata.Id = Convert.ToInt64(reader["Id"]);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetAdmin" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "GetAdmin - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return userdata;
        }

        public async Task<DataTable> GetAllUser(long PageNumber, long PageSize, string username, string firstname, string lastname, string email,string AppVersion, string SortDir, string SortCol)
        {
            DataTable users = new DataTable();

            //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
            try
            {

                using (MySqlCommand cmd = new MySqlCommand("sp_GetAllUser", objCon))
                {
                    if(PageNumber ==0) { PageNumber = 1; }
                    if (PageSize == 0) { PageNumber = 10; }
                    PageNumber = (PageNumber - 1) * PageSize;
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
                    cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                    cmd.Parameters.AddWithValue("@keyword", "");
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@firstcol", firstname);
                    cmd.Parameters.AddWithValue("@lastcoumn", lastname);
                    cmd.Parameters.AddWithValue("@emailid", email);
                    cmd.Parameters.AddWithValue("@AppVersion", AppVersion);
                    cmd.Parameters.AddWithValue("@SortDir", SortDir);
                    cmd.Parameters.AddWithValue("@SortCol", SortCol);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    MySqlDataAdapter returnVal = new MySqlDataAdapter(cmd);
                    returnVal.Fill(users);
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetAdmin" + Environment.NewLine + ex.StackTrace;
                var fileName = "GetAdmin - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return users;
        }


        public async Task<User> GetUserById(long UserId)
        {
            User userdata = new User();

            //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
            try
            {
               
                using (MySqlCommand cmd = new MySqlCommand("GetUserById", objCon))
                {
                    userdata = new User();
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            userdata.FirstName = (reader["FirstName"]).ToString();
                            userdata.LastName = (reader["LastName"]).ToString();
                            userdata.Email = (reader["Email"]).ToString();
                            userdata.Username = (reader["Username"]).ToString();
                            userdata.Id = Convert.ToInt64(reader["Id"]);
                            userdata.Childrens = (reader["Childrens"]).ToString();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserById" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(UserId);
                var fileName = "GetAdmin - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return userdata;
        }

        public async Task<int> UpdateUserDatils(AdminUser model)
        {
            int Issave = 3;
            try
            {
              
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateUserDetails", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@Username", model.Username.Trim());
                    cmd.Parameters.AddWithValue("@Email", model.Email.Trim());
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();

                        if (reader.HasRows)
                        {
                            Issave = Convert.ToInt16(reader["IsUserExist"]);
                        }
                    }
                }
                objCon.Close();
                

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateUserDatils" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "UpdateUserDatils - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
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
                objCon.Close();
                objCon.Dispose();
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

        public async void UpdateResetToken(string Email, string ResetToken)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateResetToken", objCon))
                {

                    cmd.Parameters.AddWithValue("@Email", Email.Trim());
                    cmd.Parameters.AddWithValue("@ResetToken", ResetToken);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateResetToken" + Environment.NewLine;
                var fileName = "UpdateResetToken - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ex.StackTrace);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
        }

    }
}
