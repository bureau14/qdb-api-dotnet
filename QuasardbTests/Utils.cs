using System;
using System.Threading;

namespace QuasardbTests
{
    class Utils
    {
        static long _uniqueId = DateTime.Now.Ticks;
        static Random _rand = new Random();

        public static string CreateUniqueAlias()
        {
            return "test_alias_" + Interlocked.Increment(ref _uniqueId);
        }

        public static byte[] CreateRandomContent()
        {
            var buffer = new byte[_rand.Next(10, 20)];
            _rand.NextBytes(buffer);
            return buffer;
        }
    }
}
