using System;
using System.Collections.Generic;
using System.Net.Http;
using Library.Models;
using Library.Analyzers;

namespace Library
{
    public class Parser
    {
        Uri URI;
        int Depth;
        int MaxCount;
        int Count = 0;
        HttpClient Client;
        IAnalyzer Analyzer;
        List<Uri> Analyzed = new List<Uri>();
        public event Action<Model> Finded;

        public Parser(Uri uri, int depth, int maxCount, HttpClient client, IAnalyzer analyzer)
        {
            #region Проверки
            if (uri is null) throw new ArgumentNullException("uri is null");
            if (depth <= 1) throw new ArgumentOutOfRangeException("depth must be greater than 1");
            if (maxCount <= 1) throw new ArgumentOutOfRangeException("maxCount must be greater than 1");
            if (client is null) throw new ArgumentNullException("client is null");
            if (analyzer is null) throw new ArgumentNullException("analyzer is null");
            #endregion
            URI = uri;
            Depth = depth;
            MaxCount = maxCount;
            Client = client;
            Analyzer = analyzer;
            Analyzer.Finded += s => Finded.Invoke(s);
        }

        public void StartParse()
        {
            Parse(URI, 0);
        }

        private void Parse(Uri uri, int depth)
        {
            if (uri == null) throw new ArgumentNullException("uri is null");
            if (depth < 0) throw new ArgumentOutOfRangeException("wrong depth value");

            if (Depth <= depth) return;
            if (MaxCount <= Count) return;
            var response  = Request(uri);
            if (string.IsNullOrWhiteSpace(response))
            {
                return;
            }
            var EnumUri = Analyzer.CheckURI(response, uri, depth);
            Analyzer.Analisys(response, uri, depth);
            Analyzed.Add(uri);
            Count++;
            foreach (var item in EnumUri)
            {
                if (Analyzed.Contains(item)) continue;
                Parse(item, depth + 1);
            }   
            return;
        }

        /// <summary>
        /// Получает html документ по указанному uri,
        /// если тип документа не html возвращает null
        /// </summary>
        /// <param name="uri">Адрес документа</param>
        /// <returns>Документ или null в случае несоответсвия типов</returns>
        private string Request(Uri uri)
        {
            try
            {
                var response = Client.GetAsync(uri).Result;
                if (response.Content.Headers.ContentType.MediaType != "text/html")
                {
                    return null;
                }
                var html = response.Content.ReadAsStringAsync().Result;
                return html;
            }
            catch (AggregateException)
            {
                throw new Exception("Wrong URI Exception");
            } 
        }
    }
}
