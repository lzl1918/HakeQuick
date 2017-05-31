using HakeQuick.Helpers;
using HakeQuick.Implementation.Services.TerminationNotifier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HakeQuick.Implementation.Services.Tray
{
    internal sealed class Tray : ITray, IDisposable
    {
        private ITerminationNotifier terminationNotifier;
        private NotifyIcon tray = null;
        public Tray(ITerminationNotifier terminationNotifier)
        {
            this.terminationNotifier = terminationNotifier;

            tray = new NotifyIcon();
            Assembly assembly = Assembly.GetEntryAssembly();
            Stream iconStream = assembly.LoadStream("HakeQuick.tray.ico");
            tray.Icon = new Icon(iconStream);
            iconStream.Close();
            iconStream.Dispose();
            tray.Visible = true;
            MenuItem closeMenu = new MenuItem("关闭", (sender, e) =>
            {
                this.terminationNotifier.NotifyTerminate();
            });
            MenuItem[] menuitems = new MenuItem[] { closeMenu };
            tray.ContextMenu = new ContextMenu(menuitems);
        }

        private bool disposed = false;
        ~Tray()
        {
            if (!disposed)
                Dispose();
        }
        public void Dispose()
        {
            if (disposed)
                return;

            tray.Visible = false;
            tray.Dispose();
            disposed = true;
        }

        public void SendNotification(int timeout, string title, string content, ToolTipIcon icon)
        {
            tray.ShowBalloonTip(timeout, title, content, icon);
        }
    }
}
