using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hake.Extension.ValueRecord;
using Hake.Extension.ValueRecord.Mapper;

namespace HakeQuick.Implementation.Configuration
{
    internal sealed class Configuration : IConfiguration
    {
        public RecordBase Root { get; }

        public QuickConfig Options { get; }

        public Configuration(RecordBase values)
        {
            Root = values;
            Options = ObjectMapper.ToObject<QuickConfig>(Root);
        }
    }
}
