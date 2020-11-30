using System;
using System.Collections.Generic;
using Library.Models;

namespace Library.Analyzers
{
    public interface IAnalyzer
    {
        /// <summary>
        /// Вызывает событие Finded когда находит нужную иформацию
        /// </summary>
        /// <param name="html_doc">HTML документ</param>
        public void Analisys(string html_doc, Uri path, int depth);

        /// <summary>
        /// Ищет все ссылки в html_doc
        /// </summary>
        /// <param name="html_doc">Исследуемый документ</param>
        /// <param name="uri">Адрес html_doc</param>
        /// <returns>Все встреченные ссылки на html документы</returns>
        public IEnumerable<Uri> CheckURI(string html_doc, Uri uri, int depth);

        public event Action<Model> Finded;
    }
}
