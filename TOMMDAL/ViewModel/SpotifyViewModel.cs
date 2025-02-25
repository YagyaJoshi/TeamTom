using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.ViewModel
{
   public class SpotifyViewModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
