using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
    public interface IJobs
    {
        Task<TaskJobLevel> GetUserJobsByDay(long UserId, int IsDay, DateTime CurrentDate,int WeekNumber);
        Task<List<TaskDayList>> GetUserDayList(); 
        Task<Usertask> GetUserDayJob(long UserId, int TaskId);
        Task<Boolean> UpdateTaskJob(Usertask model);
        Task<Boolean> RestoreDayTasks(long UserId, int TaskId);
        Task<Boolean> UpdateUserDayJobs(UserTaskHistory model);
        Task<Boolean> UpdateUserRoolOverDay(UserTaskHistory model);
        Task<Boolean> DeleteUserHistoryDayJobs(long UserTaskHistoryId);
        Task<TaskJobLevel> GetNextDayJob(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber);
        Task<string> GetDayTitle(int WeekNumber, int IsDay,long UserId);
        Task<Boolean> SaveSwapDayJob(long UserId, long CurrentTaskId, long SwapTaskId);
        Task<TaskJobLevelV2> GetUserNewJobsByDayV2(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber);
 
        Task<TaskJobLevelV2> GetNextDayJobV2(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber);
        Task<TaskJobLevelV2> GetNextDayJobV3(long UserId, int IsDay, DateTime CurrentDate, DateTime FutureDate, int WeekNumber);
        
        Task<Boolean> UpdateUserDayJobsV2(UserUpdateTaskJobV2 model);
        Task<Boolean> UpdateUserRoolOverDayV2(UserUpdateTaskJobV2 model);
        Task<Boolean> UpdateUserWeekFocusV2(UserWeekFocusJobV2 model);
        Task<List<TaskDayList>> GetUserDayListV2(long UserId);
        Task<string> GetDayTitleV2(int WeekNumber, int IsDay, long UserId);
        Task<Boolean> RestoreDayTasksV2(long UserId, int TaskId);

        Task<Tuple<Boolean, Int64>> UpdateUserDayJobsV3(UserUpdateTaskJobV3 model);

        Task<Tuple<Boolean, Int64>> UpdateUserDayJobsV4(UserUpdateTaskJobV3 model);

        Task<Boolean> UpdateUserDayJobsV3(UserUpdateTaskJobV2 model);

        Task<Boolean> UpdateUserDayJobsV4(UserUpdateTaskJobV4 model);
        Task<Boolean> UpdateUserRoolOverDayV3(UserUpdateTaskJobV2 model);

        Task<Boolean> UpdateUserRoolOverDayV4(UserUpdateTaskJobV4 model);

        Task<Boolean> UpdateUserDayJobsV5(UserUpdateTaskJobV3 model);
        Task<Int64> GetUserDayStreakV5(string UserId, DateTime TaskDate);

        TaskJobLevelV2 GetNextDayJobV5(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber); 
        Task<TaskJobLevelV5> GetUserNewJobsByDayV5(long UserId, int IsDay, DateTime CurrentDate, int WeekNumber);
        Task<List<TaskDayListV5>> GetUserDayListV5(long UserId);
        Task<List<Usertask>> GetUserDayJobV5(long UserId, int IsDay, int? WeekNumber);

        Task<Boolean> UpdateUserWeekFocusV5(UserWeekFocusJobV2 model); 
        Task<Boolean> UpdateTaskJobV5(UsertaskV5 model);
        Task<Boolean> BulkUpdateTaskJobV5(UsertaskV5 model);
        Boolean BulkUpdateTaskJob(List<UpdateJson> updateJsons, long UserId, long LevelId);
        Task<Boolean> RestoreDayTasksV5(long UserId);

        Task<Boolean> SwapUserTaskV5(SwapUserTaskRequest model);

        Task<List<GetSwapDaysResponse>> GetSwapDaysV5(long UserId);
        Task<Boolean> UpdateUserCreatedAt(UpdateUserCreatedAtRequest input);
    }
}
