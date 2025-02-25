using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TommDAL.Models
{
    public class Articles
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]

        public string Description { get; set; }

        [Required]

        public string Image { get; set; }

        [Required]

        public int ArticleCategoryId { get; set; }

        public bool IsSpotlight { get; set; }

        public int Ranking { get; set; }

        public DateTime Published_At { get; set; }
          
    }
    public class ArticleList
    {
        public List<ArticleData> Articles { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int ArticleCount { get; set; }
    }

    public class ArticlesPagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
       // public bool? IsAudio { get; set; }
       /// <summary>
       //public int[] CategoryFilter { get; set; }
       /// </summary>
        //public int? UserId { get; set; }
        //public int? SubCategoryId { get; set; }
        public string SearchText { get; set; }
        public string SortOrder { get; set; }
        public string SortColumn { get; set; }
        //public bool Last15Days { get; set; }

    }

    public class ArticleData
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public int ArticleCategoryId { get; set; }

        public bool IsSpotlight { get; set; }

        public int Ranking { get; set; }

        public DateTime Published_At { get; set; }

        public bool IsBookMark { get; set; }

    }
    public class ArticleCategoryMst
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ArticleBookMarks
    {

        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public bool IsBookMark { get; set; }
    }
    

    public class ArticleBookMarkResponse
    {
        public int ArticleId { get; set; }

        public bool IsBookMark { get; set; }
    }

    public class ArticleUserBookMark
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int UserId { get; set; }
    }

    public class ArticlesResponse
    {
        public List<ArticleData> Articles { get; set; }

        public List<ArticleCategoryMst> ArticleCategoryMst { get; set; }
        public List<ArticleUserBookMark> ArticleUserBookMark { get; set; }

    }
}
