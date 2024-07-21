using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHXD.APIServer.Core
{
    public class ValidateResult
    {
        public ValidateResult() { }

        public ValidateResult(bool success, string message = "")
        {
            this.Success = success;
            this.Message = message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
