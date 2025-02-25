using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
    public class TaskJobLevelV2
    {
        public TaskJobLevelV2()
        {
            ListUserTask = new List<UsertaskV2>();
        }

        public List<UsertaskV2> ListUserTask { get; set; }
        public UserTaskHisV2 UserTaskHistory { get; set; }
    }
    public class TaskJobLevelV5
    {
        public TaskJobLevelV5()
        {
            ListUserTask = new List<UsertaskV2>();
        }

        public List<UsertaskV2> ListUserTask { get; set; }
        public List<UserTaskHisV5> UserTaskHistory { get; set; }
    }


    public class UsertaskV2
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public int LevelId { get; set; }
        public string FocusTitle { get; set; }
        public string DisplayText { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public DateTime TaskDate { get; set; }
    }


    public class UserTaskHisV2
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserTasksUpdate { get; set; }
        public DateTime TaskDate { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public string Level1Title { get; set; }
        public string Level2Title { get; set; }
        public string FocusTitle { get; set; }
    }
    public class UserTaskHisV5
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public int LevelId { get; set; }
        public string FocusTitle { get; set; }
        public string DisplayText { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public DateTime TaskDate { get; set; }
    }

    public class UserUpdateTaskJobV2
    {
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public string TommorowTasksJson { get; set; }
        public DateTime TaskDate { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public string FocusDayTitle { get; set; }
        public string FocusTomorrowTitle { get; set; }
        public string Level1Title { get; set; }
        public string Level2Title { get; set; }
        public string TomorrowDisplayTitle { get; set; }
        public string TomorrowDayTitle { get; set; }
        public string TomorrowLevel1Title { get; set; }
        public string TomorrowLevel2Title { get; set; }
    }

    public class UserUpdateTaskJobV4
    {
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public string FutureDayTasksJson { get; set; }
        public DateTime TaskDate { get; set; }

        public DateTime FutureDate { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public string FocusDayTitle { get; set; }
        public string FocusFutureDayTitle { get; set; }
        public string Level1Title { get; set; }
        public string Level2Title { get; set; }
        public string FutureDayDisplayTitle { get; set; }
        public string FutureDayTitle { get; set; }
        public string FutureDayLevel1Title { get; set; }
        public string FutureDayLevel2Title { get; set; }
    }


    public class UserUpdateTaskJobV3
    {
        public long UserId { get; set; }
        public string TasksJson { get; set; }
        public string TommorowTasksJson { get; set; }
        public DateTime TaskDate { get; set; }
        public string DisplayTitle { get; set; }
        public string DayTitle { get; set; }
        public string FocusDayTitle { get; set; }
        public string FocusTomorrowTitle { get; set; }
        public string Level1Title { get; set; }
        public string Level2Title { get; set; }
        public string TomorrowDisplayTitle { get; set; }
        public string TomorrowDayTitle { get; set; }
        public string TomorrowLevel1Title { get; set; }
        public string TomorrowLevel2Title { get; set; }
        public Boolean IsComplete { get; set; }
    }


    public class UserUpdateTaskHistory
    {
        public DateTime TaskDate { get; set; }
        public string DayTitle { get; set; }
        public Boolean IsComplete { get; set; }
    }



    public class UserWeekFocusJobV2
    {
        public long UserId { get; set; }
        public long TaskId { get; set; }
        public string SetDayL3 { get; set; }
        public string SetDayL2 { get; set; }
        public string SetL3query { get; set; }
        public string SetL2query { get; set; }
    }


    public class UserTaskHisV3
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime TaskDate { get; set; }
        public string DayTitle { get; set; }
        public string LevelId { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public string TasksJson { get; set; }
        public Boolean NotifyMe { get; set; }
        
    }   

}
