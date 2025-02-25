using System;
using System.Collections.Generic;
using System.Text;
using TommDAL.Models;

namespace TommDAL.ViewModel
{
    public class TaskJobLevel
    {
        public TaskJobLevel()
        {
            ListUserTask = new List<Usertask>();
        }

        public List<Usertask> ListUserTask { get; set; }
        public UserTasksHis UserTaskHistory { get; set; }
    }
    public class Usertask
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public int LevelId { get; set; }
        public string DisplayText { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public DateTime TaskDate { get; set; }
    }
    public class UsertaskV5
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public int LevelId { get; set; }
        public int IsDay { get; set; }
        public int? WeekNumber { get; set; }
        public string DisplayTitle { get; set; }       
        public bool IsUpdateForAllLevel { get; set; }
        public string NewTask { get; set; }

    }

    public class UserTasksHis
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserTasksUpdate { get; set; }
        public DateTime TaskDate { get; set; }
        public string DisplayTitle { get; set; }
    }

    public class UserTaskHistory
    {
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public string TommorowTasksJson { get; set; }
        public DateTime TaskDate { get; set; }
        
        
    }

    public class UpdateJson
    { 
        public long Id { get; set; } 
        public string TaskJson { get; set; }
    }
}
