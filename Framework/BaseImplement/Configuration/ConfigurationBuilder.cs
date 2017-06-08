using Hake.Extension.ValueRecord;
using Hake.Extension.ValueRecord.Json;
using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Configuration
{
    public sealed class ConfigurationBuilder : IConfigurationBuilder
    {
        private RecordBase values = null;

        public ConfigurationBuilder()
        {

        }


        public IConfiguration Build()
        {
            if (values == null)
                throw new Exception("cannot build configuartions due to empty value container");
            return new Configuration(values);
        }

        public ConfigurationBuilder AddJson(string file)
        {
            Stream stream = File.OpenRead(file);
            RecordBase record = Converter.ReadJson(stream);
            stream.Dispose();

            if (values == null)
                values = record;
            else
            {
                // TODO: combine values
                throw new NotImplementedException();
            }
            return this;
        }
        
    }
}
