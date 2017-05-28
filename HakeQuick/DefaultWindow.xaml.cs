using HakeQuick.Implementation.Services.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Services;
using System.Collections.ObjectModel;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using HakeQuick.Helpers;

namespace HakeQuick
{
    public partial class DefaultWindow : Window, IQuickWindow
    {
        public string RawInput
        {
            get { return textbox_input.Text; }
            set
            {
                textbox_input.Text = value;
            }
        }

        public event EventHandler<TextUpdatedEventArgs> TextChanged;
        public event EventHandler<ExecutionRequestedEventArgs> ExecutionRequested;
        public event EventHandler VisibleChanged;


        private ObservableCollection<ActionBase> m_actions = null;
        private IntPtr hwnd;

        public DefaultWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            hwnd = new WindowInteropHelper(this).Handle;
            GlassHelper.EnableAero(hwnd);

            textbox_input.TextChanged += OnTextChanged;
            IsVisibleChanged += OnIsVisibleChanged;
            list_actions.PreviewMouseLeftButtonDown += OnListPreviewLeftMouseButtonDown;
            Deactivated += OnDeactived;
            ElementHost.EnableModelessKeyboardInterop(this);
        }

        private void OnDeactived(object sender, EventArgs e)
        {
            HideWindow();
        }

        private void OnListPreviewLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Handled)
                return;

            TextChanged?.Invoke(this, new TextUpdatedEventArgs(null, textbox_input.Text, false, TextUpdatedReason.UserInput));
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisibleChanged?.Invoke(this, null);
        }

        public void ClearInput()
        {
            textbox_input.Text = "";
        }

        public void HideWindow()
        {
            if (IsVisible == false)
                return;
            this.Hide();
        }

        public void SetActions(ObservableCollection<ActionBase> actions)
        {
            list_actions.ItemsSource = actions;
            m_actions = actions;
        }

        public void ShowWindow(IProgramContext context)
        {
            if (IsVisible == true)
                return;
            RECT position = context.WindowPosition;
            double ttop = position.Top + 50;
            double halfwidthdiff = ((position.Right - position.Left) - Width) / 2;
            double tleft = position.Left + halfwidthdiff;
            if (tleft < 0 && tleft + Width > 0)
                tleft = -(Width + 50);

            if (ttop < 50)
                ttop = 50;
            Left = tleft;
            Top = ttop;
            Show();
            Activate();
            textbox_input.Focus();
        }
    }
}
