using System;
using System.Threading;

namespace QuasardbTests.Helpers
{
    class RandomGenerator
    {
        static readonly Random _rand = new Random();
        static long _uniqueId = DateTime.Now.Ticks;

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
