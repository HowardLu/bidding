using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using InternetLibrary;
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
        //public string name;
        public int hammerPrice;
        public int serviceCharge;
        public int total;
        public string docName = "";
        public _Document paymentDoc;
        public bool isUseCreditCard;
        public string auctioneer;

        public Auction()
        {
            hammerPrice = serviceCharge = total = 0;
            lot = "";
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

        public bool GetInfoFromDictionary(ref Dictionary<string, AuctionEntity> auctions, string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (!auctions.ContainsKey(fileName))
                return false;
            AuctionEntity ae = auctions[fileName];
            this.lot = ae.AuctionId;
            this.artist = ae.Artist;
            this.artwork = ae.Artwork;
            this.initialPrice = ae.InitialPrice;
            this.nowPrice = ae.NowPrice;
            this.auctioneer = ae.Auctioneer;
            int bidderNo = Utility.ParseToInt(ae.BidderNumber, true);
            this.winBidderNo = bidderNo < 0 ? 0 : bidderNo;
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

        public AuctionEntity ToAuctionEntity()
        {
            AuctionEntity ae = new AuctionEntity();
            ae.AuctionId = this.lot;
            ae.Artist = this.artist;
            ae.Artwork = this.artwork;
            ae.InitialPrice = this.initialPrice;
            ae.NowPrice = this.nowPrice;
            ae.Auctioneer = this.auctioneer;
            return ae;
        }

        public static void LoadAuctions(ref List<Auction> auctions, ref Internet<AuctionEntity> aeInternet, bool isSilence)
        {
            Dictionary<string, AuctionEntity> aeDic = aeInternet.GetCollectionList().ToDictionary<AuctionEntity, string>(ae => ae.AuctionId);
            auctions = new List<Auction>();
            List<string> illegalFiles = new List<string>();
            string[] filePaths = Directory.GetFiles(Settings.auctionFolder).OrderBy(f => f).ToArray<string>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                Auction auction = new Auction();
                string fp = filePaths[i];
                if (auction.GetInfoFromDictionary(ref aeDic, fp))
                {
                    auction.photofilePath = fp;
                    if (auctions.Count < 10)
                        auction.photo = Utility.OpenBitmap(fp);
                    else
                        auction.photo = null;
                    auctions.Add(auction);
                }
                else
                {
                    illegalFiles.Add(fp);
                }
            }

            if (illegalFiles.Count != 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string s in illegalFiles)
                    sb.AppendLine(s);

                if (!isSilence)
                    System.Windows.Forms.MessageBox.Show("不合法的拍品，請設定:\n" + sb.ToString());
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

    public class PaymentDoc
    {
        public string name;
        public _Document doc;
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
        public Auctioneer auctioneer;
        public Dictionary<string, Auction> auctions;
        public Dictionary<string, List<string>> auctionMappings;
        public Dictionary<string, PaymentDoc> paymentDocs;
        public string cashFlowDocName;
        public _Document cashFlowDoc;
        public PayGuarantee payGuaranteeState;
        public int payGuaranteeNum;

        public void SetBidder(BidderEntity bidder, ref List<AuctionEntity> auctions)
        {
            this.name = bidder.Name;   //ignore first string of id.
            this.no = bidder.BidderID_int;
            this.phone = bidder.Tel;
            this.fax = bidder.Fax;
            this.email = bidder.EMail;
            this.addr = bidder.EMail;
            this.auctioneer = Utility.ToEnum<Auctioneer>(bidder.Auctioneer);
            this.auctions = new Dictionary<string, Auction>();
            this.payGuaranteeState = Utility.ToEnum<PayGuarantee>(bidder.GuaranteeType);
            this.payGuaranteeNum = Utility.ParseToInt(bidder.GuaranteeCost, true);
            for (int i = 0; i < auctions.Count; i++)
            {
                AuctionEntity ae = auctions[i];
                Auction auction = new Auction();
                auction.lot = ae.AuctionId;
                auction.artwork = ae.Artwork;
                auction.hammerPrice = ae.HammerPrice;
                auction.ComputeChargeAndTotal();
                auction.auctioneer = ae.Auctioneer;
                this.auctions[auction.lot] = auction;
            }
            MappingAuctions();

            this.paymentDocs = new Dictionary<string, PaymentDoc>();
            for (int i = 0; i < (int)Auctioneer.Count; i++)
            {
                string auctioneer = Utility.GetEnumString(typeof(Auctioneer), i);
                this.paymentDocs[auctioneer] = new PaymentDoc();
            }
        }

        private void MappingAuctions()
        {
            if (auctionMappings == null)
            {
                auctionMappings = new Dictionary<string, List<string>>();
            }

            foreach (Auction auc in auctions.Values)
            {
                string auctioneer = auc.auctioneer == "" ? Auctioneer.S.ToString() : auc.auctioneer;
                if (!auctionMappings.ContainsKey(auctioneer))
                {
                    auctionMappings[auctioneer] = new List<string>();
                }
                auctionMappings[auctioneer].Add(auc.lot);
            }
        }

        public List<Auction> GetAuctions(string auctioneer)
        {
            if (!auctionMappings.ContainsKey(auctioneer))
                return null;

            List<Auction> auctionsOfActioneer = new List<Auction>();
            foreach (string lot in auctionMappings[auctioneer])
            {
                if (auctions.ContainsKey(lot))
                {
                    auctionsOfActioneer.Add(auctions[lot]);
                }
            }
            return auctionsOfActioneer;
        }
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

    public enum Auctioneer
    {
        S = 0,
        A,
        M,
        Count
    };

    public enum AuctioneerName
    {
        世家 = 0,
        安德昇,
        沐春堂,
        Count
    }
}
