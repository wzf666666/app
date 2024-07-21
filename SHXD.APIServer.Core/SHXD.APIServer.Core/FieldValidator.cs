using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wyx.Framework;

namespace SHXD.APIServer.Core
{
    public static class FieldValidator
    {
        public static ValidateResult TestRequire(this ValidateItem item, object value)
        {
            if (value == null) return new ValidateResult(false,item.GetRequireErrorMessage());
            if (string.IsNullOrEmpty(value.ToString().Trim())) return new ValidateResult(false, item.GetRequireErrorMessage());
            return new ValidateResult(true);
        }

        public static string GetRequireErrorMessage(this ValidateItem item)
        {
            if (string.IsNullOrEmpty(item.Msg)) return item.Name + "：不能为空";
            else return item.Msg.Replace("{name}", item.Name);
        }

        public static ValidateResult TestStringLength(this ValidateItem item, object value)
        {
            if (value == null) return new ValidateResult(true);
            var str = value.ToString().Trim();
            if (str.Length >= item.Min && str.Length <= item.Max) return new ValidateResult(true);
            else return new ValidateResult(false, item.GetStringLengthMessage());
        }

        public static string GetStringLengthMessage(this ValidateItem item)
        {
            if (string.IsNullOrEmpty(item.Msg)) return item.Name + "：长度必须在" + item.Min + "和" + item.Max + "之间";
            else return item.Msg.Replace("{min}", item.Min.ToString()).Replace("{max}", item.Max.ToString()).Replace("{name}", item.Name);
        }

        public static ValidateResult TestRange(this ValidateItem item, object value)
        {
            if (value == null) return new ValidateResult(true);
            var val = value.ToString().ToInt(item.Min-1);
            if (val >= item.Min && val <= item.Max) return new ValidateResult(true);
            else return new ValidateResult(false, item.GetRangeMessage());
        }

        public static string GetRangeMessage(this ValidateItem item)
        {
            if (string.IsNullOrEmpty(item.Msg)) return item.Name + "：大小必须在" + item.Min + "和" + item.Max + "之间";
            else return item.Msg.Replace("{min}", item.Min.ToString()).Replace("{max}", item.Max.ToString()).Replace("{name}", item.Name);
        }

        public static ValidateResult TestRegex(this ValidateItem item, object value)
        {
            if (value == null) return new ValidateResult(true);
            var str = value.ToString().Trim();
            if (string.IsNullOrEmpty(str)) return new ValidateResult(true);
            if (str.RegexTest(item.ValStr)) return new ValidateResult(true);
            else return new ValidateResult(false, item.GetRegexMessage());
        }

        public static string GetRegexMessage(this ValidateItem item)
        {
            if (string.IsNullOrEmpty(item.Msg)) return item.Name + "：字段不符合规则";
            else return item.Msg.Replace("{name}", item.Name);
        }

        public static ValidateResult TestRemote(this ValidateItem item, object value)
        {
            if (value == null) return new ValidateResult(true);
            var str = value.ToString().Trim();
            if (string.IsNullOrEmpty(str)) return new ValidateResult(true);
            var result = wyx.Framework.Utils.HttpVisit.HttpGet(item.ValStr.Replace("{value}", str));
            if (result.Contains("\"success\":\"true\"") || result.Contains("\"success\":true")) return new ValidateResult(true);
            else return new ValidateResult(false, item.GetRemoteMessage());
        }

        public static string GetRemoteMessage(this ValidateItem item)
        {
            if (string.IsNullOrEmpty(item.Msg)) return item.Name + "：填写错误";
            else return item.Msg.Replace("{name}", item.Name);
        }

        public static ValidateResult TestIdCard(this ValidateItem item, object value)
        {
            if (value == null) return new ValidateResult(true);
            var str = value.ToString().Trim();
            if (string.IsNullOrEmpty(str)) return new ValidateResult(true);
            if (IdCardTool.CheckIDCard(str)) return new ValidateResult(true);
            return new ValidateResult(false, item.GetIdCardMessage());
        }

        public static string GetIdCardMessage(this ValidateItem item)
        {
            if (string.IsNullOrEmpty(item.Msg)) return item.Name + "：字段不符合规则";
            else return item.Msg.Replace("{name}", item.Name);
        }

        public static ValidateResult DoTest(this ValidateItem item, object value)
        {
            switch (item.ValiType.ToLower())
            {
                case "require": return item.TestRequire(value);
                case "stringlength": return item.TestStringLength(value);
                case "range": return item.TestRange(value);
                case "regex": return item.TestRegex(value);
                case "remote": return item.TestRemote(value);
                case "idcard": return item.TestIdCard(value);
                default: return new ValidateResult(true,"未找到验证项，默认做通过处理");
            }
        }

        public static List<ValidateResult> DoTest(this List<ValidateItem> items, object value)
        {
            var list = new List<ValidateResult>();
            foreach (var item in items)
            {
                list.Add(item.DoTest(value));
            }
            return list;
        }
    }
}
