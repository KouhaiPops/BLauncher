namespace BLauncherLib
{
    public static class Parser
    {
        public static VersionFile ParseEncVersionFile(byte[] file)
        {
            for (int i = 0; i < file.Length; i++)
            {
                file[i] ^= (byte)(i % 0xFF + 0x69);
            }

            return ParseVersionFile(file);
        }

        public static VersionFile ParseVersionFile(byte[] file)
        {
            using var reader = new BinaryReader(new MemoryStream(file.ToArray()));
            reader.ReadBytes(0x10);
            var files = new VersionEntry[reader.ReadInt32()];
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new VersionEntry(reader.ReadFixedSizedString(), reader.ReadFixedSizedString(), reader.Read64BitTime());
            }
            return new VersionFile(files);
        }
    }
}