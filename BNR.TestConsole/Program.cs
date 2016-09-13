using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNR.TestConsole
{
    class Program
    {

        static long mCount;

        static void Main(string[] args)
        {
            //{N:ORDER2016/0000}
            //string[] rule = new string[]{ 
            //    "{S:OD}{CN:广州}{D:yyyyMM}{N:{S:ORDER}{CN:广州}{D:yyyyMM}/00000000}{N:{S:ORDER_SUB}{CN:广州}{D:yyyyMM}/00000000}",
            //    "{S:OD}{CN:深圳}{D:yyyyMM}{N:{S:ORDER}{CN:深圳}{D:yyyyMM}/00000000}",
            //    "{S:SQ}{D:yyyy}{N:{S:SQ}{D:yyyy}/00000000}"
            //};
            string[] rule = new string[]{ 
               
                "{S:OD}{CN:广州}{D:yyyyMM}{N:{S:ORDER}{CN:广州}{D:yyyyMM}/00000000}{N:{S:ORDER_SUB}{CN:广州}{D:yyyyMM}/00000000}",
                "{CN:广州}{D:yyyyMMdd}{N:{D:yyyyMMdd}/0000}{S:RJ}"
            };
            foreach (string item in rule)
            {
                string value = BNRFactory.Default.Create(item);
                Console.WriteLine(item);
                Console.WriteLine(value);
            }
            System.Threading.ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    foreach (string item in rule)
                    {
                        string value = BNRFactory.Default.Create(item);
                        // Console.WriteLine("{0}={1}", item, value);
                        System.Threading.Interlocked.Increment(ref mCount);
                    }
                }
            });
            while (true)
            {
                Console.WriteLine(mCount);
                System.Threading.Thread.Sleep(1000);
            }
            Console.Read();
        }
    }
}
