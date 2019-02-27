using System;
using System.Collections.Generic;
using System.Threading;

namespace WpfApp4
{
    class Tester
    {
        private Thread vbusThr;
        private Thread contThr;
        private DS4Windows.X360Device xinputBus;
        //private DualShock4Controller dsc = null;
        //private Xbox360Controller xbux = null;
        private byte[] x360Rep = new byte[28];
        private byte[] outputRep = new byte[8];

        public void Start()
        {
            DS4Windows.DS4Devices.isExclusiveMode = true;
            DS4Windows.DS4Devices.findControllers();

            // Change thread affinity of bus object to not be tied
            // to GUI thread
            vbusThr = new Thread(() =>
            {
                xinputBus = new DS4Windows.X360Device();
                xinputBus.Open();
                xinputBus.Start();
            });

            vbusThr.Priority = ThreadPriority.Normal;
            vbusThr.IsBackground = true;
            vbusThr.Start();
            vbusThr.Join(); // Wait for bus object start

            IEnumerable<DS4Windows.DS4Device> devices = DS4Windows.DS4Devices.getDS4Controllers();
            int ind = 0;

            foreach (DS4Windows.DS4Device currentDev in devices)
            {
                xinputBus.Plugin(ind);
                currentDev.Report += ReadReport;
                // Start input data thread
                currentDev.StartUpdate();
                ind++;
            }
        }

        public void ReadReport(object sender, EventArgs e)
        {
            DS4Windows.DS4Device current = (DS4Windows.DS4Device)sender;
            DS4Windows.DS4State state = current.getCurrentStateRef();
            //DS4Windows.DS4State previous = current.getPreviousStateRef();

            // Translate input report for use in ScpVBus
            xinputBus.Parse(state, x360Rep, 0);
            // Send output report to driver
            xinputBus.Report(x360Rep, outputRep);
        }

        public void Stop()
        {
            xinputBus.UnplugAll();
            xinputBus.Close();
        }
    }
}
