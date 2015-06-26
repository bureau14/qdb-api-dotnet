using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuasardbTests
{
    [TestClass]
    public class DaemonRunner
    {
        public const string ClusterUrl = "qdb://127.0.0.1:2836";

        static Process _daemon;

        [AssemblyInitialize]
        public static void StartDaemon(TestContext testContext)
        {
            Console.WriteLine("Starting quasardb daemon... ");
            _daemon = Process.Start(new ProcessStartInfo
            {
                FileName = "qdbd.exe",
                Arguments = "--transient",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            Debug.Assert(_daemon != null, "_daemon != null");
            Thread.Sleep(2000);
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
