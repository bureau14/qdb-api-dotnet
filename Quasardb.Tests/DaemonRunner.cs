using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class UserKey
{
    public string username { get; set; }
    public string secret_key { get; set; }
}

namespace Quasardb.Tests
{
    [TestClass]
    public static class DaemonRunner
    {
        public static bool UseSecurity { get; private set; }
        public static string ClusterUrl { get; private set; }
        public static string ClusterPublicKey { get; private set; }
        public static string UserName { get; private set; }
        public static string UserPrivateKey { get; private set; }

        public const string InsecureClusterUrl = "qdb://127.0.0.1:2836";
        public const string SecureClusterUrl = "qdb://127.0.0.1:2838";

        public const string ClusterPublicKeyFile = "../../../../cluster_public.key";
        public const string UserCredentialsFile = "../../../../user_private.key";

        [AssemblyInitialize]
        public static void StartDaemon(TestContext testContext)
        {
            UseSecurity = testContext.Properties["useSecurity"]?.Equals("true") ?? false;
            ClusterUrl = UseSecurity ? SecureClusterUrl : InsecureClusterUrl;

            ClusterPublicKey = System.IO.File.ReadAllText(ClusterPublicKeyFile);
            var credentials = new UserKey();
            var ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(UserCredentialsFile));
            var ser = new DataContractJsonSerializer(credentials.GetType());
            credentials = ser.ReadObject(ms) as UserKey;

            UserName = credentials.username;
            UserPrivateKey = credentials.secret_key;
        }
    }
}
