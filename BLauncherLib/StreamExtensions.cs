using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLauncherLib
{
    internal static class StreamExtensions
    {
        internal static string ReadFixedSizedString(this BinaryReader reader)
        {
            var size = reader.ReadInt32();
            return Encoding.ASCII.GetString(reader.ReadBytes(size));
        }

        internal static DateTime Read64BitTime(this BinaryReader reader)
        {
            return new DateTime(reader.ReadInt64());
        }
    }
}
