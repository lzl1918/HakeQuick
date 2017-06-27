using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Services
{
    public interface ICurrentEnvironment
    {
        DirectoryInfo MainDirectory { get; }
        DirectoryInfo PluginDirectory { get; }
        DirectoryInfo ConfigDirectory { get; }
        DirectoryInfo LogDirectory { get; }
    }
}
