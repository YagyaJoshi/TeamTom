using System;
using System.Collections.Generic;
using System.Text;
using TommDAL.Models;
using TommDAL.ViewModel;
using System.Data;
using System.Threading.Tasks;

namespace TommBLL.Interface
{
    public interface IAdmin
    {
        Task<User> GetAdmin(AdminUser model);

        Task<DataTable> GetAllUser(long PageNumber, long PageSize,string username, string first_name, string last_name, string email,string AppVersion, string SortDir, string SortCol);

        Task<User> GetUserById(long UserId);

        Task<int> UpdateUserDatils(AdminUser model);
        Task<User> ResetPassword(string Email);
        void UpdateResetToken(string Email, string ResetToken);
    }
}
