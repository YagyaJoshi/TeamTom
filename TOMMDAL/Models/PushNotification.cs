using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.Models
{
    public class PushNotification
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeviceToken { get; set; }
      
    }
}
