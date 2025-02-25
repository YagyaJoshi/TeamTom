using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.Models
{
    public partial class UserBootcampHistory
    {
        public long Id { get; set; }
        public string TasksJson { get; set; }
        public long UserId { get; set; }
        public string BootcampJobs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Int32 Status { get; set; }
    }
}
