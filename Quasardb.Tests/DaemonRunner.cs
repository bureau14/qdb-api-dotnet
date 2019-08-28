using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests
{
    [TestClass]
    public class DaemonRunner
    {
        public static bool UseSecurity { get; private set; }
        public static string ClusterUrl { get; private set; }
        public static string ClusterPublicKey { get; private set; }
        public static string UserName { get; private set; }
        public static string UserPrivateKey { get; private set; }

        public const string InsecureClusterUrl = "qdb://127.0.0.1:2836";
        public const string SecureClusterUrl = "qdb://127.0.0.1:2837";

        public const string ClusterPublicKeyFile = "../../../cluster_public.key";
        public const string UserCredentialsFile = "../../../user_private.key";

        [AssemblyInitialize]
        public static void StartDaemon(TestContext testContext)
        {
            UseSecurity = testContext.Properties["useSecurity"]?.Equals("true") ?? false;
            ClusterUrl = UseSecurity ? SecureClusterUrl : InsecureClusterUrl;
            if (UseSecurity)
            {
                ClusterPublicKey = System.IO.File.ReadAllText(ClusterPublicKeyFile);
                var credentials = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(System.IO.File.ReadAllText(UserCredentialsFile));

                UserName = credentials["username"];
                UserPrivateKey = credentials["secret_key"];
            }
        }
    }
}
