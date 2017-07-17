using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests
{
    [TestClass]
    public class DaemonRunner
    {
        public static bool UseSecurity { get; private set; }

        public const string EndPoint = "127.0.0.1:28360";
        public const string ClusterUrl = "qdb://" + EndPoint;

        public const string ClusterPublicKey = "PNArZFnNlIiJ0AC8f/7no/i+gFbRUOVumO0k9J+JV6mc=";
        public const string UserName = "qdb-api-dotnet";
        public const string UserPrivateKey = "SpGwxA5Pjv904DLkxA5a0lmJ0UvIMq96/+QFoFKh82eo=";

        private const string SecretKeyFile = "cluster-secret-key.txt";
        private const string UserCredentialsFile = "users.txt";

        static Process _daemon;

        private static Process StartDaemon(TestContext testContext, String endPoint, String description, String securityArguments)
        {
            Console.WriteLine($"Starting quasardb {description} daemon... ");
            Process daemon = Process.Start(new ProcessStartInfo
            {
                FileName = "qdbd.exe",
                Arguments = $"--transient -a {endPoint} {securityArguments}",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            Debug.Assert(daemon != null, "daemon != null");
            Thread.Sleep(2000);
            if (daemon.HasExited)
            {
                Console.Write(daemon.StandardOutput.ReadToEnd());
                Console.Write(daemon.StandardError.ReadToEnd());
                throw new Exception($"Failed to start {description} qdbd.exe");
            }

            Console.WriteLine($"Starting quasardb {description} daemon... OK");

            return daemon;
        }

        [AssemblyInitialize]
        public static void StartDaemon(TestContext testContext)
        {
            UseSecurity = testContext.Properties["useSecurity"]?.Equals("true") ?? false;

            if (UseSecurity)
            {
                _daemon = StartDaemon(testContext, EndPoint, "secure",
                                      $"--cluster-private-file={SecretKeyFile} --user-list={UserCredentialsFile}");
            } else
            {
                _daemon = StartDaemon(testContext, EndPoint, "insecure", "--security=false");
            }
        }

      private
        static void StopDaemon(Process daemon, String description) {
            Console.WriteLine($"Stopping quasardb {description} daemon... ");
            daemon.Kill();
            daemon.WaitForExit();
            Console.WriteLine($"Stopping quasardb {description} daemon... OK");
        }

        [AssemblyCleanup]
        public static void StopDaemon()
        {
            if (UseSecurity)
            {
                StopDaemon(_daemon, "secure");
            }
            else
            {
                StopDaemon(_daemon, "insecure");
            }
        }
    }
}
