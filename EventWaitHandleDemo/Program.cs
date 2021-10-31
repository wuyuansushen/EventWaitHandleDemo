using System;
using System.Threading;

namespace EventWaitHandleDemo
{
    class Program
    {
        private static EventWaitHandle ewh;

        private static long threadCount = 0;
        private static long threadNeedResume = 0;

        private static EventWaitHandle clearCountEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        [MTAThread]
        static void Main(string[] args)
        {
            #region AutoReset
/*
            //Exclusive access.
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);

            for(int i=0;i<5;i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Start(i);
            }
            while(Interlocked.Read(ref threadCount)<5)
            {
                Console.WriteLine($"Now threadCount is {threadCount}, so sleep.");
                Thread.Sleep(3000);
            }
            Console.WriteLine($"Now threadCount is {threadCount} after sleep.");
            while(Interlocked.Read(ref threadCount)>0)
            {
                Console.WriteLine($"Press ENTER to release a waiting thread.");
                Console.ReadLine();

                WaitHandle.SignalAndWait(ewh, clearCountEvent);
            }
            Console.WriteLine();
*/
            #endregion

            #region ManualReset

            //Like the gate of a corral
            ewh = new EventWaitHandle(false, EventResetMode.ManualReset);

            for(int i=0;i<5;i++)
            {
                var t = new Thread(ThreadProc);
                t.Start(i);
            }
            while(Interlocked.Read(ref threadCount)<5)
            {
                Console.WriteLine($"Only Start {threadCount}<5 threads\nsleep.\n");
                Thread.Sleep(1000);
            }
            Console.WriteLine($"All {threadCount} threads start.\n");
            Console.WriteLine($"Press TNTER to release all thread.");
            Console.ReadLine();
            ewh.Set();
            while (Interlocked.Read(ref threadNeedResume) > 0)
            {
                Console.WriteLine($"Remaing {threadNeedResume} thread(s) aren't resumed.\nWaitting...\n");
                Thread.Sleep(100);
            }
            while (Interlocked.Read(ref threadCount) > 0)
            {
                Console.WriteLine($"{threadCount} thread(s) are executing.\nWait please");
                Thread.Sleep(1500);
            }
            #endregion
        }

        public static void ThreadProc(object data)
        {
            int index = (int)data;
            Interlocked.Increment(ref threadCount);
            Console.WriteLine($"Thread {data} blocks.");
            Interlocked.Increment(ref threadNeedResume);
            bool handleReturn=ewh.WaitOne();//All threads block here.

            Thread.Sleep(300);
            Interlocked.Decrement(ref threadNeedResume);
            Console.WriteLine($"Thread Resume.");

            Thread.Sleep(3000);
            Interlocked.Decrement(ref threadCount);
            Console.WriteLine($"Thread {data} exits.");
        }
    }
}
