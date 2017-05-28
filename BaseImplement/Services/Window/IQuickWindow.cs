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
    public enum TextUpdatedReason
    {
        UserInput,
        Programmly
    }
    public class TextUpdatedEventArgs : EventArgs
    {
        public string OldText { get; private set; }
        public string NewText { get; private set; }
        public bool ExplicitlyUpdate { get; private set; }
        public TextUpdatedReason Reason { get; private set; }

        public TextUpdatedEventArgs(string oldvalue, string newvalue, bool explicitlyUpdate, TextUpdatedReason reason)
        {
            OldText = oldvalue;
            NewText = newvalue;
            Reason = reason;
            ExplicitlyUpdate = explicitlyUpdate;
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
    }
}
