using System;
using System.Threading;

namespace Quasardb.Tests
{
    static class RandomGenerator
    {
        private static readonly ThreadLocal<Random> _rand = new ThreadLocal<Random>(() => new Random());

        private static long _uniqueId = DateTime.Now.Ticks;

        public static string CreateUniqueAlias()
        {
            return "test_alias_" + Interlocked.Increment(ref _uniqueId);
        }

        public static byte[] CreateRandomContent()
        {
            // create thread safe Random
            var rand = _rand.Value ?? new Random();

            var buffer = new byte[rand.Next(10, 20)];
            rand.NextBytes(buffer);
            return buffer;
        }

        public static string CreateRandomString()
        {
            // reuse byte generator for simplicity
            return Convert.ToBase64String(CreateRandomContent());
        }
    }
}
