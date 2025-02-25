using System;
using System.Collections.Generic;

namespace TommDAL.Models
{
    public partial class TaskMst
    {
        public int Id { get; set; }
        public string DayText { get; set; }
        public string TasksJson { get; set; }
        public byte IsMonday { get; set; }
        public byte IsTuesday { get; set; }
        public byte IsWednesday { get; set; }
        public byte IsThursday { get; set; }
        public byte IsFriday { get; set; }
        public byte IsSaturday { get; set; }
        public byte IsSunday { get; set; }
        public int? WeekNumber { get; set; }
        public int LevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
