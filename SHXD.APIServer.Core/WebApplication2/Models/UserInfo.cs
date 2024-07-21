using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class UserInfo
    {
        public string Id { get; set; }
        public  string NickName { get; set; }
        public string HeadImg { get; set; }
        public int Sex { get; set; }
        public string Phone { get; set; }
        public int State { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime CreateDate { get; set; }
        public string OpenId { get; set; }
    }
}