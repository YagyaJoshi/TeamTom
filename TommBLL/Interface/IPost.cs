using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;

namespace TommBLL.Interface
{
    public interface IPost
    {
        Task<PostList> GetPostData_old(PostsPagination obj);
        Task<List<Posts>> GetPostData();
        Task<PostList> GetPostList(List<Posts> post, PostsPagination obj);
        Task<PostResponse> GetPostDataList();
        Task<PostList> GetPostDataListToApp(PostResponse postResponse, PostsPagination obj, List<BookMarkResponse> bookMarks); 
        Task<PostList> GetPostDataList_old(PostsPagination obj);  
        Task<PostData> GetPostDataById(int postId);
        Task<int> SavePostData(Posts post);
        Task<bool> SaveSyncPostData(SyncPost post); 
        Task<bool> AddPostCategory(int postId, int postSubCategoryId);           
        Task<bool> DeletePostCategory(int postId);
        Task<Boolean> UpdatePostData(Posts post);
        Task<List<PostCategoryMst>> GetPostCategoriesToApp(); 
        //List<PostCategoryMst> GetPostCategoryList(string PostId = "");   
        Task<List<PostCategoryResponse>> GetPostCategoryList(PostResponse postCategories,string PostId = "");     
        Task<List<PostSubCategoryMst>> GetFilteredCategories(PostResponse postCategories);     

        Task<List<Posts>> GetBookMark_old(int UserId, bool? IsAudio, string CategoryFilter=""); 
        Task<List<Posts>> GetBookMark(BookMarkData obj, PostResponse postResponse, List<BookMarkResponse> bookMarks); 
        Task<Boolean> UpdateBookMark(BookMarks bookMark); 

        Task<Boolean> UpdatePostAudioUrl(PostAudio post);

        Task<List<Books>> GetBooks();
        Task<bool> DeletePost(int postId);

        Task<List<BookMarkResponse>> GetUserBookMarks(int UserId);

        Task<List<CsvData>> ImportExcelsheetData(string filePath);
        
    }
}
