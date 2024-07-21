using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wyx.Framework.DBHelper;

namespace WebApplication2.Controllers
{
    public class BaseController : Controller
    {
        string mysqlconstr = "";
        public MySqlDBHelper db= new wyx.Framework.DBHelper.MySqlDBHelper("server=8.130.37.45;Database=xixie;Uid=root;Pwd=123456;Port=3306;Allow User Variables=True;SslMode=None;charset=utf8;");
    }
}