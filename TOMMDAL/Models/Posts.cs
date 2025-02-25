using System;
using System.Collections.Generic;
using System.Text;
using TommDAL.Models;

namespace TommDAL.Models
{
    public class Posts 
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string[] TagWords { get; set; }

        public int Length { get; set; }

        public DateTime Published_At { get; set; }
        public int[] SubCategoryId { get; set; }
        public string VoiceWith_No_MusicUrl { get; set; }   
        public string VoiceWith_MusicUrl { get; set; }   
        public List<AudioTime> AudioTiming { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsBookMark { get; set; }

    }
    public class PostList
    {
        public List<Posts> Posts { get; set; } 
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PostCount { get; set; } 
    }
    public class PostsPagination 
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool? IsAudio { get; set; }
        public int[] CategoryFilter { get; set; }  
        public int? UserId { get; set; }
        public int? SubCategoryId { get; set; }
        public string SearchText { get; set; } 
        public string SortOrder { get; set; }
        public string SortColumn { get; set; } 
        public bool Last15Days { get; set; }       
       
    }
    public class PostData
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string[] TagWords { get; set; }

        public int Length { get; set; }
        public DateTime Published_at { get; set; }
        public string VoiceWith_No_MusicUrl { get; set; }
        public string VoiceWith_MusicUrl { get; set; }
        public List<AudioTime> AudioTiming { get; set; }
        public string ThumbnailUrl { get; set; }
        public List<PostSubCategoryMst> PostSubCategory { get; set; } 

    }
    public class PostCategoryMst  
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string CategoryType { get; set; }
        public string ImageUrl { get; set; }
    }
    public class PostSubCategoryMst    
    {
        public string CategoryType { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }

    public class SubCategory
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string ImageUrl { get; set; }
    }  
    public class PostCategoryResponse 
    {
        public List<PostSubCategoryMst> TotalCategories { get; set; }
        public List<PostSubCategoryMst> SelectedCategories { get; set; }

    }
    public class BookMarks 
    {      
        public string PostId { get; set; }
        public int UserId { get; set; }
        public bool IsBookMark { get; set; }
    }
    public class BookMarkData  
    {
        public int[] FilteredCategories { get; set; }  
        public int UserId { get; set; }
        public bool? IsAudio { get; set; } 
    }

    public class BookMarkResponse
    {
        public int PostId { get; set; }

        public bool IsBookMark { get; set; }
    }
    public  class PostAudio
    {
        public int Id { get; set; }
        public string VoiceWith_No_MusicUrl { get; set; }
        public string VoiceWith_MusicUrl { get; set; }
        public List<AudioTime> AudioTiming { get; set; }
        public string ThumbnailUrl { get; set; }
    }
    public class AudioTime
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class PostCategory  
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string PostCategoryId { get; set; }
    }

    public class PostUserBookMark
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
    }


    public class PostResponse
    {
        public List<Posts> Posts { get; set; }

        public List<PostCategory> PostCategories { get; set; }

        public List<PostCategoryMst> PostCategoryMst { get; set; } 
        public List<PostUserBookMark> PostUserBookMark { get; set; }

    }
 
    public class SyncPost  
    {   
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }        
        public string VoiceWith_No_MusicUrl { get; set; }
        public string VoiceWith_MusicUrl { get; set; }
        public string AudioTiming { get; set; }
        public string Classic_Tom { get; set; }  
        public string Top_Category { get; set; } 
        public string Mood { get; set; }
        public string Time { get; set; } 
        public string Published_At { get; set; } 
    }

    public class CsvData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AudioFile { get; set; }
        public string ImageFile { get; set; }
        public string Time { get; set; }
        public string Mood { get; set; }
        public string Room { get; set; } 
    }

    public class Credentials 
    {
        public string type { get; set; }
        public string project_id { get; set; }
        public string private_key_id { get; set; }
        public string private_key { get; set; }
        public string client_email { get; set; }
        public string client_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_x509_cert_url { get; set; }
        public string universe_domain { get; set; }
    }

}
