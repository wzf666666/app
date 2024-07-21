using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHXD.APIServer.Core
{
    /// <summary>
    /// 为了保持独立性，建的这个类
    /// </summary>
    public class SQLMaper
    {
        public string FieldConfigId {get;set;}
        public string FieldType  {get;set;}
        public string TableField {get;set;}
        public string DisplayName {get;set;}
        public string InterfaceField {get;set;}
        public string TableName {get;set;}
        public string OrgCode {get;set;}
        public bool IsTableField { get; set; }
        public bool IsPause { get; set; }
        public string ValidateItems {get;set;}
        public bool IsDicName { get; set; }
        public string DicFrom { get; set; }
        public bool IsKey { get; set; }
    }
}
