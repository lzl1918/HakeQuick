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

        public ConfigurationBuilder ReplaceAll(SetRecord record)
        {
            values = record;
            return this;
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
                    values.Combine(set);
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
                    values.Combine(set);
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
    }
}
