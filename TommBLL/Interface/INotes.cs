using System;
using System.Threading.Tasks;
using TommDAL.Models;

namespace TommBLL.Interface
{
   public interface INotes
    {
        Task<UserNotes> GetUserNotes(long UserId);
        Task<Boolean> SaveNotes(UserNotes model);

    }
}
