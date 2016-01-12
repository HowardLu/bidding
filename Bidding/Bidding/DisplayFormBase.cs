using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiddingLibrary;
using UtilityLibrary;

namespace Bidding
{
    public class DisplayFormBase : Form
    {
        public virtual void SetAuctionOnForm(Auction auction) { }
        public virtual void SetNewPrice(int newMainPrice) { }
        public virtual void SetSession(string sessionStr) { }
        public virtual void SetProgress(int currentId, int totalCount) { }
        public virtual void SetRateName(int rateId, string name) { }
        public virtual void ShowExchangeRate(int erId, bool isShow) { }
        public virtual void SetLogo(Auctioneer auctioneer) { }
    }
}
