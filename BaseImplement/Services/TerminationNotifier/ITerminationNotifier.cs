using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Services.TerminationNotifier
{
    public interface ITerminationNotifier
    {
        event EventHandler TerminationNotified;
        void NotifyTerminate();
    }
}
