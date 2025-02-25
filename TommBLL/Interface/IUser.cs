using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
   public interface IUser
   {
        Task<User> GetUser(LoginViewModel model);
        Task<User> GetUserProfile(long UserId);
        Task<int> RegisterUser(User model);
        Task<Boolean> UpdateUser(User model);
        Task<User> ResetPassword(string Email);
        void UpdateResetToken(string Email, string ResetToken);
        Task<User> UpdatePassword(SetPasswordModel model);
        Task<Boolean> ChangePassword(ChangePasswordBindingModel model);
        Task<Boolean> UpdateMigrateUser(Migrate omodel);
        Task<Boolean> MigrateUserData(Migrate omodel);
        Task<List<MigrateProfile>> GetAllMigrateProfile(long UserId);
        Task<Boolean> CheckVersion(string VersionName);
        Task<User> GetUserV3(LoginViewModel model);
        Task<User> GetUserV5(LoginViewModel model); 
        Task<Boolean> CheckVersionV3(string VersionName, string DeviceToken, string DeviceType, string UserId);
        Task<List<UserUpdateTaskHistory>> GetUserTasksHistory(long UserId, long year, Int32 month);
        Task<UserTaskHisV3> GetUserDetailsforNotification(Int32 WeekNumber, Int32 IsDay);
        Task<Boolean> Logout(long UserId);
        Task<Boolean> AddUserChildrensV5(Childrens model);
        Task<(int,User)> RegisterUserV5(UserV5 model);
        Task<Tuple<Boolean, Boolean>> CheckVersionV5(string VersionName, string DeviceToken, string DeviceType, string UserId);
        Task<Boolean> UpdateUserV5(User model);
        Task<User> ResetPasswordV5(string Email); 
    }
}
