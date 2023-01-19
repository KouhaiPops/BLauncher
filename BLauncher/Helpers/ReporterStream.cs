using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLauncher.Helpers
{
    internal class ReporterStream : Stream
    {
        public event Action<float> Report;
        private readonly Stream _stream;
        private Stopwatch sw = new Stopwatch();
        private Queue<float> elapsed = new();
        private long bytesPerSecond;
        private float avg;
        private bool started = false;
        const int movingAvg = 20;
        
        public override bool CanRead  => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override long Length => _stream.Length;
        public override long Position { get => _stream.Position; set => _stream.Position = value; }
        
        public ReporterStream(Stream stream)
        {
            _stream = stream;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!started)
                sw.Start();
            
            var read = _stream.Read(buffer, offset, count);
            bytesPerSecond += read;
            if (sw.ElapsedMilliseconds >= 1000)
            {
                var mbSec = (bytesPerSecond/1000000f)* (1000f / sw.ElapsedMilliseconds);
                sw.Restart();
                if(elapsed.Count >= movingAvg) 
                {
                    avg -= elapsed.Dequeue();
                }
                elapsed.Enqueue(mbSec);
                avg += mbSec;
                avg /= elapsed.Count;
                Report?.Invoke(MathF.Floor(avg));
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            _stream.Dispose();
        }
    }
}
