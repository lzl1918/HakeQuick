using Chrome.BookmarkSearch.Models;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chrome.BookmarkSearch
{
    public sealed class ChromeBookmarkSearchComponent
    {
        static ChromeBookmarkSearchComponent()
        {
            Bookmarks = BookmarkCollection.Retrive();
            SearchPatternErrorAction = new ErrorAction(null, "搜索模式错误", "搜索模式错误");
        }

        private static BookmarkCollection Bookmarks { get; set; }
        private static ErrorAction SearchPatternErrorAction { get; set; }

        private static void SearchFolder(BookmarkFolder folder, Regex searchReg, List<Bookmark> result)
        {
            foreach (Bookmark bookmark in folder.Bookmarks)
            {
                if (searchReg.IsMatch(bookmark.NamePinYin) || searchReg.IsMatch(bookmark.Url))
                    result.Add(bookmark);
            }
            foreach (BookmarkFolder subfolder in folder.Folders)
            {
                SearchFolder(subfolder, searchReg, result);
            }
        }

        public Task Invoke(IQuickContext context, IProgramContext program, Func<Task> next)
        {
            bool isChrome = program.CurrentProcess.ProcessName == "chrome";
            if (isChrome)
            {
                string search = context.Command.Raw.Trim().ToLower();
                if (search.Length <= 0)
                    return next();

                Regex searchReg = null;
                try
                {
                    searchReg = new Regex(search);
                }
                catch
                {
                    SearchPatternErrorAction.Update("搜索模式错误", search);
                    context.AddAction(SearchPatternErrorAction);
                }
                if (searchReg != null)
                {
                    List<Bookmark> bookmarks = new List<Bookmark>();
                    SearchFolder(Bookmarks.BookmarkBar, searchReg, bookmarks);
                    SearchFolder(Bookmarks.Others, searchReg, bookmarks);
                    foreach (Bookmark bookmark in bookmarks)
                        context.AddAction(new BookmarkAction(bookmark.Name, bookmark.Url));
                }
            }
            return next();
        }
    }
}
