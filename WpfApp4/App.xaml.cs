using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Thread testThread;
        private Tester test;

        public App()
        {
            try
            {
                Process.GetCurrentProcess().PriorityClass =
                    System.Diagnostics.ProcessPriorityClass.High;
            }
            catch { } // Ignore problems raising the priority.

            testThread = new Thread(() =>
            {
                test = new Tester();
                test.Start();
            });

            testThread.IsBackground = true;
            testThread.Start();
            testThread.Join();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            test.Stop();
            base.OnExit(e);
        }
    }
}
