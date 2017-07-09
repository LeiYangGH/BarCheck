using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestAlarmTime
{
    class Program
    {
        //static Thread t;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            for (int i = 1; i <= 5; i++)
            {
                Thread.Sleep(500);
                Alarm();
            }
            Console.WriteLine("end");
            Console.ReadLine();
        }

        private static bool closeBeforeAlarm = true;
        private static int alarmMs = 2000;
        private static DateTime closeTime;
        private static Guid latestG;
        public static void Alarm()
        {
            //await Task.Run(async () =>
            //{
            //    Console.WriteLine("1");
            //    await Task.Delay(2000);
            //    Console.WriteLine("  2");
            //});
            //if (t == null)
            //if (t != null && t.IsAlive && closeBeforeAlarm)
            //    t.Abort();
            latestG = Guid.NewGuid();
            Thread t = new Thread(() => AlarmFun(latestG));
            t.IsBackground = true;
            closeTime = DateTime.Now.AddMilliseconds(alarmMs);
            t.Start();

        }

        public static void AlarmFun(Guid g)
        {
            if (closeBeforeAlarm)
                Console.WriteLine("  2");
            Console.WriteLine("1");
            while (DateTime.Now < closeTime)
            {
                if (g != latestG)
                    return;
                Thread.Sleep(50);
            }
            Console.WriteLine("  2");
        }
    }
}
