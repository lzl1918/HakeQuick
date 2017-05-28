using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Base
{
    
    public interface ICommand
    {
        string Raw { get; }
        string Action { get; }
        string ActionPost { get; }

        Dictionary<string, object> NamedArguments { get; }
        List<object> UnnamedArguments { get; }
    }
}
