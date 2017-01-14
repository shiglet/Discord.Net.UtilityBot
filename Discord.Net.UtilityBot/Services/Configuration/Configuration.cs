using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace UtilityBot.Services.Configuration
{
    public sealed class Config
    {
        private Config() { }

        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("command_activation_strings")]
        public IEnumerable<string> CommandStrings { get; set; } = new[]
        {
            "#",
            "?ahh "
        };
        [JsonProperty("command_on_mention")]
        public bool TriggerOnMention { get; set; } = true;
        [JsonProperty("channels")]
        public IEnumerable<ulong> ChannelWhitelist { get; set; } = new ulong[]
        {
            81384956881809408,          // discord api      #dotnet_discord-net
            150482537465118720,         // discord.net dev  #general
        };
        [JsonProperty("elevated_user_map")]
        public Dictionary<ulong, IEnumerable<ulong>> GuildRoleMap { get; set; } = new Dictionary<ulong, IEnumerable<ulong>>
        {
            [81384788765712384] = new ulong[] // Discord API
            {
                175643578071121920,     // Mod
                111173097888993280,     // Contributor
                209033538329116682,     // Proficient
            },
            [150482537465118720] = new ulong[] // Discord.Net 1.0 Dev
            {
                151110145227751424,     // Volt
                235852482725543936,     // Contributor
                235852765216243712,     // Proficient
            }
        };

        public static Config Load()
        {
            if (File.Exists("config.json"))
            {
                var json = File.ReadAllText("config.json");
                return JsonConvert.DeserializeObject<Config>(json);
            }
            var config = new Config();
            config.Save();
            throw new InvalidOperationException("configuration file created; insert token and restart.");
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText("config.json", json);
        }
    }
}
