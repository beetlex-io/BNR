using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BNR
{
    public class BNRFactory
    {
        private Dictionary<string, IParameterHandler> mHandlers = new Dictionary<string, IParameterHandler>();


        public IDictionary<string, IParameterHandler> Handlers
        {

            get
            {
                return mHandlers;
            }
        }

        public void Initialize()
        {
            Register(typeof(BNRFactory).Assembly);
        }

        public void Register(System.Reflection.Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                Register(type);
            }
        }

        private static BNRFactory mDefault = null;

        public static BNRFactory Default
        {
            get
            {
                if (mDefault == null)
                {
                    mDefault = new BNRFactory();
                    mDefault.Initialize();
                }
                return mDefault;
            }
        }

        public void Register(Type type)
        {
            ParameterTypeAttribute[] result = (ParameterTypeAttribute[])type.GetCustomAttributes(typeof(ParameterTypeAttribute), false);
            if (result != null && result.Length > 0)
            {
                mHandlers[result[0].Name] =(IParameterHandler) Activator.CreateInstance(type);
                mHandlers[result[0].Name].Factory = this;
            }
        }
        public void Register<T>() where T:SequenceParameter
        {
            Register(typeof(T));
        }

        public string Create(string rule)
        {
            string[] items = RuleAnalysis.Execute(rule);
            StringBuilder sb = new StringBuilder();
            foreach (string item in items)
            {
                string[] properties = RuleAnalysis.GetProperties(item);
                IParameterHandler handler = null;
                if(mHandlers.TryGetValue(properties[0],out handler))
                {
                     handler.Execute(sb, properties[1]);
                }
            }
            return sb.ToString();
        }
    }
}
