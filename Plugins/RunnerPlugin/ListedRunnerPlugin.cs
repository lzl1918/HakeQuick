using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Plugin;
using HakeQuick.Abstraction.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerPlugin
{
    [Identity("runner")]
    public sealed class ListedRunnerPlugin : QuickPlugin
    {
        internal static ListedRunnerPlugin Instance { get; private set; }

        private List<RunCommandAction> actions = new List<RunCommandAction>();
        private UpdateRunnerAction updateAction = new UpdateRunnerAction();

        private static string[] PREDEFINED_COMMANDS = {
            "regedit", "winword", "excel", "powerpnt", "code", "explorer"
        };

        public ListedRunnerPlugin(ICurrentEnvironment env, ILoggerFactory loggerFactory)
        {
            if (Instance != null)
                throw new Exception($"cannot create another instance of {nameof(ListedRunnerPlugin)}");

            UpdateConfigurations(env);
            loggerFactory.CreateLogger("Runner");
            Instance = this;
        }

        [Action("update")]
        public ActionUpdateResult UpdateList()
        {
            return new ActionUpdateResult(updateAction, ActionPriority.Low);
        }

        [ExplicitCall]
        public IEnumerable<ActionUpdateResult> OnUpdate(ICommand command)
        {
            List<ActionUpdateResult> updateResult = new List<ActionUpdateResult>();

            string identity = command.Identity;
            foreach (RunCommandAction action in actions)
            {
                if (action.RunCommand.Length >= identity.Length && action.RunCommand.StartsWith(identity))
                {
                    updateResult.Add(new ActionUpdateResult(action, ActionPriority.Normal));
                }
            }

            return updateResult;
        }

        internal void UpdateConfigurations(ICurrentEnvironment env)
        {
            actions.Clear();

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
                            actions.Add(new RunCommandAction(cmd.Command, cmd.ExePath, Path.Combine(iconPath, cmd.IconPath), cmd.Admin, cmd.WorkingDirectory, cmd.Args));
                        else
                            actions.Add(new RunCommandAction(cmd.Command, cmd.ExePath, null, cmd.Admin, cmd.WorkingDirectory, cmd.Args));
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
                    actions.Add(new RunCommandAction(command, null, null, false, null, null));
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
    }
}
