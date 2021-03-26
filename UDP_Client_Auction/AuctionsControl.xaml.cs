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
    /// Interaction logic for AuctionsControl.xaml
    /// </summary>
    public partial class AuctionsControl : UserControl
    {
        List<Auction> auctions;
        public AuctionsControl()
        {
            InitializeComponent();
            auctions = DB.LoadAuctions();
            LoadCardControls();
        }

        private void LoadCardControls()
        {
            if (auctions.Count > 0)
            {
                noAuctionsGrid.Visibility = Visibility.Collapsed;

                foreach (Auction auction in auctions)
                {
                    cardsWp.Children.Add(new AuctionControl
                    {
                        AuctionId = auction.Id,
                        ItemName = auction.Name,
                        HighestBid = "Highest bid: " + auction.HighestBid.ToString() + "€",
                        EndDateTime = "Expires at: " + auction.End.ToString(),
                        ImageSource = new Uri(auction.ImgPath, UriKind.RelativeOrAbsolute)
                    });
                }
            }
            else
            {
                noAuctionsGrid.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                this.Width = contentArea.Width;
                this.Height = contentArea.Height;
            }

            AuctionTimer timer = new AuctionTimer();
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTb.Text)) LoadCardControls();
            if (auctions == null) return;

            cardsWp.Children.Clear();
            foreach (Auction auction in auctions)
            {
                string searchText = SearchTb.Text.ToLower();

                // search algorithm
                if (auction.Name.ToLower().Contains(searchText))
                {
                    cardsWp.Children.Add(new AuctionControl
                    {
                        AuctionId = auction.Id,
                        ItemName = auction.Name,
                        HighestBid = "Highest bid: " + auction.HighestBid.ToString() + "€",
                        EndDateTime = "Expires at:" + auction.End.ToString(),
                        ImageSource = new Uri(auction.ImgPath, UriKind.RelativeOrAbsolute)
                    });
                }
            }
        }

        internal void AuctionControlClicked(AuctionControl auctionControl)
        {
            auctions = DB.LoadAuctions();
            Auction auction = auctions.Find(x => x.Id == auctionControl.AuctionId);

            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new FullAuctionControl(auction);
                if (auction.isExpired)
                {
                    AuctionTimer.DisableAuction(auction);
                }
            }
        }
    }
}
