using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using wyx.Framework;

namespace WebApplication2.Controllers
{
    public class UserInfoController : BaseController
    {
        // GET: UserInfo
        public string add(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                return new { result = false, message = "服务器异常" }.ToJson();
            }
            if (string.IsNullOrEmpty(userInfo.OpenId)) {
                return new { result = false, message = "服务器异常" }.ToJson();
            }
            var s = db.GetById<UserInfo>(userInfo.OpenId, "OpenId");
            if (s != null)
            {
                var updateStr = string.Format(
                      "UPDATE userInfo SET HeadImg = '"+ userInfo.HeadImg+"',NickName='" + userInfo.NickName + "' where id= '"+s.Id+"'");
                var rowCount = db.ExcuteNonQuery(updateStr);
                var models = db.GetById<UserInfo>(s.Id, "Id");
                if (rowCount > 0)
                {
                    return new { result = true, data = models }.ToJson();
                }
                else
                {
                    return new { result = false, message = "服务器异常" }.ToJson();
                }
            }
            userInfo.Id = Guid.NewGuid().ToString();
            userInfo.CreateDate = DateTime.Now;
            var model = db.Add(userInfo);
            Log4.Log("ssss"+userInfo.OpenId);
            if (Convert.ToInt32(model) > 0) return new { result = true, data = userInfo }.ToJson();
            else
            {
                return new { result = false, message = "服务器异常!" }.ToJson();
            }
        }
        public string getlist(string id)
        {
            var s = db.GetById<UserInfo>(id, "Id");
            if (s != null)
            {

                return new { result = true, data = s }.ToJson();

            }
            else
            {
                return new { result = false, message = "服务器异常" }.ToJson();
            }
        }
        public string adds()
        {
            UserInfo u = new UserInfo();
            u.Id = Guid.NewGuid().ToString();
            u.NickName = "玩的撒谎";
            db.Add(u);
            return null;
        }
    }
}