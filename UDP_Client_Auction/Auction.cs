using System;

namespace UDP_Client_Auction
{
    public class Auction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }
        public DateTime End { get;  set; }
        public float HighestBid { get;  set; }
        public User CurrentUser { get; set; }
        public bool isExpired { get
            {
               return this.End <= DateTime.Now;
            } 
        }
        public Auction()
        {
            CurrentUser = new User();
        }
    }
}