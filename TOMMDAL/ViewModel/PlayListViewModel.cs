using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
    public class PlayListViewModel
    {
        public string href { get; set; }
        public List<items> items { get; set; }
    }
    public class items
    {
        public string href { get; set; }
        public string name { get; set; }
        public external_urls external_urls { get; set; }
        public List<images> images { get; set; }
        public string uri { get; set; }
    }
    public class external_urls
    {
        public string spotify { get; set; }
    }
    public class images
    {
        public string height { get; set; }
        public string url { get; set; }
        public string width { get; set; }
    }
}


