using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace asyns_demo {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// 
    /// 例子参考：
    /// https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void btn_sync_Click(object sender, RoutedEventArgs e)
        {
            //计时
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            //1. 倒咖啡（假设是现成的，立刻就可以完成）
            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");

            //2. 煎 2 个鸡蛋
            Egg eggs = FryEggs(2);
            Console.WriteLine("eggs are ready");

            //3. 煎 3 片培根
            Bacon bacon = FryBacon(3);
            Console.WriteLine("bacon is ready");

            //4. 烤 2 片面包
            Toast toast = ToastBread(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("toast is ready");

            //5. 倒橙汁
            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");

            sw.Stop();
            Console.WriteLine("===> all finish, use time: {0} ms", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 异步方法 用于事件，
        /// 
        /// 这里需要使用了 async 和 await 关键字，但是执行的顺序和原来同步的方法是一样的（await 关键字会挂起当前线程，进行等待，控制器返回其调用者（所以界面不会卡）），
        /// 
        /// 唯一的区别是，界面不会卡死
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_async1_Click(object sender, RoutedEventArgs e)
        {
            //计时
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            //1. 倒咖啡（假设是现成的，立刻就可以完成）
            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");

            //2. 煎 2 个鸡蛋
            Egg eggs = await FryEggsAsync(2);
            Console.WriteLine("eggs are ready");

            //3. 煎 3 片培根
            Bacon bacon = await FryBaconAsync(3);
            Console.WriteLine("bacon is ready");

            //4. 烤 2 片面包
            Toast toast = await ToastBreadAsync(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("toast is ready");

            //5. 倒橙汁
            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!"); 
            
            sw.Stop();
            Console.WriteLine("===> all finish, use time: {0} ms", sw.ElapsedMilliseconds);

        }

        /// <summary>
        /// 调整 await 的顺序，存储 Task 任务，然后合理安排等待动作，
        /// 
        /// 让异步任务可以同时进行。
        /// 
        /// 不仅解决阻塞的问题，还解决 UI 卡顿
        /// 
        /// 做出的修改：
        /// 将： 
        /// Egg = await FryEggsAsync(2);//线程在这里挂起，进行等待
        /// 改为：
        /// Task<Egg> eggsTask = FryEggsAsync(2);
        /// //这中间还可以“立即”执行其他代码
        /// await eggsTask；//在这里等待
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_async2_Click(object sender, RoutedEventArgs e)
        {
            //计时
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("Coffee is ready");

            Task<Egg> eggsTask = FryEggsAsync(2);
            Task<Bacon> baconTask = FryBaconAsync(3);
            Task<Toast> toastTask = ToastBreadAsync(2);

            Toast toast = await toastTask;
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("Toast is ready");
            Juice oj = PourOJ();
            Console.WriteLine("Oj is ready");

            Egg eggs = await eggsTask;
            Console.WriteLine("Eggs are ready");
            Bacon bacon = await baconTask;
            Console.WriteLine("Bacon is ready");

            Console.WriteLine("Breakfast is ready!");

            sw.Stop();
            Console.WriteLine("===> all finish, use time: {0} ms", sw.ElapsedMilliseconds);
        }

        private async void btn_async3_Click(object sender, RoutedEventArgs e)
        {
            //计时
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");

            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);

            var eggs = await eggsTask;
            Console.WriteLine("eggs are ready");

            var bacon = await baconTask;
            Console.WriteLine("bacon is ready");

            var toast = await toastTask;
            Console.WriteLine("toast is ready");

            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");

            sw.Stop();
            Console.WriteLine("===> all finish, use time: {0} ms", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 通过 WhenAll() 来高效监督多个任务的完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_async_when_all_Click(object sender, RoutedEventArgs e)
        {
            //计时
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");

            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);

            await Task.WhenAll(eggsTask, baconTask, toastTask);
            Console.WriteLine("Eggs are ready");
            Console.WriteLine("Bacon is ready");
            Console.WriteLine("Toast is ready");
            Console.WriteLine("Breakfast is ready!");

            sw.Stop();
            Console.WriteLine("===> all finish, use time: {0} ms", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 通过 WhenAny() 来高效监督多个任务的完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_async_when_any_Click(object sender, RoutedEventArgs e)
        {
            //计时
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");

            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);

            var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
            while (breakfastTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(breakfastTasks);
                if (finishedTask == eggsTask)
                {
                    Console.WriteLine("eggs are ready");
                }
                else if (finishedTask == baconTask)
                {
                    Console.WriteLine("bacon is ready");
                }
                else if (finishedTask == toastTask)
                {
                    Console.WriteLine("toast is ready");
                }
                await finishedTask;
                breakfastTasks.Remove(finishedTask);
            }

            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");

            sw.Stop();
            Console.WriteLine("===> all finish, use time: {0} ms", sw.ElapsedMilliseconds);
        }

        #region 1.同步的方法, btn_sync_Click 事件调用
        private static Juice PourOJ()
        {
            Console.WriteLine("========5.Begin========");
            Console.WriteLine("Pouring orange juice");
            return new Juice();
        }

        private static void ApplyJam(Toast toast) => 
            Console.WriteLine("Putting jam on the toast");

        private static void ApplyButter(Toast toast) =>
            Console.WriteLine("Putting butter on the toast");

        private static Toast ToastBread(int slices)
        {
            Console.WriteLine("========4.Begin========");
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
            }
            Console.WriteLine("Start toasting...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }

        private static Bacon FryBacon(int slices)
        {
            Console.WriteLine("========3.Begin========");
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            Task.Delay(3000).Wait();
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("flipping a slice of bacon");
            }
            Console.WriteLine("cooking the second side of bacon...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Put bacon on plate");

            return new Bacon();
        }

        private static Egg FryEggs(int howMany)
        {
            Console.WriteLine("========2.Begin========");
            Console.WriteLine("Warming the egg pan...");
            Task.Delay(3000).Wait();
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking the eggs ...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Put eggs on plate");

            return new Egg();
        }

        private static Coffee PourCoffee()
        {
            Console.WriteLine("========1.Begin========");
            Console.WriteLine("Pouring coffee");
            return new Coffee();
        }
        #endregion

        #region 2.使用了 async 和 await 关键字的异步方法
        private static async Task<Toast> ToastBreadAsync(int slices)
        {
            Console.WriteLine("========4.Begin========");
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
            }
            Console.WriteLine("Start toasting...");
            await Task.Delay(3000);
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }

        private static async Task<Bacon> FryBaconAsync(int slices)
        {
            Console.WriteLine("========3.Begin========");
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            await Task.Delay(3000);
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("flipping a slice of bacon");
            }
            Console.WriteLine("cooking the second side of bacon...");
            await Task.Delay(3000);
            Console.WriteLine("Put bacon on plate");

            return new Bacon();
        }

        private static async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("========2.Begin========");
            Console.WriteLine("Warming the egg pan...");
            await Task.Delay(3000);
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking the eggs ...");
            await Task.Delay(3000);
            Console.WriteLine("Put eggs on plate");

            return new Egg();
        }

        /// <summary>
        /// tips: 异步操作的组成，后面跟着同步工作，就是异步操作。换句话说，如果操作的任何部分是异步的，则整个操作都是异步的。
        /// 
        /// 因为烤面包、涂黄油、涂果酱是配套的动作，
        /// 
        /// 所以将它们整合进一个方法内
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
        {
            var toast = await ToastBreadAsync(number);
            ApplyButter(toast);
            ApplyJam(toast);

            return toast;
        }
        #endregion

    }
    // These classes are intentionally empty for the purpose of this example. They are simply marker classes for the purpose of demonstration, contain no properties, and serve no other purpose.
    internal class Bacon { }
    internal class Coffee { }
    internal class Egg { }
    internal class Juice { }
    internal class Toast { }
}
