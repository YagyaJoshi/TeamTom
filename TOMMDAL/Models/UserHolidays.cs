using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.Models
{
    public partial class UserHolidays
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
