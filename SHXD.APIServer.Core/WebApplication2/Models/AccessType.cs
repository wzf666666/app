using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class AccessType
    {
        public  string access_token { get; set; }
        public int expires_in { get; set; }
    }
}