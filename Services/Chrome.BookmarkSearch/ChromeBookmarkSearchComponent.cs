using Chrome.BookmarkSearch.Models;
using Hake.Extension.ValueRecord;
using Hake.Extension.ValueRecord.Json;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using HakeQuick.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chrome.BookmarkSearch
{
    public sealed class ChromeBookmarkSearchComponent
    {
        public static void Initialize(ICurrentEnvironment env)
        {
            Stream defaultConfigStream = Assembly.GetExecutingAssembly().LoadStream("Chrome.BookmarkSearch.default.json");
            SetRecord defaultConfig = Converter.ReadJson(defaultConfigStream) as SetRecord;

            string configPath = env.ConfigDirectory + "\\bookmarksearch.json";
            if (File.Exists(configPath))
            {
                try
                {
                    Stream userConfigStream = File.OpenRead(configPath);
                    SetRecord userConfig = Converter.ReadJson(userConfigStream) as SetRecord;
                    userConfigStream.Dispose();
                    defaultConfig.Combine(userConfig);
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    defaultConfigStream.Seek(0, SeekOrigin.Begin);
                    FileStream configStream = File.Create(configPath);
                    defaultConfigStream.CopyTo(configStream);
                    configStream.Flush();
                    configStream.Dispose();
                }
                catch
                {
                }
            }
            defaultConfigStream.Dispose();
            ConfigurationRecord = defaultConfig;
            Config = new SearchConfig()
            {
                SearchUrl = ConfigurationRecord.ReadAs<bool>("allow_url_search"),
                EnableSearch = ConfigurationRecord.ReadAs<bool>("enable")
            };

            Bookmarks = BookmarkCollection.Retrive();
            SearchPatternErrorAction = new ErrorAction(null, "搜索模式错误", "搜索模式错误");

        }

        private static SearchConfig Config { get; set; }
        private static SetRecord ConfigurationRecord { get; set; }
        private static BookmarkCollection Bookmarks { get; set; }
        private static ErrorAction SearchPatternErrorAction { get; set; }

        private static void SearchFolder(BookmarkFolder folder, Regex searchReg, List<Bookmark> result)
        {
            if (Config.SearchUrl)
            {
                foreach (Bookmark bookmark in folder.Bookmarks)
                {
                    if (searchReg.IsMatch(bookmark.NamePinYin) || searchReg.IsMatch(bookmark.Url))
                        result.Add(bookmark);
                }
            }
            else
            {
                foreach (Bookmark bookmark in folder.Bookmarks)
                {
                    if (searchReg.IsMatch(bookmark.NamePinYin))
                        result.Add(bookmark);
                }
            }

            foreach (BookmarkFolder subfolder in folder.Folders)
            {
                SearchFolder(subfolder, searchReg, result);
            }
        }

        public Task Invoke(IQuickContext context, IProgramContext program, Func<Task> next)
        {
            if (!Config.EnableSearch)
                return next();

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
