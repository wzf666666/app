using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wyx.Framework;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace SHXD.APIServer.Core
{
    public class MySQLHelper
    {
        protected const string STR_EMPTY = "";

        public MySQLHelper(string constr)
        {
            db = new wyx.Framework.DBHelper.MySqlDBHelper(constr);
        }

        wyx.Framework.DBHelper.MySqlDBHelper db = null;

        protected ICacheManager cache = CacheManager.Def;

        public List<T> DapperList<T>(string sqlstr)
        {
            return db.DapperList<T>(sqlstr);
        }

        public string GetSelectSql(List<SQLMaper> maperList, string tableName = "", string whereStr = "1=1", string orderStr = "", int pagenow = StaticValue.DefPageNow, int pagesize = StaticValue.DefPageSize, params string[] ignore)
        {
            //string sqlstr = cache.Get_<string>(tableName + "_select_sql_page" + "_" + string.Join(",", ignore));
            string sqlstr = string.Empty;
            if (string.IsNullOrEmpty(sqlstr))
            {
                sqlstr = "select " + CreateSelectFieldSql(maperList) +" from " + tableName + " ";
                sqlstr += " where " + whereStr + " ";
                
                sqlstr = this.GetPagingSql(sqlstr, orderStr, pagenow, pagesize);
                //cache.Insert_(tableName + "_select_sql_page" + "_" + string.Join(",", ignore), sqlstr);
            }
            
            return sqlstr;
        }

        public DataTable GetPageTable(List<SQLMaper> maperList, string tableName = "", string whereStr = "", string orderStr = "", int pagenow = StaticValue.DefPageNow, int pagesize = StaticValue.DefPageSize, params string[] ignore)
        {
            
            if (maperList.Count == 0) throw new Exception("maperList数量为0");
            if (string.IsNullOrEmpty(tableName)) tableName = maperList[0].TableName;
            if (string.IsNullOrEmpty(whereStr)) whereStr = "1=1";
            var sql = GetSelectSql(maperList,tableName,whereStr,orderStr,pagenow,pagesize,ignore);
            var table = db.ExcuteTable(sql);
            return table;
        }

        public List<Dictionary<string,object>> GetPageList(List<SQLMaper> maperList, string tableName = "", string whereStr = "", string orderStr = "", int pagenow = StaticValue.DefPageNow, int pagesize = StaticValue.DefPageSize, params string[] ignore)
        {

            var table = GetPageTable(maperList,tableName,whereStr,orderStr,pagenow,pagesize,ignore);
            return TableToListDic(table);
        }

        public DataTable GetById(string id, SQLMaper maper)
        {
            if (maper == null || string.IsNullOrEmpty(id)) throw new Exception("id或maper为空");
            var table = db.ExcuteTable("select * from " + maper.TableName + " where " + maper.TableField + "='" + id.Trim() + "'");
            return table;
        }

        public Dictionary<string,object> GetByIdDic(string id, SQLMaper maper)
        {
            var table = GetById(id,maper);
            if (table.Rows.Count == 0) return null;
            return TableToModelDic(table);
        }

        public bool IsExit(string id, SQLMaper maper)
        {
            if (maper == null || string.IsNullOrEmpty(id)) throw new Exception("id或maper为空");
            var result = db.ExcuteScalar("select count(1) from " + maper.TableName + " where " + maper.TableField + "='" + id.Trim() + "'");
            return result.ToString().ToInt() > 0;
        }

        public string GetPagingSql(string sqlstr, string orderstr, int pagenow = 1, int pagesize = 20)
        {
            if (!orderstr.ToLower().Contains("order by")) orderstr = "order by " + orderstr.TrimStart();
            return sqlstr + " " + orderstr + " limit " + ((pagenow - 1) * pagesize) + "," + pagesize;
        }

        public int GetPagingCount(string wherestr, string tablename)
        {
            string sqlstr = "select count(1) from " + tablename;
            if(!string.IsNullOrEmpty(wherestr)) sqlstr += " where " + wherestr;
            return db.ExcuteScalar(sqlstr).ToString().ToInt();
        }

        public string CreateSelectFieldSql(List<SQLMaper> maperList, params string[] ignore)
        {
            return string.Join(",", maperList.Where(a=>!a.IsPause && !a.IsDicName).Select(a => "`" + a.TableField + "`").ToList());
        }

        public string CreateAddSql(List<SQLMaper> maperList, string tableName = "", params string[] ignore)
        {
            List<string> propertyNames = new List<string>();
            var mapers = maperList.Where(a => !a.IsPause && !a.IsDicName).ToList();
            foreach (var info in mapers)
            {
                if (ignore.Contains(info.TableField)) continue;

                propertyNames.Add(info.TableField);
            }
            string sqlstr = "insert into " + tableName + "(" + string.Join(",", propertyNames) + ") values(?" + string.Join(",?", propertyNames) + ")";
            cache.Insert_(tableName + "_Add_paramnames" + "_" + string.Join(",", ignore), propertyNames);
            cache.Insert_(tableName + "_Add_sql" + "_" + string.Join(",", ignore), sqlstr);
            
            return sqlstr;
        }

        public string CreateUpdateSql(List<SQLMaper> maperList, string tableName = "", params string[] ignore)
        {
            
            
            List<string> propertyNames = new List<string>();
            List<string> keyNames = new List<string>();
            foreach (var info in maperList)
            {
                if (ignore.Contains(info.TableField)) continue;
                if (info.IsDicName) continue;
                else if (info.IsKey)
                {
                    keyNames.Add(info.TableField);
                    continue;
                }
                else
                {
                    propertyNames.Add(info.TableField);
                }
            }

            if (keyNames.Count == 0) throw new Exception("没有主键标记");
            string sqlstr = cache.Get_<string>(tableName + "_Update_sql" + "_" + string.Join(",", ignore));
            if (string.IsNullOrEmpty(sqlstr))
            {
                sqlstr = "update " + tableName + " set ";
                foreach (var name in propertyNames)
                {
                    sqlstr += name + "=?" + name + ",";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 1);

                foreach (var key in keyNames)
                {
                    propertyNames.Add(key);
                    if (sqlstr.Contains("where"))
                    {
                        sqlstr += " and " + key + "=?" + key;
                    }
                    else
                    {
                        sqlstr += " where " + key + "=?" + key;
                    }

                }
                cache.Insert_(tableName + "_Update_paramnames" + "_" + string.Join(",", ignore), propertyNames);
                cache.Insert_(tableName + "_Update_sql" + "_" + string.Join(",", ignore), sqlstr);

            }
            
            return sqlstr;
        }

        public string CreateDeleteSql(SQLMaper idmaper,string tableName="")
        {
            if (idmaper == null) throw new Exception("maper为空");
            if (string.IsNullOrEmpty(tableName)) tableName = idmaper.TableName;
            return "delete from " + tableName + " where " + idmaper.TableField + "=?" + idmaper.TableField;
        }

        public string CreateDeleteSql(List<SQLMaper> maperList, string tableName = "")
        {
            if (maperList.Count == 0) throw new Exception("maperList数量为0");
            if (string.IsNullOrEmpty(tableName)) tableName = maperList[0].TableName;
            string sqlkey = tableName + "_Delete2_sql";
            string cachekey = tableName + "_Delete2_key";

            var keynames = maperList.Where(a => a.IsKey && !a.IsPause && !a.IsDicName).Select(a => a.TableField).ToList();
            string sqlstr;

            sqlstr = "delete from " + tableName + " where 1=1";

            foreach (var keyname in keynames)
            {
                sqlstr += " and " + keyname + "=?" + keyname;
            }
            cache.Insert_(sqlkey, sqlstr);
            cache.Insert_(cachekey, keynames);
            return sqlstr;
        }

        public object Add(Dictionary<string, object> model, List<SQLMaper> maperList, string tableName = "", params string[] ignore)
        {
            if (maperList.Count == 0) throw new Exception("maperList数量为0");
            if (string.IsNullOrEmpty(tableName)) tableName = maperList[0].TableName;
            string sqlstr = cache.Get_<string>(tableName + "_Add_sql" + "_" + string.Join(",", ignore));
            if (string.IsNullOrEmpty(sqlstr))
            {
                this.CreateAddSql(maperList,tableName, ignore);
                sqlstr = cache.Get_<string>(tableName + "_Add_sql" + "_" + string.Join(",", ignore));
            }
            List<string> paramnames = cache.Get_<List<string>>(tableName + "_Add_paramnames" + "_" + string.Join(",", ignore));
            MySqlCommand cmd = new MySqlCommand();
            ConverNullDic(model);
            foreach (var info in maperList)
            {
                if (!paramnames.Contains(info.TableField)) continue;
                object value = null;
                if (model.ContainsKey(info.TableField)) value = ConvertField(model[info.TableField],info);
                else value = string.Empty;

                cmd.Parameters.Add(new MySqlParameter("?" + info.TableField, value));
            }

            cmd.CommandText = sqlstr;

            int i = db.ExcuteNonQuery(cmd);
            return i;
        }

        public int Update(Dictionary<string, object> model, List<SQLMaper> maperList, string tableName = "", params string[] ignore)
        {
            if (maperList.Count == 0) throw new Exception("maperList数量为0");
            var mapers = maperList.Where(a => !a.IsPause && !a.IsDicName).ToList();
            if (string.IsNullOrEmpty(tableName)) tableName = maperList[0].TableName;
            string sqlstr = cache.Get_<string>(tableName + "_Update_sql" + "_" + string.Join(",", ignore));
            if (string.IsNullOrEmpty(sqlstr))
            {
                this.CreateUpdateSql(maperList,tableName, ignore);
                sqlstr = cache.Get_<string>(tableName + "_Update_sql" + "_" + string.Join(",", ignore));
            }
            List<string> paramnames = cache.Get_<List<string>>(tableName + "_Update_paramnames" + "_" + string.Join(",", ignore));
            MySqlCommand cmd = new MySqlCommand();
            ConverNullDic(model);
            foreach (var info in maperList)
            {
                if (paramnames.Contains(info.TableField))
                {
                    object value = null;
                    if (model.ContainsKey(info.TableField)) value = ConvertField(model[info.TableField], info);
                    else value = string.Empty;
                    cmd.Parameters.Add(new MySqlParameter("?" + info.TableField, value));
                }
            }

            cmd.CommandText = sqlstr;

            return db.ExcuteNonQuery(cmd);
        }

        public int Delete(Dictionary<string, object> model, List<SQLMaper> maperList, string tableName = "") 
        {
            if (maperList.Count == 0) throw new Exception("maperList数量为0");
            if (string.IsNullOrEmpty(tableName)) tableName = maperList[0].TableName;

            string sqlstr;

            MySqlCommand cmd = new MySqlCommand();

                List<string> propertyNames = new List<string>();

                sqlstr = "delete from " + tableName + " where 1=1";
                var keylist = maperList.Where(a=>a.IsKey && !a.IsDicName && !a.IsPause).Select(a=>a.TableField).ToList();

                foreach (var keyname in keylist)
                {
                    sqlstr += " and " + keyname + "=?" + keyname;
                }

            foreach (var key in keylist)
            {
                cmd.Parameters.Add(new MySqlParameter("?" + key, model[key]));
            }

            cmd.CommandText = sqlstr;

            return db.ExcuteNonQuery(cmd);
        }

        private object ConvertField(object obj,SQLMaper maper)
        {
            if (obj == null) return string.Empty;
            object result = null;
            switch (maper.FieldType.ToLower())
            {
                case "int": result = obj.ToString().ToInt();
                    break;
                case "float": result = obj.ToString().ToFloat();
                    break;
                case "double": result = obj.ToString().ToDouble();
                    break;
                case "decimal" : result = obj.ToString().ToDecimal();
                    break;
                case "number" : result = obj.ToString().ToDecimal();
                    break;
                case "datetime" : result = obj.ToString().ToDateTime();
                    break;
                case "bool": result = Convert.ToBoolean(obj);
                    break;
                case "bit": result = Convert.ToBoolean(obj);
                    break;
                default: result = obj.ToString();
                    break;
            }
            return result;
        }

        protected Dictionary<string, object> TableToModelDic(DataTable table)
        {
            if (table == null) return null;
            if (table.Rows.Count == 0) return null;
            return JsonHelp.ToObject<List<Dictionary<string, object>>>(table.ToJson())[0];
        }

        protected List<Dictionary<string, object>> TableToListDic(DataTable table)
        {
            if (table == null) return null;
            if (table.Rows.Count == 0) return new List<Dictionary<string,object>>();
            return JsonHelp.ToObject<List<Dictionary<string, object>>>(table.ToJson());
        }

        protected void ConverNullDic(Dictionary<string, object> model)
        {
            if (model == null) return;
            var keys = model.Keys.ToList();
            foreach (var key in keys)
            {
                if (model[key] == null) model[key] = "";
            }
        }
    }
}
