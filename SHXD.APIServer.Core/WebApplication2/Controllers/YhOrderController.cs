using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using wyx.Framework;

namespace WebApplication2.Controllers
{
    public class YhOrderController : BaseController
    {
        public string add(YhOrder order)
        {
            if (order == null)
            {
                return new { result = false, message = "服务器异常" }.ToJson();
            }
            if (string.IsNullOrEmpty(order.UserId))
            {
                return new { result = false, message = "服务器异常" }.ToJson();
            }
            
            order.Id = Guid.NewGuid().ToString();
            order.CreateDate = DateTime.Now;
            order.Number = 1;
            order.State = 0;
            order.GoodId = "1";
            order.Qm = order.Id.Substring(0, 5);
            var model = db.Add(order);
            if (Convert.ToInt32(model) > 0) return new { result = true, data = order }.ToJson();
            else
            {
                return new { result = false, message = "服务器异常!" }.ToJson();
            }
        }
        public string getlist(string uid,int state)
        {
            var list = db.GetList<YhOrder>("select * from YhOrder where userid='" + uid + "' and state="+state);
            return new { result = true, data = list }.ToJson();
        }
        public string Hx(string qm)
        {
            var s=db.GetById<YhOrder>(qm,"Qm");
            if(s==null) return new { result = false, message = "券码不存在!" }.ToJson();
            if(s.State!=0) return new { result = false, message = "券码已被核销或已过期!" }.ToJson();
            var updateStr = string.Format(
                       "UPDATE YhOrder SET State = 1 WHERE Id = '" + s.Id + "'");
            var rowCount = db.ExcuteNonQuery(updateStr);
            Log4.Log(updateStr);
            if (rowCount > 0)
            {
                return new { result = true, message = "核销完成!" }.ToJson();
            }
            return new { result = false, message = "服务器异常!" }.ToJson(); ;
        }
    }
}