using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;

namespace TommBLL.Interface
{
    public interface IUserSubscription
    {
        Task<UserSubscription> GetUserSubscription(int userId);
        Task<int> AddUserSubscription(UserSubscription userSubscription);
        Task<Boolean> CancelUserSubscription(UserSubscription userSubscription);
        Task<(bool, bool)> GoogleSubscriptionCancellation(string packageName, string subscriptionId, string token);
        Task<(bool, bool)> AppleSubscriptionCancellation(string packageName, string subscriptionId, string token);
    }  
}
