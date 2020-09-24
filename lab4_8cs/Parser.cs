using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace lab4_8cs
{
    class Parser
    {
        string URI;
        int Deep;
        int MaxCount;
        int Count;
        Regex Adress;
        Regex Number;
        Regex Hrefs;
        string Domain;
        List<string> completed = new List<string>();
        HttpClient client;
        public event Action<string> Finded;
        public Parser(string uri, int deep, int max)
        {
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                MaxConnectionsPerServer = 10
        };

            // Pass the handler to httpclient(from you are calling api)
            client = new HttpClient(clientHandler);
            URI = uri;
            Deep = deep;
            MaxCount = max;
            Adress = new Regex(@"\S+\@\S+\.\S+");
            Number = new Regex(@"(((\+?7?)|8?)-)?\(?[0-9]{3}\)?-[0-9]{3}-[0-9]{2}-[0-9]{2}");
            Hrefs = new Regex(@"((https?:\/\/)([\w-]{1,32}\.[\w-]{1,32})([^\s\@>""]*))|([^<](\/[\w-]+)+)");
            Domain = (new Regex(@"https ?://(www\.)?[^\s/]+")).Match(URI).Value;

        }
        public async Task Analysis()
        {
            await SearchInURI(URI, 0);
        }
        async Task<string> SearchInURI(string uri, int deep)
        {
            string[] hrefs;
            try
            {
                var response = client.GetAsync(uri).Result;
                if (!response.IsSuccessStatusCode) return "";
                hrefs = GetInfoFromString(await response.Content.ReadAsStringAsync());
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
            
            if (deep + 1 == Deep) return uri;
            var tasks = new List<Task<string>>(); 
            foreach (var item in hrefs)
            {
                if (Count >= MaxCount) return uri;

                if (completed.Contains(item)) continue;

                tasks.Add(SearchInURI(item[0]=='"'?Domain+item[1..]:item, deep + 1));
                Count++;
            }
            foreach (var item in tasks)
            {
                completed.Add(await item);
            }
            return uri;
        }
        string[] GetInfoFromString(string data)
        {
            data = new Regex(@"<body[\s\S]+</body>").Match(data).Value;
            var col = Adress.Matches(data);
            foreach (var item in col)
            {
                Finded.Invoke(item.ToString());
            }

            col = Number.Matches(data);
            foreach (var item in col)
            {
                Finded.Invoke(item.ToString());
            }

            col = Hrefs.Matches(data);
            var s = new HashSet<string>((from item in col where VrongFormat(item.Value) select item.Value)).ToArray();
            return s;
        }
        bool VrongFormat(string data)
        {
            return (data.Contains(Domain) && !data.Contains(".js") && !data.Contains(".png") && !data.Contains(".css") && !data.Contains(".pdf") && !data.Contains(".jpg") && !data.Contains(".ico"))||data[0]=='"';
        }
    }
}
