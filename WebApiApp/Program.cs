using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Engine;
using System.Data.Entity;

namespace WebApiApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WebConnector wc = new WebConnector("https://api.covid19api.com/");

            #region SetPeriodTotalByCountry
            DateTime t1 = new DateTime(2021, 3, 23);
            DateTime t2 = new DateTime(2021, 3, 26);

            wc.SetPeriodTotalByCountry("poland", t1, t2);
            foreach (CountryData cd1 in JsonParser.ExtractListData<CountryData>(wc.Connect()))
            {
                Console.WriteLine(cd1.Country + ": " +cd1.Confirmed.ToString());
            }
            Console.WriteLine();
            #endregion

            #region SetRecentTotalByCountry
            wc.SetRecentTotalByCountry("france");
            CountryData cd2 = JsonParser.ExtractListData<CountryData>(wc.Connect())[0];
            Console.WriteLine(cd2.Country + ": " + cd2.Confirmed.ToString()+"\n");
            #endregion

            #region SetGlobalSummary
            wc.SetGlobalSummary();
            GlobalData gd1 = JsonParser.ExtractSingleData<GlobalData>(wc.Connect(), "Global");
            Console.WriteLine("Global: " + gd1.TotalConfirmed.ToString()+"\n");
            #endregion

            DateTime today = DateTime.Now;
            var context = new DataBase();
            bool existance = false;

            if (context.GDB.Any(record => record.DateDataBase == today.Date)) existance = true;
            
            if (!existance)
            {
                context.GDB.Add(new GlobalDataBase { TotalConfirmed = gd1.TotalConfirmed, DateDataBase = today.Date });
                //var st1 = context.GDB.First(x => x.GlobalDataBaseId == 1);
                //context.GDB.Remove(st1);
                context.SaveChanges();

            }

            if (existance) Console.WriteLine("Todays data already in the data base.");
            else if (!existance) Console.WriteLine("Todays data added.");

            var globalDataSets = (from s in context.GDB select s).ToList<GlobalDataBase>();
            foreach (var st in globalDataSets)
            {
                Console.WriteLine("ID: {0}, Total Confirmed Cases: {1}, Date: {2}", st.GlobalDataBaseId, st.TotalConfirmed, st.DateDataBase);
            }

        }
    }
}
