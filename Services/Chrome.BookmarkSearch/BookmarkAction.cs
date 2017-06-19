using HakeQuick.Abstraction.Action;
using System.Diagnostics;

namespace Chrome.BookmarkSearch
{
    public sealed class BookmarkAction : ActionBase
    {
        public string Url { get; }
        public string Name { get; }

        public BookmarkAction(string name, string url)
        {
            Title = Name = name;
            Subtitle = Url = url;
            IsExecutable = true;
        }

        public void Invoke()
        {
            ProcessStartInfo proc = new ProcessStartInfo("chrome.exe", Url);
            Process.Start(proc);
        }
    }
}
