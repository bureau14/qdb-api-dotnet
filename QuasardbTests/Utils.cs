using System;
using System.Threading;

namespace QuasardbTests
{
    class Utils
    {
        static long _uniqueId = DateTime.Now.Ticks;

        public static string CreateUniqueAlias()
        {
            return "test_alias_" + Interlocked.Increment(ref _uniqueId);
        }

        public static byte[] CreateRandomContent()
        {
            var rand = new Random();
            var buffer = new byte[rand.Next(10, 20)];
            rand.NextBytes(buffer);
            return buffer;
        }
    }
}
