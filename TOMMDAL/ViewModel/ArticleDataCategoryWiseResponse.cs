using System.Collections.Generic;
using TommDAL.Models;

namespace TommDAL.ViewModel
{
    public class ArticleDataCategoryWiseResponse
    {

        public List<ArticleData> GeneralCategoryArticles { get; set; }

        public List<ArticleData> RockingItCategoryArticles { get; set; }

        public List<ArticleData> RankingArticles { get;set; }

        public ArticleData SpotlightArticle { get; set; }

    }
}
