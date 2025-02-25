using System;
using System.Collections.Generic;
using System.Text;

namespace TommDAL.Models
{// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Attribute 
    {
        public string email { get; set; }
        public string full_name { get; set; }
        public bool is_monthly { get; set; }
        public string summary { get; set; }
    } 
    public class Memberships 
    {
        public List<Data> data { get; set; }     
    }

    public class Data
    {
        public Attribute attributes { get; set; }
        public string id { get; set; }
        public Relationships relationships { get; set; }
        public string type { get; set; }
    }

    public class Included
    {
        public Attribute attributes { get; set; }
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Link
    {
        public string related { get; set; }
        public string self { get; set; }
    }

    public class Relationships
    {
        public Memberships memberships { get; set; } 
    }

    public class MembershipData 
    {
        public Data data { get; set; }
        public List<Included> included { get; set; }
        public Link links { get; set; }
    }

    public class AccessToken
    {
        public string access_token { get; set; } 
        public string refresh_token { get;set; }
    }
}
