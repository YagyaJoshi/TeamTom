using System;
using System.Collections.Generic;

namespace TommDAL.Models
{
    public partial class LevelMst
    {
        public int Id { get; set; }
        public string LevelName { get; set; }
        public string DisplayText { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
