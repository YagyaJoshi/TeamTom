using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TommBLL.Interface
{
   public interface IServices
    {
        Task<bool> SendMail(string emailid, string subject, string body);
    }
}
