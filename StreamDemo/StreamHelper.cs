using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDemo
{
    static class StreamHelper
    {
        internal struct CopyStatus
        {
            public long BytesCopied { get; set; }
            public long TotalBytes { get; set; }
            public TimeSpan Elapsed { get; set; }
        }

        public static async Task Copy(Stream source, Stream destination, Action<CopyStatus> onProgress,
            CancellationToken cancellationToken)
        {
            if (source.Length == 0) return;
            
            var buffer = new byte[1024*1024];
            var chrono = Stopwatch.StartNew();
            int length;

            do
            {
                length = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                await destination.WriteAsync(buffer, 0, length, cancellationToken);

                onProgress(new CopyStatus
                {
                    BytesCopied = source.Position,
                    Elapsed = chrono.Elapsed,
                    TotalBytes = source.Length
                });

            } while (length == buffer.Length);
        }
    }
}
