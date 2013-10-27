using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.Office.Interop.Word;
using UtilityLibrary;

namespace Bidding
{
    /// <summary>
    /// Auction class to store information of the auction.
    /// </summary>
    public class Auction
    {
        public string lot;
        public string artist;
        public string artwork;
        public int initialPrice;
        public int nowPrice;
        public Bitmap photo;
        public string photofilePath;
        public int winBidderNo;
        public string name;
        public int hammerPrice;
        public int serviceCharge;
        public int total;
        public string docName = "";
        public _Document paymentDoc;
        public bool isUseCreditCard;

        public Auction()
        {
            hammerPrice = serviceCharge = total = 0;
            lot = name = "";
            paymentDoc = null;
            isUseCreditCard = false;
        }

        /// <summary>
        /// Get auction's info from its file name.
        /// </summary>
        /// <param name="fileName">auction's file name</param>
        /// <param name="auction">auction object</param>
        public bool GetInfoFromFileName(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string[] infos = fileName.Split('_');
            if (infos.Length != 4)
                return false;
            this.lot = infos[0];
            this.artist = infos[1];
            this.artwork = infos[2];
            int price = 0;
            if (int.TryParse(infos[3], out price))
                this.initialPrice = this.nowPrice = price * Settings.unit;
            else
                return false;
            return true;
        }

        public string CreateFileName(string extension)
        {
            if (extension != "")
                return lot + "_" + artist + "_" + artwork + "_" + (initialPrice / Settings.unit).ToString() + extension;
            else
                return lot + "_" + artist + "_" + artwork + "_" + (initialPrice / Settings.unit).ToString();
        }

        /// <summary>
        /// 0 ~ 10,000,000 = 18%
        /// 10,000,000 ~ 20,000,000 = 15%
        /// 20,000,000 ~ = 12%
        /// </summary>
        public void ComputeChargeAndTotal()
        {
            if (hammerPrice == 0)
                return;

            if (hammerPrice > 20000000)
            {
                serviceCharge = Convert.ToInt32((hammerPrice - 20000000) * 0.12f + 10000000 * 0.15f + 10000000 * 0.18f);
                total = hammerPrice + serviceCharge;
            }
            else if (hammerPrice > 10000000)
            {
                serviceCharge = Convert.ToInt32((hammerPrice - 10000000) * 0.15f + 10000000 * 0.18f);
                total = hammerPrice + serviceCharge;
            }
            else
            {
                serviceCharge = Convert.ToInt32(hammerPrice * 0.18f);
                total = hammerPrice + serviceCharge;
            }
        }
    }

    /// <summary>
    /// Price level structure.
    /// </summary>
    public struct PriceLevel
    {
        public int down;
        public int up;
        public System.Collections.Generic.List<int> increments;
    }

    public class Bidder
    {
        public string name;
        public int no;
        public string phone;
        public string fax;
        public string email;
        public string addr;
        public bool isUseCredit;
        public int creditCardFee;
        public int tax;
        public int amountDue;
        public Dictionary<string, Auction> auctions;
    }

    public enum ReturnState
    {
        待歸還 = 0,
        已歸還,
        拍出待取,
        拍出未取
    };

    public enum PayGuarantee
    {
        台幣現鈔 = 0,
        人民幣現鈔,
        美金現鈔,
        信用卡,
        銀聯卡,
        VIP
    };

    public enum ReturnGuarantee
    {
        台幣現鈔 = 0,
        人民幣現鈔,
        美金現鈔,
        信用卡,
        銀聯卡,
        轉貨款,
        VIP
    };

    public enum PayWay
    {
        台幣現鈔 = 0,
        人民幣現鈔,
        美金現鈔,
        信用卡,
        銀聯卡,
        轉帳北京人民幣,
        轉帳北京美金,
        保證金抵付
    };
}
