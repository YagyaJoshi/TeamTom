using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
    public class HolidayListResponse
    {
        public List<HolidayData> HolidayList { get; set; }

        public long TotalCount { get; set; }
    }

    public class HolidayData
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
