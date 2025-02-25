using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
    public interface IArticle
    {
        Task<int> SaveArticleData(Articles article);

        ArticleList GetArticleList(List<ArticleData> article, ArticlesPagination obj);

        Task<List<ArticleData>> GetArticleData();
        Task<List<ArticleCategoryMst>> GetArticleCategories();

        Task<bool> DeleteArticle(int articleId);

        Task<ArticleData> GetArticleDataById(int articleId);

        Task<Boolean> UpdateArticleBookMark(ArticleBookMarks articlebookMark);

        Task<ArticlesResponse> GetArticleDataList();
        Task<List<ArticleBookMarkResponse>> GetUserArticleBookMarks(long UserId);

        List<ArticleData> GetArticleBookMark( ArticlesResponse articleResponse, List<ArticleBookMarkResponse> bookMarks);


        Task<ArticleDataCategoryWiseResponse> GetArticleDataCategoryWise(List<ArticleData> articles,long userId, string searchText);

        Task<ArticleList> GetArticles(List<ArticleData> articles, long userId, int skipId, int categoryId, int pageSize = 2, int pageNumber = 1, bool isSkipThree = false);
    }
}
