using System;

namespace Library.Models
{
    public class RecordModel:Model
    {
        public string[] Addresses;
        public string[] Numbers;

        public RecordModel(int depth, string title, string[] addresses, string[] numbers)
        {
            if (depth < 0) throw new ArgumentOutOfRangeException("depth must be greater than 0");
            if (addresses is null) throw new ArgumentNullException("addresses is null");
            if (numbers is null) throw new ArgumentNullException("numbers is null");

            Depth = depth;
            Title = title;
            Addresses = addresses;
            Numbers = numbers;
        }
    }
}
