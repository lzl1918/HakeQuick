using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using HakeQuick.Implementation.Services.HotKey;
using HakeQuick.Implementation.Services.ProgramContext;
using HakeQuick.Implementation.Services.TerminationNotifier;
using HakeQuick.Implementation.Services.Tray;
using HakeQuick.Implementation.Services.Window;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HakeQuick.Implementation.Base
{
    internal sealed class Host : IHost
    {
        private IServiceProvider services = null;
        private IServiceCollection pool = null;
        private IAppBuilder appBuilder = null;
        private ITerminationNotifier terminationNotifier = null;
        private ITray tray = null;
        private IHotKey hotkey = null;
        private IQuickWindow window = null;
        private ObservableCollection<ActionBase> actions;

        public Host(IServiceProvider services, IServiceCollection pool, IAppBuilder appBuilder, ITerminationNotifier terminationNotifier, ITray tray, IHotKey hotkey, IQuickWindow window)
        {
            this.services = services;
            this.pool = pool;
            this.appBuilder = appBuilder;
            this.terminationNotifier = terminationNotifier;
            this.tray = tray;
            this.hotkey = hotkey;
            this.window = window;
            actions = new ObservableCollection<ActionBase>();
        }

        public void Run()
        {
            Application.ApplicationExit += OnApplicationExit;
            try
            {
                OnRun();
                tray.SendNotification(2000, "HakeQuick", "HakeQuick正在运行", ToolTipIcon.Info);
                terminationNotifier.TerminationNotified += OnTerminationNotified;
                hotkey.KeyPressed += OnHotKeyPressed;
                Application.Run();
            }
            catch
            {
                OnExit();
            }
        }

        private void OnTerminationNotified(object sender, EventArgs e)
        {
            OnExit();
        }

        private void OnHotKeyPressed(IHotKey sender, int hotkeyid)
        {
            if (window.IsVisible)
            {
                window.HideWindow();
                return;
            }
            pool.EnterScope();
            IProgramContext context = services.GetService<IProgramContext>();
            window.ClearInput();
            actions.Clear();
            window.ShowWindow(context);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            OnExit();
        }

        private void OnRun()
        {
            window.SetActions(actions);
            window.VisibleChanged += OnWindowVisibleChanged;
            window.HideWindow();
        }

        private void OnWindowVisibleChanged(object sender, EventArgs e)
        {
            if (window.IsVisible)
            {
                window.TextChanged += OnWindowTextChanged;
                window.ExecutionRequested += OnWindowExecutionRequested;
            }
            else
            {
                window.TextChanged -= OnWindowTextChanged;
                window.ExecutionRequested -= OnWindowExecutionRequested;
                pool.LeaveScope();
            }
        }

        private void OnWindowExecutionRequested(object sender, Abstraction.Action.ExecutionRequestedEventArgs e)
        {

        }

        private async void OnWindowTextChanged(object sender, TextUpdatedEventArgs e)
        {
            Command command = new Command(e.NewText);
            QuickContext context = new QuickContext(command);
            AppDelegate app = appBuilder.Build();
            actions.Clear();
            await app(context);
            foreach (ActionBase action in context.RetriveActions())
                actions.Add(action);
        }

        private void OnExit()
        {
            Application.ApplicationExit -= OnApplicationExit;

            pool.Dispose();
            Application.ExitThread();
            Application.Exit();
        }
    }
}
