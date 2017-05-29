using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Plugin;
using HakeQuick.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TestPlugin
{
    internal sealed class TestAction : ActionBase
    {
        public TestAction(string title, string content)
        {
            Assembly current = Assembly.GetExecutingAssembly();
            BitmapImage image = current.LoadImage("TestPlugin.Resources.icon.png");
            Icon = image;
            Title = title;
            Subtitle = content;
        }
    }

    [Identity("a")]
    public class APlugin : Plugin
    {
        private PlaceholderAction placeholder;
        public APlugin()
        {
            Assembly current = Assembly.GetExecutingAssembly();
            BitmapImage image = current.LoadImage("TestPlugin.Resources.icon.png");
            placeholder = new PlaceholderAction(image, "placeholder", "placeholder");
        }

        [Action("test")]
        public ActionBase Test()
        {
            return new TestAction(nameof(Test), "content of test");
        }

        [IgnoreIdentity("test")]
        public IEnumerable<ActionBase> TestB()
        {
            return new ActionBase[]
            {
                new TestAction(nameof(TestB), "content of testb"),
                new TestAction(nameof(TestB), "content of another testb"),
            };
        }

        [Action("async")]
        public AsyncActionUpdate TestC()
        {
            return AsyncActionUpdate.Create(
                placeholder,
                ActionPriority.Topmost,
                async () =>
                {
                    await Task.Delay(5000);
                    return new ActionUpdateResult(new TestAction(nameof(TestB), "content of another testb"), ActionPriority.Topmost);
                });
        }
    }
}
