using System;
using System.IO;
using System.Linq;
using System.Text;
using Library.Models;

namespace lab4_8cs
{
    public class MyCSVWriter:IDisposable
    {
        StreamWriter Writer;

        public MyCSVWriter(string filename)
        {
            Writer = new StreamWriter(filename, false, Encoding.UTF8);
        }

        public void Write(Model model)
        {
            if(model is RecordModel recmodel)
            {
                recmodel.Title = string.Concat(Enumerable.Repeat("--|", recmodel.Depth)) + recmodel.Title;
                var str_numbers = string.Join(';', recmodel.Numbers);
                var str_addresses = string.Join(';', recmodel.Addresses);
                var tmp_string = $"{recmodel.Title},{str_numbers},{str_addresses}\n";
                Writer.Write(tmp_string);
            }
        }

        public void Dispose()
        {
            Writer.Dispose();
        }
    }
}
