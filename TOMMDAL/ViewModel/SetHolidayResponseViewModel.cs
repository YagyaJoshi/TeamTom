
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TommDAL.ViewModel
{
   public class SetHolidayResponseViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public bool HasMarkedHoliday { get; set; }

        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
