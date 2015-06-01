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
            return new byte[10];
        }
    }
}
