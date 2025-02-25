using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
   public class Migrate
    {
        public Boolean IsMigrate { get; set; }
        public long UserId { get; set; }
        public long CurrentUserId { get; set; }
    }
}
