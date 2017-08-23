using Hake.Extension.ValueRecord;
using Hake.Extension.ValueRecord.Mapper;
using System.Collections.Generic;

namespace RunnerPlugin
{
    internal sealed class CommandData
    {
        [MapProperty("command", MissingAction.Throw)]
        public string Command { get; set; }

        [MapProperty("path", MissingAction.Throw)]
        public string ExePath { get; set; }

        [MapProperty("icon", MissingAction.GivenValue, null)]
        public string IconPath { get; set; }

        [MapProperty("admin", MissingAction.GivenValue, false)]
        public bool Admin { get; set; }

        [MapProperty("workingdir", MissingAction.GivenValue, null)]
        public string WorkingDirectory { get; set; }

        [MapProperty("args", MissingAction.CreateInstance)]
        public List<string> Args { get; set; }
    }
}
