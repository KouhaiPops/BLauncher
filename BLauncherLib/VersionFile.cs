namespace BLauncherLib
{
    public class VersionFile
    {
        public VersionEntry[] Files { get; }

        public VersionFile(VersionEntry[] files)
        {
            Files = files;
        }
    }
}