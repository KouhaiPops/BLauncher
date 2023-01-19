using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLauncher.Model
{
    internal class ServerInfo
    {
        [JsonPropertyName("game-server")]
        public string GameServer { get; set; }

        [JsonPropertyName("game-port")]
        public int GamePort { get; set; }
        public string Version { get; set; }
    }
}
