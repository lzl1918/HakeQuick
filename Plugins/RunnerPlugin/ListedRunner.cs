using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Plugin;
using HakeQuick.Abstraction.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerPlugin
{
    public sealed class ListedRunnerPlugin : QuickPlugin
    {
        private List<RunAction> actions = new List<RunAction>();

        private static string[] PREDEFINED_COMMANDS = {
            "regedit", "winword", "excel", "powerpnt", "code", "explorer"
        };

        public ListedRunnerPlugin(ICurrentEnvironment env)
        {
            string filename = "runner.json";
            string iconPath = "icons";
            string configPath = Path.Combine(env.ConfigDirectory.FullName, "runner");
            if (!Directory.Exists(configPath)) Directory.CreateDirectory(configPath);
            iconPath = Path.Combine(configPath, iconPath);
            filename = Path.Combine(configPath, filename);
            if (File.Exists(filename))
            {
                FileStream stream = File.Open(filename, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
                try
                {
                    List<CommandData> data = JsonConvert.DeserializeObject<List<CommandData>>(content);
                    foreach (CommandData cmd in data)
                    {
                        if (cmd.IconPath != null)
                            actions.Add(new RunAction(cmd.Command, cmd.ExePath, Path.Combine(iconPath, cmd.IconPath)));
                        else
                            actions.Add(new RunAction(cmd.Command, cmd.ExePath, null));
                    }
                }
                catch
                {

                }
            }
            else
            {
                List<CommandData> data = new List<CommandData>();
                foreach (string command in PREDEFINED_COMMANDS)
                {
                    data.Add(new CommandData()
                    {
                        Command = command,
                        ExePath = null,
                        IconPath = null
                    });
                    actions.Add(new RunAction(command, null, null));
                }
                FileStream stream = File.Create(filename);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                writer.Close();
                writer.Dispose();
                stream.Close();
                stream.Dispose();
            }
        }


        [ExplicitCall]
        public IEnumerable<ActionUpdateResult> OnUpdate(ICommand command)
        {
            List<ActionUpdateResult> updateResult = new List<ActionUpdateResult>();

            string identity = command.Identity;
            foreach (RunAction action in actions)
            {
                if (action.RunCommand.Length >= identity.Length && action.RunCommand.StartsWith(identity))
                {
                    updateResult.Add(new ActionUpdateResult(action, ActionPriority.Normal));
                }
            }

            return updateResult;
        }
    }
}
