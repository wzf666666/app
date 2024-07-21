using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class AddressInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Sex { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddressXx { get; set; }
        public int IsMR { get; set; }
        public int State { get; set; }
        public  string  Remake{ get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }

    }
}