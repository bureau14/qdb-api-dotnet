using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDemo
{
    interface IStreamCopyObserver
    {
        void Progress(double percent);
        void Thoughtput(double kbps);
    }

    static class StreamHelper
    {
        public static async Task Copy(Stream source, Stream destination, IStreamCopyObserver observer,
            CancellationToken cancellationToken)
        {
            if (source.Length == 0) return;
            
            var buffer = new byte[1024*1024];
            var chrono = Stopwatch.StartNew();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var length = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                await destination.WriteAsync(buffer, 0, length, cancellationToken);

                observer.Progress(100.0 * source.Position / source.Length);
                observer.Thoughtput(source.Position / chrono.Elapsed.TotalMilliseconds);

                if (length < buffer.Length) break;
            }
        }
    }
}
