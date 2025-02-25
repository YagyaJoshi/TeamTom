using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
    public interface IChritmas
    {
        Task<UserChristmasHistory> GetChritmasList(long UserId);
        Task<UserChristmasHistory> GetChritmasListV5(long UserId); 
        Task<Boolean> UpdateChritmas(UserChristmasHistory model);
        Task<Boolean> UpdateChritmasV5(UserChristmasHistory model);  

        Task<Boolean> AddChritmasTasksV5(AddChristmasTask model);  
    }
}
