using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TommBLL.Interface;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Repository
{
    public class ArticleRepo : IArticle
    {

        #region Dependency injection  
        public IConfiguration _configuration { get; }
        MySqlConnection objCon;
        private IServices _services;
        public ArticleRepo(IServices services, IConfiguration configuration)
        {
            _configuration = configuration;
            _services = services;
            objCon = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        }
        #endregion

        public async Task<List<ArticleCategoryMst>> GetArticleCategories()
        {
            List<ArticleCategoryMst> articleCategory = new List<ArticleCategoryMst>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetArticleCategories", objCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            articleCategory.Add(new ArticleCategoryMst()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = Convert.ToString(reader["Name"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();


                string ExceptionString = "Repo : GetArticleCategoryToApp" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetArticleCategoryToApp - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Dispose();
                objCon.Close();
            }

            return articleCategory;
        }

        public ArticleList GetArticleList(List<ArticleData> article, ArticlesPagination obj)
        {
            var query = article.Where(e => e.Title.Contains(obj.SearchText));

            // Sorting logic based on sortOrder and sortColumn
            if (obj.SortOrder.ToUpper() == "DESC")
            {
                switch (obj.SortColumn)
                {
                    //case "Id":
                    //    query = query.OrderByDescending(e => e.Id);
                    //    break;
                    case "Title":
                        query = query.OrderByDescending(e => e.Title);
                        break;
                    case "Image":
                        query = query.OrderByDescending(e => e.Image);
                        break;
                    case "Description":
                        query = query.OrderByDescending(e => e.Description);
                        break;
                    case "ArticleCategoryId":
                        query = query.OrderByDescending(e => e.ArticleCategoryId);
                        break;
                    case "IsSpotlight":
                        query = query.OrderByDescending(e => e.IsSpotlight);
                        break;
                    case "Ranking":
                        query = query.OrderByDescending(e => e.Ranking);
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
                    //case "Id":
                    //    query = query.OrderBy(e => e.Id);
                    //    break;
                    case "Title":
                        query = query.OrderBy(e => e.Title);
                        break;
                    case "Image":
                        query = query.OrderBy(e => e.Image);
                        break;
                    case "Description":
                        query = query.OrderBy(e => e.Description);
                        break;
                    case "ArticleCategoryId":
                        query = query.OrderByDescending(e => e.ArticleCategoryId);
                        break;
                    case "IsSpotlight":
                        query = query.OrderByDescending(e => e.IsSpotlight);
                        break;
                    case "Ranking":
                        query = query.OrderByDescending(e => e.Ranking);
                        break;
                    case "Published_at":
                        query = query.OrderBy(e => e.Published_At);
                        break;
                }
            }

            int totalCount = article.Count(); // total count of number of post

            var offsets = (obj.PageNumber - 1) * obj.PageSize;
            var query1 = query.Skip(offsets).Take(obj.PageSize);

            // Execute the query and create the PostList object
            var articleList = new ArticleList
            {
                Articles = query1.ToList(),
                PageSize = obj.PageSize,
                TotalCount = totalCount,
                ArticleCount = query.Count()
            };
            return articleList;
        }

        public async Task<List<ArticleData>> GetArticleData()
        {
            List<ArticleData> article = new List<ArticleData>();
            try
            {
                using (var cmd = new MySqlCommand("sp_GetArticles", objCon))
                {
                    await objCon.OpenAsync();

                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            article.Add(new ArticleData()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                ArticleCategoryId = Convert.ToInt32(reader["ArticleCategoryId"]),
                                IsSpotlight = Convert.ToBoolean(reader["IsSpotlight"]),
                                Ranking = Convert.ToInt32((int)reader["Ranking"]),
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

                string ExceptionString = "Repo : GetArticleData" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetArticleData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return article;
        }
        public async Task<int> SaveArticleData(Articles article)
        {
            int ArticleId = 0;
            try
            {
                
                using (MySqlCommand cmd = new MySqlCommand("sp_InsertUpdateArticle", objCon))
                {
                    cmd.Parameters.AddWithValue("@Id", article.Id);
                    cmd.Parameters.AddWithValue("@Title", article.Title);
                    cmd.Parameters.AddWithValue("@ImageUrl", article.Image);
                    cmd.Parameters.AddWithValue("@Description", article.Description);
                    cmd.Parameters.AddWithValue("@ArticleCategoryId",article.ArticleCategoryId);
                    cmd.Parameters.AddWithValue("@IsSpotlight", article.IsSpotlight);
                    cmd.Parameters.AddWithValue("@Ranking", article.Ranking);
                    cmd.Parameters.AddWithValue("@Published_at", article.Published_At);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader.HasRows)
                        {
                            ArticleId = Convert.ToInt32(reader["ArticleId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : SaveArticleData" + Environment.NewLine + ex.Message +" "+ ex.StackTrace;
                var fileName = "SaveArticleData - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return ArticleId;
        }

        public async Task<bool> DeleteArticle(int articleId)
        {
            bool Isdeleted = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_DeleteArticle", objCon))
                {
                    cmd.Parameters.AddWithValue("@ArticleId", articleId);
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

                string Exceptionstring = "repo : DeleteArticle" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var filename = "DeleteArticle - " + System.DateTime.Now.ToString("mm-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["log:erroaddress"], filename, Exceptionstring);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return Isdeleted;
        }

        public async Task<ArticleData> GetArticleDataById(int articleId)
        {
            ArticleData article = new ArticleData();
            List<ArticleCategoryMst> articleCategory = new List<ArticleCategoryMst>();
            try
            {
               
                using (var cmd = new MySqlCommand("sp_GetArticleById", objCon))
                {
                    objCon.Open();
                    cmd.Parameters.AddWithValue("ArticleId", articleId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            article.Id = Convert.ToInt32(reader["Id"]);
                            article.Title = Convert.ToString(reader["Title"]);
                            article.Description = Convert.ToString(reader["Description"]);
                            article.Image = Convert.ToString(reader["Image"]);
                            article.ArticleCategoryId = Convert.ToInt32(reader["ArticleCategoryId"]);
                            article.IsSpotlight = Convert.ToBoolean(reader["IsSpotlight"]);
                            article.Ranking = Convert.ToInt32(reader["Ranking"]);
                            article.Published_At = Convert.ToDateTime(reader["Published_at"]);
                        }
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            articleCategory.Add(new ArticleCategoryMst
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                
                            });
                        }
                    }
                   
                }

            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();


                string ExceptionString = "Repo : GetArticleDataById" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetArticleDataById - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return article;
        }

        public async Task<bool> UpdateArticleBookMark(ArticleBookMarks articlebookMark)
        {
            Boolean IsSave = false;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_AddUpdateArticleBookMark", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", articlebookMark.UserId);
                    cmd.Parameters.AddWithValue("@ArticleId", articlebookMark.ArticleId);
                    cmd.Parameters.AddWithValue("@IsBookMarkStatus", articlebookMark.IsBookMark);
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
                string ExceptionString = "Repo : UpdateArticleBookMark" +  Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "UpdateArticleBookMark - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return IsSave;
        }

        public async Task<List<ArticleBookMarkResponse>> GetUserArticleBookMarks(long UserId)
        {
            List<ArticleBookMarkResponse> bookmark = new List<ArticleBookMarkResponse>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetUserArticleBookMark", objCon))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await objCon.OpenAsync();
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bookmark.Add(new ArticleBookMarkResponse
                            {
                                ArticleId = Convert.ToInt32(reader["ArticleId"]),
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

                string ExceptionString = "Repo : GetUserArticleBookMarks" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetUserArticleBookMarks - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return bookmark;
        }

        public List<ArticleData> GetArticleBookMark(ArticlesResponse articlesResponse, List<ArticleBookMarkResponse> bookMarks)
        {
            var articleBookMarkData = new List<ArticleData>();

            articleBookMarkData = articlesResponse.Articles;

            var bookmarkedArticleIds = bookMarks.Where(bm => bm.IsBookMark).Select(bm => bm.ArticleId).ToHashSet();

            articleBookMarkData = articleBookMarkData.Where(a => bookmarkedArticleIds.Contains(a.Id)).ToList();


            foreach (var article in articleBookMarkData)
            {
                article.IsBookMark = bookMarks.Any(au => au.ArticleId == article.Id);
            }
            return articleBookMarkData;
        }

        public async Task<ArticlesResponse> GetArticleDataList()
        {
            var count = 0;
            List<ArticleData> article = new List<ArticleData>();
            ArticleList articleList = new ArticleList();
            List<ArticleCategoryMst> articleCategoryMst = new List<ArticleCategoryMst>();
            List<ArticleUserBookMark> articleUserBookMark = new List<ArticleUserBookMark>();
            ArticlesResponse articleResponse = new ArticlesResponse();
            try
            {
                using (var cmd = new MySqlCommand("sp_GetArticleDataList", objCon))
                {
                    await objCon.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader =await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            article.Add(new ArticleData()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                Image = Convert.ToString(reader["Image"]),
                                ArticleCategoryId = Convert.ToInt32(reader["ArticleCategoryId"]),
                                IsSpotlight = Convert.ToBoolean(reader["IsSpotlight"]),
                                Ranking = Convert.ToInt32(reader["Ranking"]),
                                Published_At = Convert.ToDateTime(reader["Published_at"])

                            });
                        }
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            articleUserBookMark.Add(new ArticleUserBookMark
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ArticleId = Convert.ToInt32(reader["ArticleId"]),
                                UserId = Convert.ToInt32(reader["UserId"])
                            });
                        }
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            articleCategoryMst.Add(new ArticleCategoryMst
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = Convert.ToString(reader["Name"]),
                                
                            });
                        }
                    }
                }
                articleResponse.Articles = article;
                articleResponse.ArticleCategoryMst= articleCategoryMst;
                articleResponse.ArticleUserBookMark = articleUserBookMark;
            }
            catch (Exception ex)
            {
                objCon.Close();
                objCon.Dispose();
                string ExceptionString = "Repo : GetArticleDataList" + Environment.NewLine + ex.Message + " " + ex.StackTrace;
                var fileName = "GetArticleDataList - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                await _services.SendMail(_configuration["Log:ErroAddress"], fileName, ExceptionString);
            }
            finally
            {
                objCon.Close();
                objCon.Dispose();
            }
            return articleResponse;
        }

        public async Task<ArticleDataCategoryWiseResponse> GetArticleDataCategoryWise(List<ArticleData> articles, long userId, string searchText)
        {
            var bookMarks =await GetUserArticleBookMarks(userId);
            foreach (var article in articles)
            {
                article.IsBookMark = bookMarks.Any(au => au.ArticleId == article.Id);
            }

            var generalCategoryArticles = articles
                .Where(e => e.ArticleCategoryId == 1 && e.Published_At <= DateTime.UtcNow
                && (string.IsNullOrEmpty(searchText) || e.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(e => e.Published_At)
                .Take(3)
                .ToList();

            var rockingItCategoryArticles = articles
                .Where(e => e.ArticleCategoryId == 2 && e.Published_At <= DateTime.UtcNow && (string.IsNullOrEmpty(searchText) || e.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(e => e.Published_At)
                .Take(3)
                .ToList();

            var rankingArticles = articles
                .Where(e => e.Ranking >= 1 && e.Ranking <= 3)
                .OrderBy(e => e.Ranking)
                .ToList();

            var spotlightArticle = articles
                .Where(e => e.IsSpotlight == true)
                .FirstOrDefault();

            return new ArticleDataCategoryWiseResponse()
            {
                GeneralCategoryArticles = generalCategoryArticles,
                RockingItCategoryArticles = rockingItCategoryArticles,
                RankingArticles = rankingArticles,
                SpotlightArticle = spotlightArticle
            };

        }

        public async Task<ArticleList> GetArticles(List<ArticleData> articles, long userId, int skipId, int categoryId, int pageSize = 2, int pageNumber = 1, bool isSkipThree = false)
        {
            var bookMarks = await GetUserArticleBookMarks(userId);
            foreach (var article in articles)
            {
                article.IsBookMark = bookMarks.Any(au => au.ArticleId == article.Id);
            }

            List<ArticleData> categoryWiseArticles = new List<ArticleData>();
            if (isSkipThree)
            {
                categoryWiseArticles = articles
                   .Where(e => e.ArticleCategoryId == categoryId && e.Published_At <= DateTime.UtcNow)
                   .OrderByDescending(e => e.Published_At)
                   .Skip(3)
                   .ToList();
            }
            else
            {
                categoryWiseArticles = articles
                    .Where(e => e.ArticleCategoryId == categoryId && e.Published_At <= DateTime.UtcNow && e.Id != skipId)
                   .OrderByDescending(e => e.Published_At)
                   .ToList();
            }

            int totalCount = categoryWiseArticles.Count();

            var offsets = (pageNumber - 1) * pageSize;
            var query1 = categoryWiseArticles.Skip(offsets).Take(pageSize);

            var articleList = new ArticleList
            {
                Articles = query1.ToList(),
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return articleList;
        }
    }
}
