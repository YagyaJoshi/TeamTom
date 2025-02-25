using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.Models
{
   public class Response
    {
        public Boolean Success { get; set; }

        public string Message { get; set; }

        public dynamic data { get; set; }
        //  public Object data { get; set; }
    }
}
