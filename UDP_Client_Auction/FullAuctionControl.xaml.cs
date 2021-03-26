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
    /// Interaction logic for FullAuctionControl.xaml
    /// </summary>
    public partial class FullAuctionControl : UserControl
    {
        public Auction auction;

        public FullAuctionControl(Auction _auction)
        {
            InitializeComponent();

            auction = _auction;
        }

        private void _FullAuctionControl_Loaded(object sender, RoutedEventArgs e)
        {
            itemNameTb.Text = auction.Name;
            itemDescriptionTb.Text = auction.Description;
            dateTimeTb.Text = auction.End.ToString();
            highestBidTb.Text = auction.HighestBid.ToString() + "€";

            BitmapImage auctionImage = new BitmapImage(new Uri(auction.ImgPath, UriKind.RelativeOrAbsolute));
            itemImg.Source = auctionImage;
            itemImagePopUp.Source = auction.ImgPath;
        }
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new AuctionsControl();
            }
        }

        private void itemImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                itemImagePopUp.Show();
            }
        }

        private void bidItemBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // Do something with the Input
            if (string.IsNullOrWhiteSpace(InputTextBox.Text) || !float.TryParse(InputTextBox.Text, out _))
            {
                MessageBox.Show("Empty field or not an integer.");
            }
            else if (float.Parse(InputTextBox.Text) < 0)
            {
                MessageBox.Show("The bid price cannot be negative");
            }
            else if(float.Parse(InputTextBox.Text) <= auction.HighestBid)
            {
                MessageBox.Show("The bid price cannot be less than the current highest bid. C'mon man, don't be cheap!");
            }
            else
            {
                // send bid to server
                Session.UDPClient.SendBid(InputTextBox.Text, auction.Id);

                InputBox.Visibility = Visibility.Collapsed;
                MessageBox.Show("Perfect! You made a bid of " + InputTextBox.Text + "€.");
                highestBidTb.Text = InputTextBox.Text + "€";
            }

            // Clear InputBox.
            InputTextBox.Text = string.Empty;
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            InputBox.Visibility = Visibility.Collapsed;

            // Clear InputBox.
            InputTextBox.Text = string.Empty;
        }

        private void winnerImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                winnerImagePopUp.Show();
            }
        }
    }
}
