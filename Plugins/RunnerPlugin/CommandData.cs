using Newtonsoft.Json;
using System.Collections.Generic;

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
        [JsonProperty(PropertyName = "admin", Required = Required.Default)]
        public bool Admin { get; set; }
        [JsonProperty(PropertyName = "workingdir", Required = Required.Default)]
        public string WorkingDirectory { get; set; }
        [JsonProperty(PropertyName = "args", Required = Required.Default)]
        public List<string> Args { get; set; }
    }
}
