using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for StatsControl.xaml
    /// </summary>
    public partial class StatsControl : UserControl
    {
        string calendarChartPath = "Charts/calendar.html";
        string columnsChartPath = "Charts/column_chart.html";
        string lineChartPath = "Charts/line_chart.html";
        string timeOfDayColumnsChartPath = "Charts/timeofday_column_chart.html"; // [{v: [8, 30], f: '8:30 AM'}, 2]

        List<Auction> itemsBought;

        public StatsControl()
        {
            InitializeComponent();

            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;

            itemsBought = DB.GetItemsBought(Session.LoggedUser);
        }

        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            DisplayChart();
        }

        // loading spinning thing
        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                this.Dispatcher.Invoke(() =>
                {
                    loadingIcon.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void LoadHtml(string _html)
        {
            var base64EncodedHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(_html));
            browser.Load("data:text/html;base64," + base64EncodedHtml);
            loadingIcon.Visibility = Visibility.Visible;
        }

        private string GetCalendarChartHTML()
        {
            Dictionary<string, int> datePurchaseCount = new Dictionary<string, int>(); // dictionary formed by each date of purchase and number of purchases on that date

            string rows = "";
            // template : [ new Date(2012, 3, 13), 37032 ]
            foreach (Auction auction in itemsBought)
            {
                if (!datePurchaseCount.ContainsKey(auction.End.ToShortDateString()))
                {
                    datePurchaseCount.Add(auction.End.ToShortDateString(), 1);
                }
                else
                {
                    datePurchaseCount[auction.End.ToShortDateString()]++;
                }
            }
            foreach (var item in datePurchaseCount)
            {
                var splitDate = item.Key.Split('/');
                string day = splitDate[1];
                string month = (int.Parse(splitDate[0]) - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = splitDate[2];

                rows += $"[ new Date({year}, {month}, {day}), {item.Value} ],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(calendarChartPath); // read "template" text

            // add actual data
            int startIndex = html.IndexOf("dataTable.addRows([") + "dataTable.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }

        private string GetMoneySpentLineChartHTML()
        {
            string rows = "";
            float amount = 0;
            itemsBought = itemsBought.OrderBy(x => x.End).ToList();
            //set up dictionary
            for (int i = 0; i < itemsBought.Count; i++)
            {
                amount += itemsBought[i].HighestBid;
                if (i > 0 && itemsBought[i - 1].End.Date == itemsBought[i].End.Date) continue;
                // get date in correct format
                string day = itemsBought[i].End.Day.ToString();
                string month = (itemsBought[i].End.Month - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = itemsBought[i].End.Year.ToString();
                rows += $"[ new Date({year}, {month}, {day}), {amount} ],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(lineChartPath); // read "template" text
            html = html.Replace("BMI", "Money");

            // add actual data
            int startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }

        private string GetWorkoutsThroughoutDay()
        {
            Dictionary<DateTime, int> timesOfDay = new Dictionary<DateTime, int>(); // dictionary formed by <timeOfDay, numberOfWorkouts> 
            
            // each time of day has a 15 minutes difference
            foreach (Auction auction in itemsBought)
            {
                // get time rounded to nearer 15 minutes
                DateTime roundedTimeOfDay = Helper.RoundToNearest(auction.End, TimeSpan.FromMinutes(15));

                if (!timesOfDay.ContainsKey(roundedTimeOfDay))
                {
                    timesOfDay.Add(roundedTimeOfDay, 1);
                }
                else
                {
                    timesOfDay[roundedTimeOfDay]++;
                }
            }

            string rows = "";
            // template for row: [{v: [17, 30], f: '5:30 PM'}, 2],
            foreach (var item in timesOfDay)
            {
                rows += $"[{{v: [{item.Key.Hour}, {item.Key.Minute}], f: '{item.Key.ToShortTimeString()}'}}, {item.Value}],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(timeOfDayColumnsChartPath); // read "template" text

            // add actual data
            int startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // debugging
            File.WriteAllText(@"‪yes.html", html);

            // return html with data
            return html;
        }
        private void DisplayChart()
        {
            if (browser.IsBrowserInitialized)
            {
                switch (tbc.SelectedIndex)
                {
                    case 0:
                        LoadHtml(GetCalendarChartHTML());
                        break;
                    case 1:
                        LoadHtml(GetMoneySpentLineChartHTML());
                        break;
                    case 2:
                        LoadHtml(GetWorkoutsThroughoutDay());
                        break;
                }
            }
        }

        private void tbc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayChart();
        }
    }
}
