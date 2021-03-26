using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace UDP_Client_Auction
{
    public class AuctionTimer
    {
        List<Auction> auctions;
        public AuctionTimer()
        {
            auctions = DB.LoadAuctions();
            SetTimers();
        }

        private void SetTimers()
        {
            foreach (Auction auction in auctions)
            {
                ScheduleAction(auction);
            }
        }

        public async void ScheduleAction(Auction auction)
        {
            if(auction.isExpired) // that means the auction has already ended
            {
                DisableAuction(auction.Id);
            }
            else
            {
                //await Task.Delay((int)auction.End.Subtract(DateTime.Now).TotalMilliseconds);
                await Task.Delay(auction.End - DateTime.Now);
                DisableAuction(auction.Id);
            }
        }

        static public void DisableAuction(int auctionId)
        {
            Auction auction = DB.LoadAuctions().Find(x => x.Id == auctionId);
            if (Session.contentArea.Content is FullAuctionControl)
            {
                FullAuctionControl control = (FullAuctionControl)Session.contentArea.Content;
                control.dateTimeTb.Text = "AUCTION EXPIRED!";
                control.bidItemBtn.Visibility = System.Windows.Visibility.Collapsed;
                control.bidEndedSp.Visibility = System.Windows.Visibility.Visible;

                //control.usernameTb.Text = auction.CurrentUser.Username;
                //control.bidAmountTb.Text = auction.HighestBid.ToString();
                //control.endDateTimeTb.Text = auction.End.ToString();

                control.endAuctionTb.Inlines.Clear();
                control.endAuctionTb.Inlines.Add(new Run("The auction ended at: "));
                control.endAuctionTb.Inlines.Add(new Run(auction.End.ToString()) { FontStyle = FontStyles.Italic, FontWeight = FontWeights.SemiBold });

                if(auction.CurrentUser != null)
                {
                    control.endAuctionTb.Inlines.Add(new Run("\nIt was won by the user: "));
                    control.endAuctionTb.Inlines.Add(new Run(auction.CurrentUser.Username.ToString()) { FontStyle = FontStyles.Italic, FontWeight = FontWeights.SemiBold });
                    control.endAuctionTb.Inlines.Add(new Run(" with a bid of "));
                    control.endAuctionTb.Inlines.Add(new Run(auction.HighestBid.ToString()) { FontStyle = FontStyles.Italic, FontWeight = FontWeights.SemiBold });
                    control.endAuctionTb.Inlines.Add(new Run("€"));
                    BitmapImage auctionImage = new BitmapImage(new Uri(auction.CurrentUser.ProPicPath, UriKind.RelativeOrAbsolute));
                    control.winnerImg.Source = auctionImage;
                    control.winnerImagePopUp.Source = auction.CurrentUser.ProPicPath;
                }
                else
                {
                    control.endAuctionTb.Inlines.Add(new Run("\nThe auction ended without anyone winning it."));
                }
            }
            else if (Session.contentArea.Content is AuctionsControl)
            {
                AuctionsControl control = (AuctionsControl)Session.contentArea.Content;
                foreach (var card in control.cardsWp.Children)
                {
                    AuctionControl cardControl = (AuctionControl)card;
                    if (cardControl.AuctionId == auction.Id)
                    {
                        cardControl.EndDateTime = "AUCTION EXPIRED!";
                    }
                }

            }
        }
    }
}
