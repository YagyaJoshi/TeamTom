using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
    public class SwapUserTaskRequest
    {
        public long UserId { get; set; }
        public int SwapDayId { get; set; }
        public int CurrentDayId { get; set; }
       
    }

    public class GetSwapDaysResponse
    {
        public int IsDay { get; set; }
        public string DayText { get; set; }
    }

    public class UpdateUserCreatedAtRequest
    {
        public long UserId { get; set; }
        public DateTime new_datetime { get; set; }
    }
}
