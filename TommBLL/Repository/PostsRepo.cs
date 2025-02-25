using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.Models;

namespace TommBLL.Repository
{
    public class PostsRepo : IPost
    {
        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public PostsRepo(IServices services, IConfiguration configuration)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        }
        #endregion

        public async Task<PostList> GetPostData_old(PostsPagination obj)    /// ---** not in use **----
        {
            var count = 0;
            List<Posts> post = new List<Posts>();
            PostList postList = new PostList();
            try
            {
                
                using (var cmd = new MySqlCommand("sp_GetPostData", objCon))
                {
                    await objCon.OpenAsync();
                    cmd.Parameters.AddWithValue("@pageIndex", obj.PageNumber);
                    cmd.Parameters.AddWithValue("@pageSize", obj.PageSize);
                    cmd.Parameters.AddWithValue("@search", obj.SearchText);
                    cmd.Parameters.AddWithValue("@sortOrder", obj.SortOrder);
                    cmd.Parameters.AddWithValue("@sortColumn", obj.SortColumn);

                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            count = Convert.ToInt32(reader["Count"]);
                            post.Add(new Posts()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                Length = Convert.ToInt32(reader["Length"]),
                                TagWords = JsonConvert.DeserializeObject<string[]>(Convert.ToString(reader["TagWords"])),
                                VoiceWith_No_MusicUrl = Convert.ToString(reader["AudioUrl"]),
                                ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"]),
                                AudioTiming = JsonConvert.DeserializeObject<List<AudioTime>>(reader["AudioTiming"].ToString()),
                                Published_At = Convert.ToDateTime(reader["Published_at"])


                            });
                        }
                    }
                    postList.Posts = post;
                    postList.PageSize = obj.PageSize;
                    postList.TotalCount = count;
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return postList;
        }

        public async Task<PostList> GetPostList(List<Posts> post, PostsPagination obj)
        {
            var query = post.Where(e => e.Title.Contains(obj.SearchText));

            // Sorting logic based on sortOrder and sortColumn
            if (obj.SortOrder.ToUpper() == "DESC")
            {
                switch (obj.SortColumn)
                {
                    case "Id":
                        query = query.OrderByDescending(e => e.Id);
                        break;
                    case "Title":
                        query = query.OrderByDescending(e => e.Title);
                        break;
                    case "Image":
                        query = query.OrderByDescending(e => e.Image);
                        break;
                    case "Description":
                        query = query.OrderByDescending(e => e.Description);
                        break;
                    case "VoiceWith_No_MusicUrl":
                        query = query.OrderByDescending(e => e.VoiceWith_No_MusicUrl);
                        break;
                    case "VoiceWith_MusicUrl":
                        query = query.OrderByDescending(e => e.VoiceWith_MusicUrl);
                        break;
                    case "Published_at":
                        query = query.OrderByDescending(e => e.Published_At);
                        break;
                }
            }
            else
            {
                switch (obj.SortColumn)
                {
                    case "Id":
                        query = query.OrderBy(e => e.Id);
                        break;
                    case "Title":
                        query = query.OrderBy(e => e.Title);
                        break;
                    case "Image":
                        query = query.OrderBy(e => e.Image);
                        break;
                    case "Description":
                        query = query.OrderBy(e => e.Description);
                        break;
                    case "VoiceWith_MusicUrl":
                        query = query.OrderBy(e => e.VoiceWith_MusicUrl);
                        break;
                    case "VoiceWith_No_MusicUrl":
                        query = query.OrderBy(e => e.VoiceWith_No_MusicUrl);
                        break;
                    case "Published_at":
                        query = query.OrderBy(e => e.Published_At);
                        break;
                }
            }

            int totalCount = post.Count(); // total count of number of post

            var offsets = (obj.PageNumber - 1) * obj.PageSize;
            var query1 = query.Skip(offsets).Take(obj.PageSize);

            // Execute the query and create the PostList object
            var postList = new PostList
            {
                Posts = query1.ToList(),
                PageSize = obj.PageSize,
                TotalCount = totalCount,
                PostCount = query.Count()
            };
            return postList;
        }

        public async Task<List<Posts>> GetPostData()
        {
            List<Posts> post = new List<Posts>();
            try
            {
                using (var cmd = new MySqlCommand("sp_GetPostDataV2", objCon))
                {
                    await objCon.OpenAsync();

                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            post.Add(new Posts()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                Length = Convert.ToInt32(reader["Length"]),
                                TagWords = JsonConvert.DeserializeObject<string[]>(Convert.ToString(reader["TagWords"])),
                                VoiceWith_No_MusicUrl = Convert.ToString(reader["VoiceWithNoMusicUrl"]),
                                VoiceWith_MusicUrl = Convert.ToString(reader["VoiceWithMusicUrl"]),
                                ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"]),
                                AudioTiming = JsonConvert.DeserializeObject<List<AudioTime>>(reader["AudioTiming"].ToString()),
                                Published_At = Convert.ToDateTime(reader["Published_at"])

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return post;
        }

        public async Task<PostList> GetPostDataListToApp(PostResponse postResponse, PostsPagination obj, List<BookMarkResponse> bookMarks)
        {
            var postList = new PostList();
            IEnumerable<Posts> query;

            var postIdsWithMatchingCategories = postResponse.PostCategories
                .Where(pc => obj.CategoryFilter.Contains(int.Parse(pc.PostCategoryId)))
                .Select(pc => pc.PostId)
                .ToHashSet();

            if (obj.Last15Days)
            {
                query = from post in postResponse.Posts
                        where post.Published_At >= DateTime.UtcNow.AddDays(-15) &&
                        post.Published_At <= DateTime.UtcNow &&
                        ((obj.IsAudio == null && (!string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl) &&
                                !string.IsNullOrEmpty(post.VoiceWith_MusicUrl))) ||
                              (obj.IsAudio == true &&
                               (!string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl) &&
                                !string.IsNullOrEmpty(post.VoiceWith_MusicUrl))) ||
                              (obj.IsAudio == false &&
                               !string.IsNullOrEmpty(post.VoiceWith_MusicUrl) && string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl))
                              )
                           && post.Title.ToLower().Contains(obj.SearchText.ToLower())
                           && (obj.SubCategoryId == 0 ||
                              postResponse.PostCategories.Any(pc => pc.PostId == post.Id && pc.PostCategoryId == obj.SubCategoryId.ToString()))
                           && (obj.CategoryFilter.Length == 0 || postIdsWithMatchingCategories.Contains(post.Id))
                        orderby post.Published_At descending
                        select post;
            }
            else
            {
                query = from post in postResponse.Posts
                        where ((obj.IsAudio == null && (!string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl) &&
                                !string.IsNullOrEmpty(post.VoiceWith_MusicUrl))) ||
                              (obj.IsAudio == true &&
                               (!string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl) &&
                                !string.IsNullOrEmpty(post.VoiceWith_MusicUrl))) ||
                              (obj.IsAudio == false &&
                               !string.IsNullOrEmpty(post.VoiceWith_MusicUrl) && string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl)) 
                              )
                           && post.Title.ToLower().Contains(obj.SearchText.ToLower())
                           && (obj.SubCategoryId == 0 ||
                              postResponse.PostCategories.Any(pc => pc.PostId == post.Id && pc.PostCategoryId == obj.SubCategoryId.ToString()))
                           && (obj.CategoryFilter.Length == 0 || postIdsWithMatchingCategories.Contains(post.Id)) orderby post.Published_At descending
                        select post;
            }
          
            // Apply pagination with Limit and Offset
            var offsets = (obj.PageNumber - 1) * obj.PageSize;            
                   
            var results = query.Skip(offsets).Take(obj.PageSize).ToList();      
            
            postList = new PostList
            {
              //  Posts = results.Select(p => p.post).ToList(),
                Posts = results,
                PageSize = obj.PageSize,
                TotalCount = postResponse.Posts.ToList().Count, // count of total number of posts
                PostCount = query.Count() // Count the number of posts before pagination
            };

            //  var postIds = postList.Posts.Select(p => p.Id).ToList();

            foreach (var post in postList.Posts)
            {
                post.IsBookMark = bookMarks.Any(pu => pu.PostId == post.Id);
            }

            return postList;
        }

        public async Task<PostResponse> GetPostDataList()
        {
            var count = 0;
            List<Posts> post = new List<Posts>();
            PostList postList = new PostList();
            List<PostCategoryMst> postCategoryMst = new List<PostCategoryMst>();
            List<PostCategory> postCategory = new List<PostCategory>();
            List<PostUserBookMark> postUserBookMark = new List<PostUserBookMark>();
            PostResponse postResponse = new PostResponse();
            try
            {
                using (var cmd = new MySqlCommand("sp_GetPostDataListV2", objCon))
                {
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            post.Add(new Posts()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                // Length = Convert.ToInt32(reader["Length"]),
                                // TagWords = JsonConvert.DeserializeObject<string[]>(Convert.ToString(reader["TagWords"])),
                                VoiceWith_No_MusicUrl = Convert.ToString(reader["VoiceWithNoMusicUrl"]),
                                VoiceWith_MusicUrl = Convert.ToString(reader["VoiceWithMusicUrl"]),
                                ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"].ToString()),
                                AudioTiming = JsonConvert.DeserializeObject<List<AudioTime>>(reader["AudioTiming"].ToString()),
                                Published_At = Convert.ToDateTime(reader["Published_at"])

                            });
                        }
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            postUserBookMark.Add(new PostUserBookMark
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                PostId = Convert.ToInt32(reader["PostId"]),
                                UserId = Convert.ToInt32(reader["UserId"])
                            });
                        }
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            postCategory.Add(new PostCategory
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                PostId = Convert.ToInt32(reader["PostId"]),
                                PostCategoryId = Convert.ToString(reader["PostCategoryId"])
                            });
                        }

                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            postCategoryMst.Add(new PostCategoryMst
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CategoryType = Convert.ToString(reader["CategoryType"]),
                                Value = Convert.ToString(reader["Value"]),
                                ImageUrl = Convert.ToString(reader["ImageUrl"])
                            });
                        }
                    }
                }
                postResponse.Posts = post;
                postResponse.PostCategories = postCategory;
                postResponse.PostCategoryMst = postCategoryMst;
                postResponse.PostUserBookMark = postUserBookMark;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostDataList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostDataList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return postResponse;
        }

        public async Task<PostList> GetPostDataList_old(PostsPagination obj)  // ---** not in Use **---
        {
            var count = 0;
            List<Posts> post = new List<Posts>();
            PostList postList = new PostList();
            try
            {
                
                using (var cmd = new MySqlCommand("sp_GetPostDataList", objCon))
                {
                    await objCon.OpenAsync();
                    cmd.Parameters.AddWithValue("@pageIndex", obj.PageNumber);
                    cmd.Parameters.AddWithValue("@pageSize", obj.PageSize);
                    cmd.Parameters.AddWithValue("@IsAudio", obj.IsAudio);
                    cmd.Parameters.AddWithValue("@UserId", obj.UserId);
                    cmd.Parameters.AddWithValue("@subcategoryId", obj.SubCategoryId);
                    cmd.Parameters.AddWithValue("@search", obj.SearchText);
                    cmd.Parameters.AddWithValue("@categoryFilter", obj.CategoryFilter);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            postList.TotalCount = Convert.ToInt32(reader["totalCount"]);
                            postList.PostCount = Convert.ToInt32(reader["Count"]);
                            post.Add(new Posts()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                Length = Convert.ToInt32(reader["Length"]),
                                TagWords = JsonConvert.DeserializeObject<string[]>(Convert.ToString(reader["TagWords"])),
                                VoiceWith_No_MusicUrl = Convert.ToString(reader["VoiceWithNoMusicUrl"]),
                                VoiceWith_MusicUrl = Convert.ToString(reader["VoiceWithMusicUrl"]),
                                ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"].ToString()),
                                IsBookMark = Convert.ToBoolean(reader["IsBookMark"]),
                                AudioTiming = JsonConvert.DeserializeObject<List<AudioTime>>(reader["AudioTiming"].ToString()),
                                Published_At = Convert.ToDateTime(reader["Published_at"])

                            });
                        }
                    }
                    postList.Posts = post;
                    postList.PageSize = obj.PageSize;
                    //postList.TotalCount = post.Count;
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostDataList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostDataList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return postList;
        }

        public async Task<PostData> GetPostDataById(int postId)
        {
            PostData post = new PostData();
            List<PostCategoryMst> postCategory = new List<PostCategoryMst>();
            List<PostSubCategoryMst> postCategoryMst = new List<PostSubCategoryMst>();
            try
            {
                
                using (var cmd = new MySqlCommand("sp_GetPostDataById", objCon))
                {
                    await objCon.OpenAsync();
                    cmd.Parameters.AddWithValue("PostId", postId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            post.Id = Convert.ToInt32(reader["Id"]);
                            post.Title = Convert.ToString(reader["Title"]);
                            post.Description = Convert.ToString(reader["Description"]);
                            post.Image = Convert.ToString(reader["Image"]);
                            post.Length = Convert.ToInt32(reader["Length"]);
                            post.TagWords = JsonConvert.DeserializeObject<string[]>(Convert.ToString(reader["TagWords"]));
                            post.VoiceWith_No_MusicUrl = Convert.ToString(reader["VoiceWithNoMusicUrl"]);
                            post.VoiceWith_MusicUrl = Convert.ToString(reader["VoiceWithMusicUrl"]);
                            post.ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"]);
                            post.AudioTiming = JsonConvert.DeserializeObject<List<AudioTime>>(reader["AudioTiming"].ToString());
                            post.Published_at = Convert.ToDateTime(reader["Published_at"]);
                        }
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            postCategory.Add(new PostCategoryMst
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CategoryType = Convert.ToString(reader["CategoryType"]),
                                Value = Convert.ToString(reader["Value"]),
                                ImageUrl = Convert.ToString(reader["ImageUrl"])
                            });
                        }
                    }

                    foreach (var item in postCategory.GroupBy(e => e.CategoryType))
                    {
                        postCategoryMst.Add(new PostSubCategoryMst
                        {
                            CategoryType = item.Key,
                            SubCategories = item.Select(e => new SubCategory
                            {
                                Id = e.Id,
                                Value = e.Value,
                                ImageUrl = e.ImageUrl
                            }).ToList()
                        });
                    }
                    
                    post.PostSubCategory = postCategoryMst;
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostDataById" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostDataById - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return post;
        }

        public async Task<int> SavePostData(Posts post)
        {
            int PostId = 0;
            try
            {
               
                var tagWords = JsonConvert.SerializeObject(post.TagWords);
                using (MySqlCommand cmd = new MySqlCommand("sp_SavePostData", objCon))
                {
                    cmd.Parameters.AddWithValue("@Title", post.Title);
                    cmd.Parameters.AddWithValue("@Image", post.Image);
                    cmd.Parameters.AddWithValue("@Description", post.Description);
                    cmd.Parameters.AddWithValue("@TagWords", tagWords);
                    cmd.Parameters.AddWithValue("@Length", post.Length);
                    cmd.Parameters.AddWithValue("@Published_at", post.Published_At);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            PostId = Convert.ToInt32(reader["PostId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : SavePost" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "SavePost - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return PostId;
        }

        public async Task<bool> AddPostCategory(int postId, int postSubCategoryId)
        {
            bool Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_SavePostCategory", objCon))
                {
                    cmd.Parameters.AddWithValue("@PostId", postId);
                    cmd.Parameters.AddWithValue("@PostSubCategoryId", postSubCategoryId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : AddPostCategory" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "AddPostCategory - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }        
        
        public async Task<bool> DeletePostCategory(int postId)
        {
            bool Issave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_DeletePostCategory", objCon))
                {
                    cmd.Parameters.AddWithValue("@PostId", postId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                Issave = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : DeletePostCategory" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "DeletePostCategory - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
               await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<bool> UpdatePostData(Posts post)
        {
           
            bool Issave = false;
            try
            {
                var tagWords = JsonConvert.SerializeObject(post.TagWords);
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdatePostData", objCon))
                {
                    cmd.Parameters.AddWithValue("@PostId", post.Id);
                    cmd.Parameters.AddWithValue("@Title", post.Title);
                    cmd.Parameters.AddWithValue("@Image", post.Image);
                    cmd.Parameters.AddWithValue("@Description", post.Description);
                    cmd.Parameters.AddWithValue("@TagWords", tagWords);
                    cmd.Parameters.AddWithValue("@Length", post.Length);
                    cmd.Parameters.AddWithValue("@Published_at", post.Published_At);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            var isExist = Convert.ToInt16(reader["isExist"]);
                            if (isExist == 1)
                                Issave = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdatePostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdatePostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<List<PostCategoryMst>> GetPostCategoriesToApp()
        {
            List<PostCategoryMst> postCategories = new List<PostCategoryMst>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetPostCategories", objCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            postCategories.Add(new PostCategoryMst()
                            {
                                CategoryType = Convert.ToString(reader["CategoryType"]),
                                Id = Convert.ToInt32(reader["Id"]),
                                Value = Convert.ToString(reader["Value"]),
                                ImageUrl = Convert.ToString(reader["ImageUrl"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostCategories" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostCategories - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Dispose();
                objCon.Close();
            }

            return postCategories;
        }

        public async Task<List<PostCategoryResponse>> GetPostCategoryList(PostResponse postCategory, string PostId)
        {
            List<PostCategoryResponse> response = new List<PostCategoryResponse>();
            List<PostSubCategoryMst> totalPostCategories = new List<PostSubCategoryMst>();
            List<PostSubCategoryMst> selectedPostCategories = new List<PostSubCategoryMst>();
            try
            {

                foreach (var item in postCategory.PostCategoryMst.GroupBy(e => e.CategoryType))
                {
                    totalPostCategories.Add(new PostSubCategoryMst()
                    {
                        CategoryType = item.Key,
                        SubCategories = item.Select(e => new SubCategory()
                        {
                            Id = e.Id,
                            Value = e.Value,
                            ImageUrl = e.ImageUrl
                        }).ToList()
                    });
                }
                // LINQ query to get records from the "PostCategoryMst" table and join with "PostCategory" table

                if (!string.IsNullOrEmpty(PostId))
                {
                    var joinedPostCategories = from cm in postCategory.PostCategoryMst
                                               join pc in postCategory.PostCategories
                                               on cm.Id equals int.Parse(pc.PostCategoryId)
                                               where pc.PostId == int.Parse(PostId)   // Filter based on the PostId
                                               select new
                                               {
                                                   PostCategoryMst = cm,
                                               };

                    foreach (var item in joinedPostCategories.Select(a => a.PostCategoryMst).GroupBy(e => e.CategoryType))
                    {
                        selectedPostCategories.Add(new PostSubCategoryMst()
                        {
                            CategoryType = item.Key,
                            SubCategories = item.Select(e => new SubCategory()
                            {
                                Id = e.Id,
                                Value = e.Value,
                                ImageUrl = e.ImageUrl
                            }).ToList()
                        });
                    }
                }

                response.Add(new PostCategoryResponse()
                {
                    TotalCategories = totalPostCategories,
                    SelectedCategories = selectedPostCategories
                });

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetPostCategoryList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetPostCategoryList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Dispose();
                objCon.Close();
            }

            return response;
        }

        public async Task<List<PostSubCategoryMst>> GetFilteredCategories(PostResponse postCategory)
        {
            List<PostSubCategoryMst> totalPostCategories = new List<PostSubCategoryMst>();

            MySqlDataReader reader;
            try
            {
                string[] allowedCategoryTypes = new string[] { "Mood", "Classic TOM", "Time" };

                foreach (var item in postCategory.PostCategoryMst.GroupBy(e => e.CategoryType))
                {
                    if (allowedCategoryTypes.Contains(item.Key))
                    {
                        totalPostCategories.Add(new PostSubCategoryMst()
                        {
                            CategoryType = item.Key,
                            SubCategories = item.Select(e => new SubCategory()
                            {
                                Id = e.Id,
                                Value = e.Value,
                                ImageUrl = e.ImageUrl
                            }).ToList()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetFilteredCategories" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetFilteredCategories - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Dispose();
                objCon.Close();
            }

            return totalPostCategories;
        }

        public async Task<List<Posts>> GetBookMark_old(int UserId, bool? IsAudio, string CategoryFilter = "") // -- not in use---
        {
           
            List<Posts> bookmark = new List<Posts>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetBookMark", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@IsAudio", IsAudio);
                    cmd.Parameters.AddWithValue("@categoryFilter", CategoryFilter);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bookmark.Add(new Posts
                            {
                                Id = Convert.ToInt32(reader["PostId"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                Length = Convert.ToInt32(reader["Length"]),
                                TagWords = JsonConvert.DeserializeObject<string[]>(Convert.ToString(reader["TagWords"])),
                                VoiceWith_No_MusicUrl = Convert.ToString(reader["VoiceWithNoMusicUrl"]),
                                VoiceWith_MusicUrl = Convert.ToString(reader["VoiceWithMusicUrl"]),
                                AudioTiming = JsonConvert.DeserializeObject<List<AudioTime>>(reader["AudioTiming"].ToString()),
                                ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"].ToString()),
                                IsBookMark = Convert.ToBoolean(reader["IsBookMark"]),
                                Published_At = Convert.ToDateTime(reader["Published_at"])
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetBookMark" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return bookmark;
        }

        public async Task<List<Posts>> GetBookMark(BookMarkData obj, PostResponse postResponse, List<BookMarkResponse> bookMarks)
        {
            var postBookMarkData = new List<Posts>();

            var postIdsWithMatchingCategories = postResponse.PostCategories
                .Where(pc => obj.FilteredCategories.Contains(int.Parse(pc.PostCategoryId)))
                .Select(pc => pc.PostId)
                .ToHashSet();

            var query = from post in postResponse.Posts
                        where (obj.IsAudio == null ||
                              (obj.IsAudio == true && (!string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl) ||
                              !string.IsNullOrEmpty(post.VoiceWith_MusicUrl))) ||
                              (obj.IsAudio == false && (string.IsNullOrEmpty(post.VoiceWith_No_MusicUrl) && string.IsNullOrEmpty(post.VoiceWith_MusicUrl))))
                           && (obj.FilteredCategories.Length == 0 || postIdsWithMatchingCategories.Contains(post.Id))
                        select new
                        {
                            Post = post
                        };

            // Execute the query and get the results.
            postBookMarkData = query.Select(e => e.Post).ToList();

            var bookmarkedPostIds = bookMarks.Where(bm => bm.IsBookMark).Select(bm => bm.PostId).ToHashSet();

            postBookMarkData = postBookMarkData.Where(p => bookmarkedPostIds.Contains(p.Id)).ToList();
            foreach (var post in postBookMarkData)
            {
                post.IsBookMark = bookMarks.Any(pu => pu.PostId == post.Id);
            }
            return postBookMarkData;
        }

        public async Task<bool> UpdateBookMark(BookMarks bookMark)
        {
            Boolean IsSave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateBookMark", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", bookMark.UserId);
                    cmd.Parameters.AddWithValue("@PostId", bookMark.PostId);
                    cmd.Parameters.AddWithValue("@IsBookMark", bookMark.IsBookMark);
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
                IsSave = true;

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : UpdateBookMark" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdateBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return IsSave;
        }

        public async Task<bool> UpdatePostAudioUrl(PostAudio post)
        {
            Boolean Issave = false;
            try
            {
                var audioTime = JsonConvert.SerializeObject(post.AudioTiming);
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdatePostAudioUrl", objCon))
                {
                    cmd.Parameters.AddWithValue("@PostId", post.Id);
                    cmd.Parameters.AddWithValue("@VoiceWith_No_MusicUrl", post.VoiceWith_No_MusicUrl);
                    cmd.Parameters.AddWithValue("@VoiceWith_MusicUrl", post.VoiceWith_MusicUrl);
                    cmd.Parameters.AddWithValue("@ThumbnailUrl", post.ThumbnailUrl);
                    cmd.Parameters.AddWithValue("@AudioTiming", audioTime);
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
                string ExceptionString = "Repo : UpdatePostAudioUrl" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdatePostAudioUrl - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Issave;
        }

        public async Task<List<Books>> GetBooks()
        {
            List<Books> books = new List<Books>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetBooks", objCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var amount = Convert.ToDouble(reader["Amount"]);
                            books.Add(new Books
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Image = Convert.ToString(reader["Image"]),
                                BooksUrl = Convert.ToString(reader["BooksUrl"]),
                                Amount = "£" + amount,
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetBooks" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetBooks - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return books;
        }

        public async Task<bool> DeletePost(int postId)
        {
            bool Isdeleted = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_DeletePost", objCon))
                {
                    cmd.Parameters.AddWithValue("@PostId", postId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                Isdeleted = true;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : DeletePost" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "DeletePost - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Isdeleted;
        }

        public async Task<List<BookMarkResponse>> GetUserBookMarks(int UserId)
        {
            List<BookMarkResponse> bookmark = new List<BookMarkResponse>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserBookMark", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bookmark.Add(new BookMarkResponse
                            {
                                PostId = Convert.ToInt32(reader["PostId"]),
                                IsBookMark = Convert.ToBoolean(reader["IsBookMark"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetUserBookMarks" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetUserBookMarks - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return bookmark;
        }

        public async Task<bool> SaveSyncPostData(SyncPost post) 
        {
           bool isSave = false;
            try
            {
                using (MySqlConnection objCon1 = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_InsertPostData", objCon1))
                    {
                        cmd.Parameters.AddWithValue("@Title", post.Title);
                        cmd.Parameters.AddWithValue("@Description", post.Description);
                        cmd.Parameters.AddWithValue("@ThumbnailUrl", post.Image);
                        cmd.Parameters.AddWithValue("@VoiceWithNoMusicUrl", post.VoiceWith_No_MusicUrl);
                        cmd.Parameters.AddWithValue("@VoiceWithMusicUrl", post.VoiceWith_MusicUrl);
                        cmd.Parameters.AddWithValue("@AudioTiming", post.AudioTiming);
                        cmd.Parameters.AddWithValue("@Classic_Tom", post.Classic_Tom);
                        cmd.Parameters.AddWithValue("@Category", post.Top_Category);
                        cmd.Parameters.AddWithValue("@Time", post.Time);
                        cmd.Parameters.AddWithValue("@Mood", post.Mood);
                        cmd.Parameters.AddWithValue("@Published_at", DateTime.Parse(post.Published_At));
                        cmd.CommandType = CommandType.StoredProcedure;
                        await objCon1.OpenAsync();
                        using (var reader =await cmd.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            if (reader.HasRows)
                            {
                                var PostId = Convert.ToInt32(reader["PostId"]);
                            }
                        }
                    }
                    isSave = true;
                }
            }
            catch (Exception ex)
            {
                //objCon.Close();
                //objCon.Dispose();
                string ExceptionString = "Repo : SaveSyncPostData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "SaveSyncPostData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                //objCon.Close();
                //objCon.Dispose();
            }
            return isSave;
        }

        public async Task<List<CsvData>> ImportExcelsheetData(string filePath)
        {          
            List<CsvData> csvDataList = new List<CsvData>();

            using (var reader = new StreamReader(filePath))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    await csv.ReadAsync(); // Skip the header row

                    while (csv.Read())
                    {
                        CsvData csvData = new CsvData
                        {
                            Id = csv.GetField(0),
                            Title = csv.GetField(1),
                            Description = csv.GetField(2),
                            AudioFile = csv.GetField(11),
                            ImageFile = csv.GetField(12),
                            Time = csv.GetField(8),
                            Mood = csv.GetField(9),
                            Room = csv.GetField(10)
                        };
                        csvDataList.Add(csvData);
                    }
                }
            }
            return csvDataList;
        }       
    }
}
