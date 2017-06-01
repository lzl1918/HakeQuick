using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Services.Window
{
    public class TextUpdatedEventArgs : EventArgs
    {
        public string Value { get; private set; }

        public TextUpdatedEventArgs(string value)
        {
            Value = value;
        }
    }
    public interface IQuickWindow
    {
        event EventHandler<TextUpdatedEventArgs> TextChanged;
        event EventHandler<ExecutionRequestedEventArgs> ExecutionRequested;
        event EventHandler VisibleChanged;

        bool IsVisible { get; }
        string RawInput { get; }

        void SetActions(ObservableCollection<ActionBase> actions);

        void HideWindow();
        void ShowWindow(IProgramContext context);

        void ClearInput();

        void OnActionUpdateCompleted();
    }
}
