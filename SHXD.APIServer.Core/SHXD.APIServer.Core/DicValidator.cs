using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wyx.Framework;

namespace SHXD.APIServer.Core
{
    public static class DicValidator
    {
        public static List<ValidateResult> DoValidate(Dictionary<string, object> dic, List<SQLMaper> mappers)
        {
            List<ValidateResult> list = new List<ValidateResult>();
            if (dic == null || dic.Keys.Count == 0) return list;
            if (mappers == null || mappers.Count == 0) return list;

            foreach (var key in dic.Keys)
            {
                var maper = mappers.FirstOrDefault(a => a.TableField.Equals(key));
                if (maper == null || string.IsNullOrEmpty(maper.ValidateItems)) continue;
                var value = dic[key];
                var valiItems = JsonHelp.ToObject<List<ValidateItem>>(maper.ValidateItems);
                foreach (var item in valiItems)
                {
                    if (string.IsNullOrEmpty(item.Name)) item.Name = maper.DisplayName;
                }
                list.AddRange(valiItems.DoTest(value));
            }

            return list;
        }
    }
}
