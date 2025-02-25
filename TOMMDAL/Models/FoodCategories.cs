using System;
using System.Collections.Generic;

namespace TommDAL.Models
{
    public partial class FoodCategories
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Categories { get; set; }
        public string JetpackFeaturedMediaUrl { get; set; }
        public string Link { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
