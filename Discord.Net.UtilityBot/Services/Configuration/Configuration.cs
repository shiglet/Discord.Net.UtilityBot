using Newtonsoft.Json;
using System;
using System.IO;

namespace UtilityBot.Services.Configuration
{
    public sealed class Configuration
    {
        private Configuration() { }

        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("command_activation_string")]
        public string CommandCharacter { get; set; } = "$ ";
        [JsonProperty("command_on_mention")]
        public bool TriggerOnMention { get; set; } = true;

        public static Configuration Load()
        {
            if (File.Exists("config.json"))
            {
                var json = File.ReadAllText("config.json");
                return JsonConvert.DeserializeObject<Configuration>(json);
            }
            var config = new Configuration();
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
