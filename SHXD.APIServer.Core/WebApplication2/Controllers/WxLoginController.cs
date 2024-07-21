using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using wyx.Framework;
using static System.Net.WebRequestMethods;

namespace WebApplication2.Controllers
{
    public class WxLoginController : Controller
    {
        // GET: WxLogin
        public string Login(string code)
        {

            var s=wyx.Framework.Utils.HttpVisit.HttpPost("https://api.weixin.qq.com/wxa/business/getuserphonenumber?access_token=ACCESS_TOKEN", "application/json", new { code = code }.ToJson());
            return s.ToJson();
        }
        public string GetAccesStoken(string code)
        {
            var appid = "wxa040bc4f960c0a9c";
            var secret = "31854ebc580326df1ffe260ccd6d7e94";
            var url = @"https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + secret + "&js_code=" + code + "&grant_type=authorization_code"; 
            var s = wyx.Framework.Utils.HttpVisit.HttpGet(url);
            var model=JsonHelp.ToObject<AccessType>(s);
            return s;
        }
    }
}