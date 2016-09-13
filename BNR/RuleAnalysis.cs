using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNR
{
    public class RuleAnalysis
    {
        [ThreadStatic]
        private static Stack<char> stack;

        public static string[] Execute(string rule)
        {
            List<string> items = new List<string>();
            StringBuilder sb = new StringBuilder();
            if (stack == null)
                stack = new Stack<char>();
            stack.Clear();
            foreach (char c in rule)
            {
                if (c == '{')
                {
                    if (stack.Count > 0)
                        sb.Append(c);
                    stack.Push(c);
                  
                }
                else if (c == '}')
                {
                    stack.Pop();
                    if (stack.Count == 0)
                    {
                        items.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                        sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return items.ToArray();
        }

        public static string[] GetProperties(string value)
        {
            int index = value.IndexOf(':');
            return new string[] { value.Substring(0,index),value.Substring(index+1,value.Length -index-1)};
        }
    }
}
