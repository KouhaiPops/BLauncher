using BLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BLauncher.Model
{
    public sealed class Config
    {
        private static readonly JsonSerializerOptions options = new()
        {
            Converters =
            {
                new Helpers.JsonConverters.StringKVPConverter()
            }
        };

        public string CurrentlyLoggedIn { get; set; }
        public Dictionary<string, string> LoggedIn { get; set; } = new();
        public string Gamepath { get; set; }

        public static Config Load()
        {
            Config config = new();
            if (File.Exists("config.json"))
            {

                try
                {
                    config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"), options);
                    // This is redundent and instead we should deserialize into the cache.
                    foreach(var loggedIn in config.LoggedIn)
                    {
                        Cache.CachedLogIns.Add(loggedIn);
                    }
                    Cache.UpdateLoggedIn(config.CurrentlyLoggedIn);
                }
                catch (Exception ex)
                {
                    App.HandleException(ex);
                    Environment.Exit(1);
                }
            }
            return config;
        }

        public void Save()
        {
            File.WriteAllText("config.json", JsonSerializer.Serialize(this, options));
        }



        internal bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Gamepath) && Directory.Exists(Gamepath);
        }

        public string GetFileInPath(string file)
        {
            var root = Path.GetDirectoryName(file);
            if(root.Length > 1)
            {
                CreateDirectory(root);
            }
            return Path.Combine(Gamepath, file);
        }

        private void CreateDirectory(string root)
        {
            var dir = Path.Combine(Gamepath, root);
            if (Directory.Exists(dir))
                return;
            Directory.CreateDirectory(dir);
        }
    }
}
