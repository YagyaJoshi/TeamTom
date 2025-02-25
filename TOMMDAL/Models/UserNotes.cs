using System;
using System.Collections.Generic;

namespace TommDAL.Models
{
    public partial class UserNotes
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Notes { get; set; }
        public byte IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
