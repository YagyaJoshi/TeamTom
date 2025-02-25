using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.Models;

namespace TommBLL.Repository
{
   public class NotesRepo : INotes
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public NotesRepo(IConfiguration configuration, IServices services)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        #endregion

        public async Task<UserNotes> GetUserNotes(long UserId)
        {
            UserNotes ObjNotes = null;
            try
            {

                                //List<UserTasksMst> userTasksMst = new List<UserTasksMst>();
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserNotes", objCon))
                {
                   
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var dr =await cmd.ExecuteReaderAsync())
                    {
                        await dr.ReadAsync();
                        if (dr.HasRows)
                        {
                            ObjNotes = new UserNotes();
                            ObjNotes.Id = Convert.ToInt64(dr["Id"]);
                            ObjNotes.UserId = Convert.ToInt64(dr["UserId"]);
                            ObjNotes.Notes = (dr["Notes"]).ToString();
                        }
                    }          
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserNotes" + Environment.NewLine + ex.StackTrace + UserId;
                var fileName = "GetUserNotes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return ObjNotes;
        }

        public async Task<Boolean> SaveNotes(UserNotes model)
        {
            Boolean Issave = false;
            try
            {
                MySqlCommand cmd = new MySqlCommand("sp_SaveUserNotes", objCon);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@Notes", model.Notes);
                cmd.CommandType = CommandType.StoredProcedure;
                await objCon.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                Issave = true;
                
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : SaveNotes" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "SaveNotes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
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
