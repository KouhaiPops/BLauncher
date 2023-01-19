namespace BLauncherLib
{
    public class VersionEntry
    {
        public string Name { get; }
        public  string Hash { get; }
        public DateTime Timestamp { get; }

        public VersionEntry(string name, string hash, DateTime timestamp)
        {
            Name = name;
            Hash = hash;
            Timestamp = timestamp;
        }
    }
}