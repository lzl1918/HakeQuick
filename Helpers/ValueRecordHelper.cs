using Hake.Extension.ValueRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Helpers
{
    public static class ValueRecordHelper
    {
        public static SetRecord Combine(this SetRecord dest, SetRecord source)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            RecordBase temp;
            foreach (var pair in source)
            {
                if (dest.TryGetValue(pair.Key, out temp))
                {
                    if (pair.Value is SetRecord srcset && temp is SetRecord dstset)
                        Combine(dstset, srcset);
                    else
                        dest[pair.Key] = pair.Value;
                }
                else
                    dest[pair.Key] = pair.Value;
            }
            return dest;
        }
    }
}
