using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNR
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParameterTypeAttribute:Attribute
    {
        public ParameterTypeAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

    }
}
