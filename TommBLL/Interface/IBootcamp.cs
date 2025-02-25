using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;

namespace TommBLL.Interface
{
    public interface IBootcamp
    {
        Task<UserBootcampHistory> GetBootcampList(long UserId);
        Task<UserBootcampHistory> GetBootcampListV5(long UserId); 
        Task<Boolean> UpdateBootcamp(UserBootcampHistory model);
        Task<Boolean> ResetBootcamp(long UserId);
        Task<Boolean> UpdateBootCampExtraDays(UserBootcampHistory model);
        Task<Boolean> UpdateBootcampV5(UserBootcampHistory model);
    }
}
