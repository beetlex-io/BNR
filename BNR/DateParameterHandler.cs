using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNR
{
    /// <summary>
    /// {D:yyyyMMdd}
    /// </summary>
    [ParameterType("D")]
    public class DateParameterHandler : IParameterHandler
    {

        public void Execute(StringBuilder sb, string value)
        {
            sb.Append(DateTime.Now.ToString(value));
        }

        public BNRFactory Factory
        {
            get;
            set;
        }
    }
}
