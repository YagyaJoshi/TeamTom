using System;
using System.Collections.Generic;

namespace TommDAL.Models
{
    public partial class CategoryMst
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ImageSource { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Color { get; set; }
    }
}
