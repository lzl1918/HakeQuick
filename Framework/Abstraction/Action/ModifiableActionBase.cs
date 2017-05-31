using System;
using System.Windows.Media.Imaging;

namespace HakeQuick.Abstraction.Action
{
    public abstract class ModifiableActionBase : ActionBase
    {
        public void Update(string title, string subtitle)
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));
            if (subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));

            Title = title;
            Subtitle = subtitle;
        }
        public void Update(BitmapImage icon, string title, string subtitle)
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));
            if (subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));

            Icon = icon;
            Title = title;
            Subtitle = subtitle;
        }

        public void SetExecutable(bool executable)
        {
            IsExecutable = executable;
        }
    }
}
