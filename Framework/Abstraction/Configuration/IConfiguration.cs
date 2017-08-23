using Hake.Extension.ValueRecord;

namespace HakeQuick.Abstraction.Base
{
    public interface IConfiguration
    {
        RecordBase Root { get; }

        QuickConfig Options { get; }
    }
}
