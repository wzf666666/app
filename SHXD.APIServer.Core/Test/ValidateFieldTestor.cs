using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wyx.Framework;
using SHXD.APIServer.Core;

namespace Test
{
    public class ValidateFieldTestor
    {
        public void Run()
        {
            try
            {
                ValidateResult result = null;
                object val = null;
                ValidateItem item = new ValidateItem() { ValiType = "require", ValStr = string.Empty, Name = "年龄", Msg = "这个字段{name}不能为空" };
                result = item.DoTest("  abc");
                Console.WriteLine(result.ToJson());

                item = new ValidateItem() { ValiType = "stringlength", ValStr = string.Empty, Name = "姓名", Msg = "", Min = 1, Max = 5 };
                val = "张三";
                result = item.DoTest(val);
                Console.WriteLine(result.ToJson());


                var dic = new Dictionary<string, object>();
                dic.Add("id", Guid.NewGuid().ToString());
                dic.Add("name", "aaaaaaaaaaaa");
                dic.Add("age", 99);
                dic.Add("idcard", "320311198603046419");

                var mappers = new List<SQLMaper>();
                mappers.Add(new SQLMaper() { DisplayName = "Id", FieldConfigId = "asdf", FieldType = "string", IsKey = true, TableField = "id", TableName = "abc", ValidateItems = string.Empty });
                List<ValidateItem> vitems2 = new List<ValidateItem>();
                vitems2.Add(new ValidateItem() { ValiType = "require", ValStr = string.Empty, Msg = "这个字段{name}不能为空" });
                vitems2.Add(new ValidateItem() { ValiType = "stringlength", Min = 1, Max = 10 });

                mappers.Add(new SQLMaper() { DisplayName = "姓名", FieldConfigId = "asdf", FieldType = "string", TableField = "name", TableName = "abc", ValidateItems=vitems2.ToJson() });

                List<ValidateItem> vitems3 = new List<ValidateItem>();
                vitems3.Add(new ValidateItem() { ValiType = "require", ValStr = string.Empty, Msg = "这个字段{name}不能为空" });
                vitems3.Add(new ValidateItem() { ValiType = "range", Min = 0, Max = 100 });
                mappers.Add(new SQLMaper() { DisplayName = "年龄", FieldConfigId = "asdf", FieldType = "int", TableField = "age", TableName = "abc", ValidateItems = vitems3.ToJson() });

                List<ValidateItem> vitems4 = new List<ValidateItem>();
                vitems4.Add(new ValidateItem() { ValiType = "require", ValStr = string.Empty, Msg = "这个字段{name}不能为空" });
                vitems4.Add(new ValidateItem() { ValiType = "idcard"});
                mappers.Add(new SQLMaper() { DisplayName = "身份证号", FieldConfigId = "asdf", FieldType = "string", TableField = "idcard", TableName = "abc", ValidateItems = vitems4.ToJson() });

                var testlist = DicValidator.DoValidate(dic, mappers).Where(a=>!a.Success).ToList();
                Console.WriteLine(testlist.ToJson());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
