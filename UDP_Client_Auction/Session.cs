using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UDP_Client_Auction
{
    public static class Session
    {
        public static User LoggedUser { get; set; }
        public static UDPClient UDPClient { get; set; }
        public static ContentControl contentArea { get; set; }

    }
}
