using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace UtilityBot.Services.Tags
{
    public class TagDb
    {
        private TagDb() { }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; } = new List<Tag>();

        public Dictionary<string, Tag> TagMap { get; private set; } = new Dictionary<string, Tag>();

        public void RebuildMap()
        {
            TagMap = new Dictionary<string, Tag>();
            foreach (var tag in Tags)
            {
                TagMap.Add(tag.Name, tag);
                foreach (var alias in tag.Aliases) TagMap.Add(alias, tag);
            }
        }

        public static TagDb Load()
        {
            if (File.Exists("tags.json"))
            {
                var json = File.ReadAllText("tags.json");
                return JsonConvert.DeserializeObject<TagDb>(json);
            }
            var db = new TagDb();
            db.Save();
            return db;
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText("tags.json", json);
        }
    }

    public sealed class Tag
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public ulong OwnerId { get; set; }
        public string Content { get; set; }
    }
}
