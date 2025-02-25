using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.ViewModel;

namespace TommBLL.Repository
{
    public class HolidayRepo : IHoliday
    {
        private readonly IServices _services;
        private readonly IConfiguration _configuration;
        MySqlConnection objCon;
        public HolidayRepo(IServices services, IConfiguration configuration)
        {
           
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
           
        }

        public async Task<bool> DeleteUserHoliday(DeleteHolidayViewModel model)
        {
          
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_DeleteHoliday", objCon))
                {

                    cmd.Parameters.AddWithValue("@HolidayId", model.Id);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetHolidayDetails" + Environment.NewLine + ex.StackTrace ;
                var fileName = "GetHolidayDetails - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return false;
        }

        public async Task<List<HolidayDetailsResponseViewModel>> GetHolidayDatesByMonth(long UserId, byte Month, short Year)
        {
            List<HolidayDetailsResponseViewModel> listHoliday = new List<HolidayDetailsResponseViewModel>();
           
            try
            {
                
                var firstDayOfMonth = new DateTime(Year, Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                using (MySqlCommand cmd = new MySqlCommand("sp_GetHolidayDatesByMonth", objCon))
                {

                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@Year", Year);
                    cmd.Parameters.AddWithValue("@FirstDayofMonth", firstDayOfMonth);
                    cmd.Parameters.AddWithValue("@LastDayofMonth", lastDayOfMonth);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await  cmd.ExecuteReaderAsync())
                    {

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {

                                listHoliday.Add(new HolidayDetailsResponseViewModel
                                {
                                    Id = Convert.ToInt64(reader["Id"]),
                                    UserId = Convert.ToInt64(reader["UserId"]),
                                    HasMarkedHoliday = reader["EndDate"] == DBNull.Value ? true : false,
                                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),

                                });
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetHolidayDatesByMonth" + Environment.NewLine + ex.StackTrace + UserId;
                var fileName = "GetHolidayDatesByMonth - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return listHoliday;
        }

        public async Task<HolidayDetailsResponseViewModel> GetHolidayDetails(long UserId)
        {
            HolidayDetailsResponseViewModel response = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetHolidayDetails", objCon))
                {

                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await  cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            return new HolidayDetailsResponseViewModel
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                UserId = Convert.ToInt64(reader["UserId"]),
                                HasMarkedHoliday = reader["EndDate"] == DBNull.Value ? true : false,
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),

                            };

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetHolidayDetails" + Environment.NewLine + ex.StackTrace + UserId;
        var fileName = "GetHolidayDetails - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
        await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return response;
        }

        public async Task<HolidayListResponse> GetUserHolidayList(long UserId, int PageNo, int PageSize)
        {
            HolidayListResponse response = null;
            List<HolidayData> HolidayList = new List<HolidayData>();
            long TotalCount = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserHolidayList", objCon))
                {

                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PageNo", PageNo);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);

                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        
                        while (await reader.ReadAsync())
                        {
                            TotalCount = Convert.ToInt64(reader["total"]);
                            HolidayList.Add(new HolidayData
                            {
                                Id = Convert.ToInt64(reader["HolidayId"]),
                                UserId = Convert.ToInt64(reader["UserId"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"])
                            });
                        }


                    }
                }
                if (HolidayList.Count > 0)
                    response = new HolidayListResponse { HolidayList = HolidayList, TotalCount = TotalCount };

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetHolidayDetails" + Environment.NewLine + ex.StackTrace + UserId;
                var fileName = "GetHolidayDetails - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return response;
        }

        public async Task<SetHolidayResponseViewModel> SetHolidayStatus(HolidayRequestViewModel model)
        {
            DataSet ds = new DataSet();
            SetHolidayResponseViewModel response = null;
            try
            {        
                using (MySqlCommand cmd = new MySqlCommand("sp_SetHolidayStatus", objCon))
                {

                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@CurrentDate", model.CurrentDate);
                    cmd.Parameters.AddWithValue("@HasMarkedHoliday", model.HasMarkedHoliday);
                   
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    await objCon.OpenAsync();
                    adapter.Fill(ds);

                    DataTable dtHolidayRecord = ds.Tables[0];
                    DataTable dtErrorMsg = ds.Tables[1];
                    if (dtHolidayRecord.Rows.Count > 0)
                    {
                        response = new SetHolidayResponseViewModel
                        {
                            UserId = Convert.ToInt64(dtHolidayRecord.Rows[0]["UserId"]),
                            HasMarkedHoliday = Convert.ToBoolean(model.HasMarkedHoliday),
                            Id = Convert.ToInt64(dtHolidayRecord.Rows[0]["Id"])

                        };

                    }
                    if (dtErrorMsg.Rows.Count > 0)
                    {
                        if (response != null)
                            response.ErrorMessage = Convert.ToString(dtErrorMsg.Rows[0]["ErrorMsg"]);
                        else response = new SetHolidayResponseViewModel
                        {
                            ErrorMessage = Convert.ToString(dtErrorMsg.Rows[0]["ErrorMsg"])
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : SetHolidayStatus" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "SetHolidayStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return response;
        }
    }
}
