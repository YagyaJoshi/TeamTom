using System;
using System.Collections.Generic;

namespace TommDAL.Models
{
    public partial class UserTasksHistory
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserTasksUpdate { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
