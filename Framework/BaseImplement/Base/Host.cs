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
        private Task waitTask;
        private object locker = new object();
        private AutoResetEvent mutex = new AutoResetEvent(true);
        private IInternalContext lastContext;

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
                hotkey.BindKey();
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
            window.TextChanged += OnWindowTextChanged;
            window.ExecutionRequested += OnWindowExecutionRequested;
            window.HideWindow();
        }

        private void OnWindowVisibleChanged(object sender, EventArgs e)
        {
            if (window.IsVisible)
            {
                window.TextChanged += OnWindowTextChanged;
                window.ExecutionRequested += OnWindowExecutionRequested;
                window.ClearInput();
            }
            else
            {
                window.TextChanged -= OnWindowTextChanged;
                window.ExecutionRequested -= OnWindowExecutionRequested;
                pool.LeaveScope();
            }
        }

        private void OnWindowExecutionRequested(object sender, ExecutionRequestedEventArgs e)
        {
            ActionBase action = e.Action;
            if (!action.IsExecutable) return;

            try
            {
                if (lastContext == null)
                    ObjectFactory.InvokeMethod(action, "Invoke", services);
                else
                {
                    object[] args = new object[lastContext.Command.UnnamedArguments.Count + 2];
                    args[0] = lastContext;
                    args[1] = lastContext.Command;
                    if (lastContext.Command.UnnamedArguments.Count > 0)
                        lastContext.Command.UnnamedArguments.CopyTo(args, 2);
                    if (lastContext.Command.UnnamedArguments.Count > 0)
                        lastContext.Command.UnnamedArguments.CopyTo(args, 1);
                    ObjectFactory.InvokeMethod(action, "Invoke", services, lastContext.Command.NamedArguments, args);
                }
                window.HideWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnWindowTextChanged(object sender, TextUpdatedEventArgs e)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current;
            Task.Run(async () =>
            {
                mutex.WaitOne();
                if (lastContext != null && waitTask.IsCompleted == false)
                {
                    if (waitTask.IsCompleted == false)
                    {
                        lastContext.InternalCancellationProvider.Cancel();
                        waitTask.Wait();
                    }
                    lastContext.Dispose();
                }
                Command command = new Command(e.Value);
                IInternalContext context = new QuickContext(command);
                AppDelegate app = appBuilder.Build();
                await app(context);
                lastContext = context;
                syncContext.Send(s =>
                    waitTask = lastContext.WaitResults(actions).ContinueWith(tsk =>
                    {
                        if (tsk.Status == TaskStatus.RanToCompletion)
                            syncContext.Send(st => window.OnActionUpdateCompleted(), null);
                    }), null);
                mutex.Set();
            });
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
