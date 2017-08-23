using Hake.Extension.ValueRecord;
using System.Collections.Generic;

namespace RunnerPlugin
{
    internal sealed class CommandData
    {
        public string Command { get; set; }
        public string ExePath { get; set; }
        public string IconPath { get; set; }
        public bool Admin { get; set; }
        public string WorkingDirectory { get; set; }
        public List<string> Args { get; set; }

        public static CommandData Read(SetRecord record)
        {
            string command = record.ReadAs<string>("command");
            string path = record.ReadAs<string>("path");
            string icon = record.ReadAs<string>("icon");
            bool admin = false;
            string workingdir = null;
            List<string> args = null;
            RecordBase recordbase;
            if (record.TryGetValue("admin", out recordbase) && recordbase is ScalerRecord adminRecord && adminRecord.ScalerType == ScalerType.Boolean)
                admin = (bool)adminRecord.Value;
            if (record.TryGetValue("workingdir", out recordbase) && recordbase is ScalerRecord dirRecord && dirRecord.ScalerType == ScalerType.String)
                workingdir = (string)dirRecord.Value;
            if (record.TryGetValue("args", out recordbase) && recordbase is ListRecord argsRecord)
            {
                args = new List<string>();
                foreach (RecordBase rec in argsRecord)
                {
                    if (rec is ScalerRecord argRecord)
                        args.Add(argRecord.Value.ToString());
                }
            }
            return new CommandData()
            {
                Command = command,
                ExePath = path,
                IconPath = icon,
                Admin = admin,
                WorkingDirectory = workingdir,
                Args = args
            };
        }
        public static List<CommandData> ReadList(ListRecord record)
        {
            List<CommandData> result = new List<CommandData>();
            foreach (RecordBase rec in record)
            {
                if (rec is SetRecord recRecord)
                    result.Add(Read(recRecord));
            }
            return result;
        }
    }
}
