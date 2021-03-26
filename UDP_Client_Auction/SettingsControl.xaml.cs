using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        Timer settingSavedCheckIconTimer;

        public SettingsControl()
        {
            InitializeComponent();

            settingSavedCheckIconTimer = new Timer();
            settingSavedCheckIconTimer.Interval = 2000;
            settingSavedCheckIconTimer.Elapsed += SettingSavedCheckIconTimer_Elapsed;

            SetSettings();
        }

        private void SetSettings()
        {
            //startupPageCb.SelectedItem = startupPageCb.FindName(Session.Settings.StartupPage.Replace(" ", ""));
        }

        private void SettingSavedCheckIconTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => {
                settingSavedCheckSp.Visibility = Visibility.Collapsed;
            }));

            settingSavedCheckIconTimer.Stop();
        }

        private void SaveSetting()
        {
            if (settingSavedCheckSp != null)
            {
                settingSavedCheckSp.Visibility = Visibility.Visible;
                settingSavedCheckIconTimer.Start();
            }
        }

        private void startupPageCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// if user changed settings
            //if (Session.Settings.StartupPage != (startupPageCb.SelectedItem as ComboBoxItem).Content.ToString())
            //{
            //    Session.Settings.StartupPage = (startupPageCb.SelectedItem as ComboBoxItem).Content.ToString();
            //    SaveSetting();
            //}
            SaveSetting();
        }
    }
}


