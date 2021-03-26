using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UDP_Client_Auction
{
    public class UDPClient
    {
        UdpClient udpClient;
        Thread listeningThread;
        volatile bool listening;

        public object Dispatcher { get; private set; }

        public UDPClient()
        {
            try
            {
                udpClient = new UdpClient(GetOpenPort());

                //// this is needed to disable a weird exception
                const int SIO_UDP_CONNRESET = -1744830452;
                udpClient.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);

                listeningThread = new Thread(Listener);
                this.listening = true;
                listeningThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private int GetOpenPort()
        {
            int PortStartIndex = 1000;
            int PortEndIndex = 2000;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();

            List<int> usedPorts = udpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 0;

            for (int port = PortStartIndex; port < PortEndIndex; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    unusedPort = port;
                    break;
                }
            }
            return unusedPort;
        }

        private void Listener()
        {
            IPEndPoint IpRemoto = new IPEndPoint(IPAddress.Any, 0); //accetta datagram da chiunque
            while (this.listening)
            {
                Byte[] receiveBytes;
                try
                {
                    receiveBytes = udpClient.Receive(ref IpRemoto);
                }
                catch (SocketException socketEx) // if another thread closes udpclient while he's receiving this gets called
                {
                    break;
                }
                //chiamata bloccante. attesa datagram
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                if (returnData.Contains("NEW_BID"))
                {
                    // UPDATE HIGHEST BID
                    List<string> returnDataList = returnData.Split(' ').ToList();
                    string amount = returnDataList[1];
                    string itemId = returnDataList[2];

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (Session.contentArea.Content is FullAuctionControl)
                        {
                            FullAuctionControl control = (FullAuctionControl)Session.contentArea.Content;
                            control.auction.HighestBid = float.Parse(amount);
                            control.highestBidTb.Text = amount + "€";
                        }
                        else if (Session.contentArea.Content is AuctionsControl)
                        {
                            AuctionsControl control = (AuctionsControl)Session.contentArea.Content;
                            foreach (var card in control.cardsWp.Children)
                            {
                                AuctionControl cardControl = (AuctionControl)card;
                                if (cardControl.AuctionId.ToString() == itemId)
                                {
                                    cardControl.HighestBid = "Highest bid: " + amount + "€";
                                }
                            }

                        }
                    });
                }
            }
        }

        public void StopListener()
        {
            this.listening = false;
        }

        private void SendMessage(string message)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            udpClient.Send(sendBytes, sendBytes.Length, "localhost", 11125);
        }

        internal void ConnectToServer()
        {
            SendMessage("HELO");
        }

        internal void DisconnectFromServer()
        {
            SendMessage("BYE");
            CloseClient();
        }

        internal void SendBid(string amount, int itemId)
        {
            string message = $"{itemId} {amount} {Session.LoggedUser.Id} {Session.LoggedUser.Password}";
            SendMessage(message);
        }

        private void CloseClient()
        {
            this.listening = false;
            udpClient.Close();
        }
    }
}