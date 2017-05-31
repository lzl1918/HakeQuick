using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
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
    public class APlugin : QuickPlugin
    {
        private PlaceholderAction placeholder;
        public APlugin()
        {
            Assembly current = Assembly.GetExecutingAssembly();
            BitmapImage image = current.LoadImage("TestPlugin.Resources.icon.png");
            placeholder = new PlaceholderAction(image, "placeholder", "placeholder");
        }

        [Action("test")]
        public ActionUpdateResult Test()
        {
            return new ActionUpdateResult(new TestAction(nameof(Test), "content of test"), ActionPriority.Normal);
        }

        [IgnoreIdentity("test")]
        public IEnumerable<ActionUpdateResult> TestB()
        {
            return new ActionUpdateResult[]
            {
                new ActionUpdateResult(new TestAction(nameof(TestB), "content of testb"), ActionPriority.Normal),
                new ActionUpdateResult(new TestAction(nameof(TestB), "content of another testb"), ActionPriority.Normal)
            };
        }

        [Action("async")]
        public AsyncActionUpdate TestC(IQuickContext context)
        {
            return AsyncActionUpdate.Create(
                placeholder,
                ActionPriority.Topmost,
                async () =>
                {
                    await Task.Delay(5000, context.CancellationProvider.CancellationToken);
                    return new ActionUpdateResult(new TestAction(nameof(TestB), "content of another testb"), ActionPriority.Topmost);
                });
        }
    }
}
