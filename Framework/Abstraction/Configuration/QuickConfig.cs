using Hake.Extension.ValueRecord.Mapper;
using System;
using System.IO;
using System.Windows.Input;

namespace HakeQuick.Abstraction.Base
{
    public sealed class HotkeyConfig
    {
        [MapProperty("key", MissingAction.GivenValue, "Q")]
        public string KeyString { get; set; }

        [MapProperty("flags", MissingAction.GivenValue, "Control")]
        public string FlagsString { get; set; }

        public Key Key { get { return (Key)Enum.Parse(typeof(Key), KeyString); } }
        public KeyFlags KeyFlags
        {
            get
            {
                string[] keyflags = FlagsString.Split('+');
                KeyFlags hotkeyFlags = KeyFlags.None;
                if (keyflags.Length > 0)
                    foreach (string keyflag in keyflags)
                        hotkeyFlags |= (KeyFlags)Enum.Parse(typeof(KeyFlags), keyflag);
                if (hotkeyFlags == KeyFlags.None)
                    hotkeyFlags = KeyFlags.Control;
                return hotkeyFlags;
            }
        }
    }

    public sealed class QuickConfig
    {
        [MapProperty("config", MissingAction.GivenValue, ".\\configs")]
        public string ConfigPath { get; set; }

        [MapProperty("log", MissingAction.GivenValue, ".\\log")]
        public string LogPath { get; set; }

        [MapProperty("hotkey", MissingAction.CreateInstance)]
        public HotkeyConfig Hotkey { get; set; }
    }
}
