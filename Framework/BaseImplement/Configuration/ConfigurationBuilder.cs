using Hake.Extension.ValueRecord;
using Hake.Extension.ValueRecord.Json;
using HakeQuick.Abstraction.Base;
using HakeQuick.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Configuration
{
    public sealed class ConfigurationBuilder : IConfigurationBuilder
    {
        private SetRecord values = null;

        public ConfigurationBuilder()
        {

        }


        public IConfiguration Build()
        {
            if (values == null)
                throw new Exception("cannot build configuartions due to empty value container");
            return new Configuration(values);
        }


        public ConfigurationBuilder AddDefault()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            Stream stream = ass.LoadStream("HakeQuick.default.json");
            RecordBase record = Converter.ReadJson(stream);
            stream.Dispose();

            if (record is SetRecord set)
            {
                if (values == null)
                    values = set;
                else
                    CombineValue(values, set);
            }
            else
            {
                throw new Exception("invalid configuration format");
            }

            return this;
        }
        public ConfigurationBuilder AddJson(string file)
        {
            Stream stream = File.OpenRead(file);
            RecordBase record = Converter.ReadJson(stream);
            stream.Dispose();
            if (record is SetRecord set)
            {
                if (values == null)
                    values = set;
                else
                    CombineValue(values, set);
            }
            else
            {
                throw new Exception("invalid configuration format");
            }
            return this;
        }
        public ConfigurationBuilder TryAddJson(string file)
        {
            if (File.Exists(file))
                return AddJson(file);
            else
                return this;
        }

        private void CombineValue(SetRecord dest, SetRecord source)
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
                        CombineValue(dstset, srcset);
                    else
                        dest[pair.Key] = pair.Value;
                }
                else
                    dest[pair.Key] = pair.Value;
            }
        }
    }
}
