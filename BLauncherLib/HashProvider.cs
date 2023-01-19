using Blake2Fast;

using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLauncherLib
{
    public static class HashProvider
    {
        public static async Task<string> QuickHash(Stream stream)
        {
            return Hex.ToHexString(await MD5.HashDataAsync(stream));
        }

        public static string QuickHash(byte[] data)
        {
            return Hex.ToHexString(MD5.HashData(data));
        }

        public static async Task<string?> FileHash(string path)
        {
            if(!File.Exists(path))
            {
                return null;
            }
            var hasher = Blake2b.CreateIncrementalHasher(32);
            using var stream = File.OpenRead(path);
            var buffer = new byte[5_000_000];
            var bytesRead = 0;
            while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) != 0)
            {
                hasher.Update(buffer.AsSpan(0, bytesRead));
            }
            return Hex.ToHexString(hasher.Finish());
        }
    }
}
