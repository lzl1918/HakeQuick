using Hake.Extension.ValueRecord;
using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome.BookmarkSearch.Models
{

    public sealed class Bookmark
    {
        public string NamePinYin { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public sealed class BookmarkFolder
    {
        public string Name { get; set; }
        public List<BookmarkFolder> Folders { get; set; }
        public List<Bookmark> Bookmarks { get; set; }
    }
    public sealed class BookmarkCollection
    {
        public BookmarkFolder BookmarkBar { get; set; }
        public BookmarkFolder Others { get; set; }

        private static BookmarkCollection CreateEmpty()
        {
            return new BookmarkCollection()
            {
                BookmarkBar = new BookmarkFolder()
                {
                    Name = "书签栏",
                    Bookmarks = new List<Bookmark>(),
                    Folders = new List<BookmarkFolder>()
                },
                Others = new BookmarkFolder()
                {
                    Name = "其他",
                    Bookmarks = new List<Bookmark>(),
                    Folders = new List<BookmarkFolder>()
                }
            };
        }
        private static Bookmark RetriveBookmark(SetRecord record)
        {
            string name = record.ReadAs<string>("name");
            ChineseChar ch;
            StringBuilder builder = new StringBuilder(name.Length);
            foreach (char c in name)
            {
                if (ChineseChar.IsValidChar(c))
                {
                    ch = new ChineseChar(c);
                    builder.Append(ch.Pinyins[0]);
                    builder.Remove(builder.Length - 1, 1);
                }
                else
                    builder.Append(c);
            }
            string pinyin = builder.ToString().ToLower();
            return new Bookmark()
            {
                Name = name,
                Url = record.ReadAs<string>("url"),
                NamePinYin = pinyin
            };
        }
        private static BookmarkFolder RetriveFolder(SetRecord record)
        {
            string name = record.ReadAs<string>("name");
            string type;
            ListRecord children = record["children"] as ListRecord;
            List<Bookmark> bookmarks = new List<Bookmark>();
            List<BookmarkFolder> folders = new List<BookmarkFolder>();
            foreach (RecordBase rec in children)
            {
                if (rec is SetRecord set)
                {
                    type = set.ReadAs<string>("type");
                    if (type == "folder")
                    {
                        folders.Add(RetriveFolder(set));
                    }
                    else if (type == "url")
                    {
                        bookmarks.Add(RetriveBookmark(set));
                    }
                }
            }
            return new BookmarkFolder()
            {
                Name = name,
                Bookmarks = bookmarks,
                Folders = folders
            };
        }
        public static BookmarkCollection Retrive()
        {
            string appdata = Environment.GetEnvironmentVariable("appdata");
            string filepath = Path.Combine(appdata, @"..\local\Google\Chrome\User Data\Default\Bookmarks");
            if (!File.Exists(filepath))
                return CreateEmpty();

            Stream stream = File.OpenRead(filepath);
            SetRecord record = Hake.Extension.ValueRecord.Json.Converter.ReadJson(stream) as SetRecord;
            stream.Dispose();
            if (record == null)
                return CreateEmpty();
            SetRecord bookmarkbar = record.FromPath("roots.bookmark_bar") as SetRecord;
            SetRecord others = record.FromPath("roots.other") as SetRecord;
            return new BookmarkCollection()
            {
                BookmarkBar = RetriveFolder(bookmarkbar),
                Others = RetriveFolder(others)
            };
        }
    }
}
