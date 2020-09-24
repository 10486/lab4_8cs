using System;
using System.Diagnostics;
using System.IO;
using System.Net;           
using System.Threading;
using System.Threading.Tasks;

namespace lab4_8cs
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parser = new Parser("https://www.susu.ru/ru/structure",100,10000);
            parser.Finded += Console.WriteLine;
            await parser.Analysis();
            Console.WriteLine("asdas");
        }
        enum DataType
        {
            Number,Email
        }
        static void WriteCSV(string site, string data, DataType type)
        {
            
        }
    }
}
