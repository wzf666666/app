using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using wyx.Framework;

namespace WebApplication2.Controllers
{
    public class AddressInfoController : BaseController
    {
        // GET: Address
        public string GetList(string id,string state)
        {
            var list = db.GetList<AddressInfo>("select * from addressinfo where userid='"+id+ "' and state=" + state);
            return  new { result = true, data = list }.ToJson();
        }
        public string add(AddressInfo info)
        {
            if (info != null)
            {
                info.CreateDate = DateTime.Now;
                info.Id = Guid.NewGuid().ToString();
                var s=db.Add(info);
                if (s != null)
                {
                    return new { result = true, data = s }.ToJson();
                }
                else
                {
                    return new { result = false, message = "服务器异常!" }.ToJson();
                }
            }
            else
            {
                return new { result = false, message = "服务器异常!" }.ToJson(); 
            }
               
        
        }
        public string del(string id)
        {
             var res=db.Delete<AddressInfo>(id,"Id");
            if (res>0)
            {
                return new { result = true, message = "删除成功!" }.ToJson();
            }
            else
            {
                return new { result = false, message = "服务器异常!" }.ToJson();
            }


        }
    }
}