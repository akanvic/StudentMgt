using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMgt.Core.Responses
{
    public class ResponseModel
    {
        public int State { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}
