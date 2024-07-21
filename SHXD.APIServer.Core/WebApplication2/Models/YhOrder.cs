using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class YhOrder
    {
        public string Id { get; set; }
        public string GoodId { get; set; }
        public int Number { get; set; }
        public double Price { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int State { get; set; }
        public string Qm { get; set; }             
    }
}