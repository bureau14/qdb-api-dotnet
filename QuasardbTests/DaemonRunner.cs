using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuasardbTests
{
    [TestClass]
    public class DaemonRunner
    {
        static Process _daemon;

        [AssemblyInitialize]
        public static void StartDaemon(TestContext testContext)
        {
            Console.WriteLine("Starting quasardb daemon... ");
            _daemon = Process.Start(new ProcessStartInfo
            {
                FileName = "qdbd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            Thread.Sleep(5000);
            if (_daemon.HasExited)
            {
                Console.Write(_daemon.StandardOutput.ReadToEnd());
                throw new Exception("Failed to start qdbd.exe");
            }
            Console.WriteLine("Starting quasardb daemon... OK");
        }

        [AssemblyCleanup]
        public static void StopDaemon()
        {
            Console.WriteLine("Stopping quasardb daemon... ");
            _daemon.Kill();
            _daemon.WaitForExit();
            Console.WriteLine("Stopping quasardb daemon... OK");
        }
    }
}
