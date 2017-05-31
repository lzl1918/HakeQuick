using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HakeQuick.Implementation.Services.Tray
{
    public interface ITray
    {
        void SendNotification(int timeout, string title, string content, ToolTipIcon icon = ToolTipIcon.Info);
    }
}
