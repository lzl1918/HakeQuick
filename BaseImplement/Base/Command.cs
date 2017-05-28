using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Base
{
    internal sealed class Command : ICommand
    {
        public string Raw { get; }

        public string Action { get; }
        public string ActionPost { get; }
        public Dictionary<string, object> NamedArguments { get; }

        public List<object> UnnamedArguments { get; }


        public Command(string input)
        {
            Raw = input;
            Dictionary<string, object> namedargs = new Dictionary<string, object>();
            List<object> unnamedargs = new List<object>();




            NamedArguments = namedargs;
            UnnamedArguments = unnamedargs;
        }
    }
}
