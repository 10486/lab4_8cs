using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Models;

namespace Library.Analyzers
{
    public class AnalyzerCheladmin : IAnalyzer
    {
        public event Action<Model> Finded;
        string[] WrongLinkFormat = { ".css", ".js", ".png", ".jpg", ".jpeg", ".ico", ".svg", ".gif", ".doc", ".pdf","/offline/", ".rtf", ".xlsx", ".zip" };
        Uri Uri;
        Regex Link;
        Regex Address;
        Regex Number;
        Regex Title;

        public AnalyzerCheladmin(Uri uri)
        {
            Uri = uri ?? throw new ArgumentNullException("uri is null");
            Title = new Regex(@"<title>.+</title>");
            Link = new Regex(@$"=""(https?://{Uri.Host})?/[\S]+""");
            Address = new Regex(@"Адрес:.+?<");
            Number = new Regex(@"\s((\+7 )?(\(351\)) )?[0-9]{3}-[0-9]{2}-[0-9]{2}");
        }

        public void Analisys(string html_doc, Uri path, int depth)
        {
            if (string.IsNullOrWhiteSpace(html_doc)) throw new ArgumentException("wrong html_doc");
            if (path is null) throw new ArgumentNullException("path is null");

            var title = FindTitle(html_doc);

            var ad = Address
                .Matches(html_doc)
                .Cast<Match>()
                .Select(match => match.Value)
                .ToArray();

            var numbers = Number
                .Matches(html_doc)
                .Cast<Match>()
                .Select(match => match.Value)
                .ToArray();

            // title[7..(title.Length - 8)] убирает теги title
            Finded.Invoke(new RecordModel(depth, title[7..(title.Length - 8)], ad, numbers));
        }

        /// <summary>
        /// Ищет title в html документе
        /// </summary>
        /// <param name="html_doc">html документ</param>
        /// <returns>title html документа</returns>
        private string FindTitle(string html_doc)
        {
            return Title.Match(html_doc).Value;
        }

        public IEnumerable<Uri> CheckURI(string html_doc, Uri uri, int depth)
        {
            var links = Link.Matches(html_doc).ToList();
            var strings = new string[links.Count];

            for (int i = 0; i < links.Count; i++)
            {
                strings[i] = links[i].Value[2..(links[i].Length - 1)];
            }

            var UriList = new List<Uri>();
            foreach (var item in strings)
            {
                if (!CheckFormat(item))
                {
                    continue;
                }
                if (item[0] == '/')
                {
                    UriList.Add(new Uri(uri, item));
                }
                else
                {
                    UriList.Add(new Uri(item));

                }
            }
            return UriList.Distinct();
        }

        /// <summary>
        /// Проверяет формат документа в ссылке
        /// </summary>
        /// <param name="item">Ссылка</param>
        /// <returns>true если формат верный, в ином случае false</returns>
        private bool CheckFormat(string item)
        {
            foreach (var format in WrongLinkFormat)
            {
                if (item.Contains(format))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
