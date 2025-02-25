using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.Models
{
    public class UserSubscription
    {
        public int Id { get; set; } 
        public int userId { get; set; } 
        public string packageName { get; set; }
        public string subscriptionId { get; set; }
        public string token { get; set; }
        public string platform { get; set; }
        public string status { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class CancelSubscription 
    {
        public string packageName { get; set; }
        public string subscriptionId { get; set; }
        public string token { get; set; }
    }
    
}
