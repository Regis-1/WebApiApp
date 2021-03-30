using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Engine;

namespace WebApiApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WebConnector wc = new WebConnector("https://api.covid19api.com/");

            wc.SetRecentTotalByCountry("poland");
            foreach (CountryData cd in JsonParser.ExtractListData<CountryData>(wc.Connect()))
            {
                Console.WriteLine(cd.Country);
            }
        }
    }
}
