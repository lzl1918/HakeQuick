﻿using System.Windows.Media.Imaging;

namespace HakeQuick.Abstraction.Action
{
    public class HintAction : ActionBase
    {
        public HintAction(BitmapImage icon, string title, string subtitle)
        {
            IsExecutable = false;
            Icon = icon;
            Title = title;
            Subtitle = subtitle;
        }
    }
}
