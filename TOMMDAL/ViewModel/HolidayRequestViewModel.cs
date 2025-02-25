using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
   public class HolidayRequestViewModel
    {
        public long UserId { get; set; }

        public DateTime CurrentDate { get; set; }

        public bool HasMarkedHoliday { get; set; }
    }
}
