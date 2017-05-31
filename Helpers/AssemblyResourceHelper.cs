using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HakeQuick.Helpers
{
    public static class AssemblyResourceHelper
    {
        public static BitmapImage LoadImage(this Assembly assembly, string resource)
        {
            Stream stream = LoadStream(assembly, resource);
            if (stream == null)
                return null;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            stream.Close();
            stream.Dispose();
            return image;
        }
        public static Stream LoadStream(this Assembly assembly, string resource)
        {
            Stream stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }
    }
}
