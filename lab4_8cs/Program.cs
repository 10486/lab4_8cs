using System;
using System.Net.Http;
using Library.Models;
using Library.Analyzers;
using Library;

namespace lab4_8cs
{
    class Program
    {
        static string URI = "https://www.cheladmin.ru/";
        static void Main(string[] args)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);
            Uri uri = new Uri(URI);
            var parser = new Parser(uri, 10, 100, client, new AnalyzerCheladmin(uri));
            using var csv = new MyCSVWriter("data.csv");
            parser.Finded += PrintRecord;
            parser.Finded += csv.Write;
            parser.StartParse();
        }

        static void PrintRecord(Model model) 
        {
            if(model is RecordModel)
            {
                var recmodel = model as RecordModel;
                Console.WriteLine(recmodel.Title);
                foreach (var i in recmodel.Addresses) Console.WriteLine(i);
                Console.WriteLine(new string('-', 15));
                foreach (var i in recmodel.Numbers) Console.WriteLine(i);
                Console.WriteLine(new string('*', 15));
            }
            else
            {
                throw new ArgumentException();
            }
            
        }
    }
}
