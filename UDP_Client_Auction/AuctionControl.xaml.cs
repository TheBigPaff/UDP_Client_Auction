using System;
using System.Collections.Generic;
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

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for AuctionControl.xaml
    /// </summary>
    public partial class AuctionControl : UserControl
    {
        public int AuctionId;

        public AuctionControl()
        {
            InitializeComponent();
        }
        public Uri ImageSource
        {
            get { return (Uri)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ImageSource. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(Uri), typeof(AuctionControl));

        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ItemName. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemNameProperty = DependencyProperty.Register("ItemName", typeof(string), typeof(AuctionControl));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(AuctionControl));



        public string EndDateTime
        {
            get { return (string)GetValue(EndDateTimeProperty); }
            set { SetValue(EndDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndDateTimeProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndDateTimeProperty = DependencyProperty.Register("EndDateTime", typeof(string), typeof(AuctionControl));

        public string HighestBid
        {
            get { return (string)GetValue(HighestBidProperty); }
            set { SetValue(HighestBidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighestBid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighestBidProperty = DependencyProperty.Register("HighestBid", typeof(string), typeof(AuctionControl));

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            AuctionsControl parent = Helper.GetAncestorOfType<AuctionsControl>(this);
            if (parent != null)
            {
                parent.AuctionControlClicked(this);
            }
        }
    }
}
