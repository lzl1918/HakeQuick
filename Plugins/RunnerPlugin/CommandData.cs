using Newtonsoft.Json;

namespace RunnerPlugin
{
    [JsonObject]
    internal sealed class CommandData
    {
        [JsonProperty(PropertyName = "command")]
        public string Command { get; set; }
        [JsonProperty(PropertyName = "path")]
        public string ExePath { get; set; }
        [JsonProperty(PropertyName = "icon")]
        public string IconPath { get; set; }
    }
}
