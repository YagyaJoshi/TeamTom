using System;
using System.Collections.Generic;
using System.Text;

namespace TommBLL.Interface
{
  public  interface IAuth
    {
        string GetToken(long userid, string role);
        string GetPassword(int lowercase, int uppercase, int numerics);
    }
}
