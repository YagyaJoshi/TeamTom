using System;


namespace TommDAL.Models
{
    public partial class UserChristmasHistory
    {
        public long Id { get; set; }
        public string TasksJson { get; set; }
        public long UserId { get; set; }

        public string ChristmasJobs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AddChristmasTask
    {
        public int userId { get; set; }
        public string tasksJson { get; set; }  
       // public string ChristmasJobs { get; set; }

    }
}
