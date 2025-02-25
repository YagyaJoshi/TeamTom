using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TommDAL.ViewModel
{
    public class TaskViewModel
    {
        public long UserId { get; set; }
        public int? IsDay { get; set; }
        public int? CurrentDate { get; set; }
    }
    public class TaskDayList
    {
        public long Id { get; set; }
        public string Day { get; set; }
        public int LevelId { get; set; }
    }

    public class TaskDayListV5 
    {
        public long Id { get; set; } 
        public string Day { get; set; }
        public int LevelId { get; set; }
        public bool IsFocusDay { get; set; }
        public int SortBy { get; set; }
        public int? WeekNumber { get; set; }

        public int IsDay { get; set; }

        public List<Week> Weeks { get; set; } = null;
    }

    public class Week
    {
        public long Id { get; set; }
        public string Day { get; set; }
        public int LevelId { get; set; }
        public int? WeekNumber { get; set; }
        public int IsDay { get; set; }
    }
}
