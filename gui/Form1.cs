using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace gui
{
    public partial class Form1 : Form
    {
        private WebConnector wc;
        public Form1()
        {
            //Creating webconnector
            wc = new WebConnector("https://api.covid19api.com/");
            InitializeComponent();

            DateTime today = DateTime.Today;
            var context = new DataBase();
            bool existance = false;

            if (context.GDB.Any(record => record.DateDataBase == today)) existance = true;

            if (!existance)
            {
                wc.SetGlobalSummary();
                GlobalData gd1 = JsonParser.ExtractSingleData<GlobalData>(wc.Connect(), "Global");
                context.GDB.Add(new GlobalDataBase { TotalConfirmed = gd1.TotalConfirmed, DateDataBase = today });
                context.SaveChanges();

            }

            if (existance) Console.WriteLine("Todays data already in the data base.");
            else if (!existance) Console.WriteLine("Todays data added.");

            var globalDataSets = (from s in context.GDB select s).ToList<GlobalDataBase>();
            lbGlobal.Items.Clear();
            lbGlobal.Items.Add("Global summary history:");
            foreach (var st in globalDataSets)
            {
                lbGlobal.Items.Add("Cases: "+st.TotalConfirmed.ToString()+" - "+st.DateDataBase.ToString("dd/MM/yyyy"));
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            DateTime t1 = dtpFrom.Value;
            DateTime t2 = dtpTo.Value;
            string country = cbCountry.SelectedItem.ToString().ToLower();
            if(t1.Date == t2.Date)
            {
                lbResults.Items.Clear();
                wc.SetRecentTotalByCountry(country);
                CountryData cd2 = JsonParser.ExtractListData<CountryData>(wc.Connect())[0];
                lbResults.Items.Add("Confirmed: "+cd2.Confirmed.ToString());
            }
            else if(DateTime.Compare(t1, t2) < 0)
            {
                lbResults.Items.Clear();
                lbResults.Items.Add("Confirmed last " + (t2.Subtract(t1).Days-1).ToString() + " days:");
                wc.SetPeriodTotalByCountry(country, t1, t2);
                List<CountryData> cdl = JsonParser.ExtractListData<CountryData>(wc.Connect());
                int _c = 0;
                foreach(CountryData cd in cdl)
                {
                    lbResults.Items.Add((_c+1).ToString()+ ". " + cd.Confirmed.ToString());
                    _c++;
                }
            }
            else
            {
                MessageBox.Show("Date 'from' is later than 'to'!");
            }
        }
    }
}
