using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace demo线程
{
    public delegate string AsyncMethodCaller(int callDuration, out int threadId);
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMethodCaller caller = new AsyncMethodCaller(TestMethodAsync); // caller 为委托函数
            int threadid = 0;
            //开启异步操作
            IAsyncResult result = caller.BeginInvoke(1000, out threadid, null, null);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("其它业务" + i.ToString());
            }
            //调用EndInvoke,等待异步执行完成
            Console.WriteLine("等待异步方法TestMethodAsync执行完成");
            //等待异步执行完毕信号
            //result.AsyncWaitHandle.WaitOne();
            //Console.WriteLine("收到WaitHandle信号");
            //通过循环不停的检查异步运行状态
            while (result.IsCompleted == false)
            {
                Thread.Sleep(100);
                Console.WriteLine("异步方法，running........");
            }
            //异步结束，拿到运行结果
            string res = caller.EndInvoke(out threadid, result);
            //显示关闭句柄
            result.AsyncWaitHandle.Close();
            Console.WriteLine("关闭了WaitHandle句柄");
            Console.Read();


        }
        public static void Calculate()
        {
            DateTime time = DateTime.Now;//得到当前时间
            Random ra = new Random();//随机数对象
            Thread.Sleep(ra.Next(10, 100));//随机休眠一段时间
            Console.WriteLine(time.Minute + ":" + time.Millisecond);
        }
        static string TestMethodAsync(int callDuration, out int threadId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("异步TestMethodAsync开始");
            for (int i = 0; i < 5; i++)
            { // 模拟耗时操作
                Thread.Sleep(callDuration);
                Console.WriteLine("TestMethodAsync:" + i.ToString());
            }
            sw.Stop();
            threadId = Thread.CurrentThread.ManagedThreadId;
            return string.Format("耗时{0}ms.", sw.ElapsedMilliseconds.ToString());
        }
    }

}
