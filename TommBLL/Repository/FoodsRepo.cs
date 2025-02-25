using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Repository
{
   public class FoodsRepo : IFoods
    {
       

        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public FoodsRepo(IConfiguration configuration, IServices services)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<CategoryMst>> CategoryList()
        {
           List<CategoryMst> objList = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetCategoryList", objCon))
                {
                    objList = new List<CategoryMst>();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new CategoryMst()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = (reader["Title"]).ToString(),
                                ImageSource = (reader["ImageSource"]).ToString(),
                                Color = (reader["Color"]).ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : CategoryList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "CategoryList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objList;
        }

        public async Task<List<FoodView>> GetFoodData(int CategoryId)
        {
            List<FoodView> objList = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetFoodUrl", objCon))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                    objList = new List<FoodView>();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objList.Add(new FoodView()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = (reader["Title"]).ToString(),
                                Link = (reader["Link"]).ToString(),
                                Jetpack_featured_media_url = (reader["Jetpack_featured_media_url"]).ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetFoodData"+ Environment.NewLine + ex.StackTrace + CategoryId;
                var fileName = "GetFoodData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return objList;
        }


        public async Task<Boolean> UpdateFood(FoodCategories model)
        {
            Boolean Issave = false;
            try
            {
                string test = JsonConvert.SerializeObject(model);
                using (MySqlCommand cmd = new MySqlCommand("sp_SaveFood", objCon))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Title", model.Title);
                    cmd.Parameters.AddWithValue("@Categories", model.Categories);
                    cmd.Parameters.AddWithValue("@JetpackFeaturedMediaUrl", model.JetpackFeaturedMediaUrl);
                    cmd.Parameters.AddWithValue("@PostDate", model.PostDate);
                    cmd.Parameters.AddWithValue("@Link", model.Link);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateFood" + Environment.NewLine + ex.StackTrace + JsonConvert.SerializeObject(model);
                var fileName = "UpdateFood - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
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
