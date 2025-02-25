using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
   public class HolidayDetailsResponseViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool HasMarkedHoliday { get; set; }
    }
}
