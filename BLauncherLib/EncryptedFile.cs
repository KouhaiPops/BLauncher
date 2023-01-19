using Blake2Fast;

using S2Dotnet;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLauncherLib
{
    public static class EncryptedFile
    {
        const int BufferSize = 8196;
        public static async Task<string> DecryptFileAndHash(Stream source, IProgress<long> progress, Stream dest, CancellationToken cancellationToken)
        {
            var buffer = new byte[5_000_000];
            long totalBytesRead = 0;
            int bytesRead;
            using var s2 = new S2Stream(source, S2Stream.StreamMode.Decode);
            var hasher = Blake2b.CreateIncrementalHasher();
            while ((bytesRead = await s2.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
            {
                hasher.Update(buffer.AsSpan(0, bytesRead));
                dest.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
            return Hex.ToHexString(hasher.Finish());
        }
        public static async Task DownloadFile(Stream source, IProgress<long> progress, Stream dest)
        {
            var buffer = new byte[5_000_000];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer).ConfigureAwait(false)) != 0)
            {
                dest.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
