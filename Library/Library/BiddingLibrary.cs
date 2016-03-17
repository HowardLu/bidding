//#define MCT
//#define SJ

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InternetLibrary;
using Microsoft.Office.Interop.Word;
using UtilityLibrary;

namespace BiddingLibrary
{
    /// <summary>
    /// Auction class to store information of the auction.
    /// </summary>
    public class Auction
    {
        public static float ServiceChargeRate = 0.2f;
        public static float CreditCardRate = 0.08f;
        public static BiddingCompany DefaultBiddingCompany = BiddingCompany.S;

        public string lot;
        public string artist;
        public string artwork;
        public int initialPrice;
        public int nowPrice;
        public Bitmap photo;
        public string photoFilePath;
        public int winBidderNo;
        //public string name;
        public int hammerPrice;
        public int serviceCharge;
        public int total;
        public string docName = "";
        public _Document paymentDoc;
        public bool isUseCreditCard;
        public string auctioneer;
        public int checkoutNumber;
        public string checkoutTime;
        public string videoPath;

        public Auction()
        {
            hammerPrice = serviceCharge = total = 0;
            lot = "";
            paymentDoc = null;
            isUseCreditCard = false;
            checkoutNumber = 0;
            checkoutTime = "";
            videoPath = "";
            auctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)DefaultBiddingCompany);
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
            if (this.nowPrice < this.initialPrice)
                this.nowPrice = this.initialPrice;
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
        /// 20131219 all 20%
        /// </summary>
        public void ComputeChargeAndTotal()
        {
            if (this.hammerPrice == 0)
            {
                this.serviceCharge = this.total = 0;
                return;
            }

            /* befor 20131219, old
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
            }*/

            serviceCharge = Convert.ToInt32(hammerPrice * ServiceChargeRate);
            total = hammerPrice + serviceCharge;
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

        public static List<string> LoadSessions()
        {
            List<string> sessions = new List<string>();
            string[] dirs = Directory.GetDirectories(Settings.auctionFolder);
            for (int i = 0; i < dirs.Length; i++)
            {
                string dirName = Path.GetFileNameWithoutExtension(dirs[i]);
                if (Utility.ParseToInt(dirName, true) != -1)
                    sessions.Add(dirName);
            }
            return sessions;
        }

        public static void LoadAuctions(string sessionId, ref List<Auction> auctions, ref Internet<AuctionEntity> aeInternet, bool isSilence)
        {
            Dictionary<string, AuctionEntity> aeDic = ToDictionary(ref aeInternet);
            auctions = new List<Auction>();
            List<string> illegalFiles = new List<string>();
            string dir = Path.Combine(Settings.auctionFolder, sessionId);
            string[] filePaths = Directory.GetFiles(dir).OrderBy(f => f).ToArray<string>();
            Dictionary<string, string> videoPaths = GetVideosFilePaths();
            for (int i = 0; i < filePaths.Length; i++)
            {
                string fp = filePaths[i];
                if (fp.Contains("Thumbs"))
                    continue;

                Auction auction = new Auction();
                if (auction.GetInfoFromDictionary(ref aeDic, fp))
                {
                    auction.photoFilePath = fp;
                    if (auctions.Count < 10)
                        auction.photo = Utility.OpenBitmap(fp);
                    else
                        auction.photo = null;
                    if (videoPaths.ContainsKey(auction.lot))
                        auction.videoPath = videoPaths[auction.lot];
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
                    System.Windows.Forms.MessageBox.Show("不合法的拍品，請設定:\n\n" + sb.ToString());
            }
        }

        public void ResetPrice()
        {
            this.hammerPrice = this.nowPrice = this.initialPrice;
        }

        private static Dictionary<string, string> GetVideosFilePaths()
        {
            Dictionary<string, string> videoPaths = new Dictionary<string, string>();
            if (Directory.Exists(Settings.videoFolder))
            {
                string[] filePaths = Directory.GetFiles(Settings.videoFolder).OrderBy(f => f).ToArray<string>();
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string fp = filePaths[i];
                    string fn = Utility.GetFileName(fp, false);
                    videoPaths.Add(fn, fp);
                }
            }
            return videoPaths;
        }

        private static Dictionary<string, AuctionEntity> ToDictionary(ref Internet<AuctionEntity> aeInternet)
        {
            Dictionary<string, AuctionEntity> aucs = new Dictionary<string, AuctionEntity>();
            List<AuctionEntity> auctionEnts = aeInternet.GetCollectionList();
            foreach (AuctionEntity auc in auctionEnts)
            {
                if (!aucs.ContainsKey(auc.AuctionId))
                {
                    aucs.Add(auc.AuctionId, auc);
                }
                else
                {
                    if (DialogResult.OK == MessageBox.Show(String.Format("重複LOT: {0} 是否刪除舊的?", auc.AuctionId),
                        "Warning", MessageBoxButtons.OKCancel))
                    {
                        aeInternet.RemoveOne<string>(ae => ae.AuctionId, auc.AuctionId);
                        aucs[auc.AuctionId] = auc;
                    }
                    else
                    {
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            return aucs;
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

        public PriceLevel(int down, int up)
        {
            this.down = down;
            this.up = up;
            this.increments = new List<int>();
        }
    }

    public static class PriceLevels
    {
        public static System.Collections.Generic.List<PriceLevel> levels;

        private static string filePath;

        /// <summary>
        /// Load price level.
        /// </summary>
        /// <param name="fn">price level file name</param>
        public static void Load(string fn)
        {
            if (!Utility.IsFileExist(fn, false))
                return;

            filePath = fn;
            levels = new List<PriceLevel>();
            using (StreamReader sr = new StreamReader(fn))
            {
                int count = int.Parse(sr.ReadLine().Remove(0, "Down Up Increment---".Length));
                for (int i = 0; i < count; i++)
                {
                    string[] data = sr.ReadLine().Split(' ');
                    int down = int.Parse(data[0]) * Settings.unit;
                    int up = int.Parse(data[1]) * Settings.unit;
                    PriceLevel pl = new PriceLevel(down, up);
                    string[] s = data[2].Split(',');
                    //pl.increments = new List<int>();
                    for (int j = 0; j < s.Length; j++)
                    {
                        float level = 0.0f;
                        if (float.TryParse(s[j], out level))
                        {
                            pl.increments.Add(Convert.ToInt32(level * Settings.unit));
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(fn + "格式錯誤");
                            return;
                        }
                    }
                    levels.Add(pl);
                }
            }
        }

        public static void Save()
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                int count = levels.Count;
                sw.WriteLine("Down Up Increment---{0}", count);
                for (int i = 0; i < count; i++)
                {
                    PriceLevel pl = levels[i];
                    int up = pl.up / Settings.unit;
                    int down = pl.down / Settings.unit;
                    string levelStr = "";
                    if (pl.increments.Count == 1)
                    {
                        levelStr = (pl.increments[0] / Settings.unit).ToString();
                    }
                    else
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        for (int j = 0; j < pl.increments.Count; j++)
                        {
                            sb.Append(pl.increments[j] / Settings.unit);
                            if (j != pl.increments.Count - 1)
                                sb.Append(',');
                        }
                        levelStr = sb.ToString();
                    }
                    sw.WriteLine("{0} {1} {2}", down, up, levelStr);
                }
            }
        }
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
        public int serviceFee = 0;
        public BiddingCompany auctioneer;
        public Dictionary<string, Auction> auctions;
        public Dictionary<string, List<string>> auctionMappings;
        public Dictionary<string, PaymentDoc> paymentDocs;
        public string cashFlowDocName;
        public _Document cashFlowDoc;
        public PayGuarantee payGuaranteeState;
        public int payGuaranteeNum;

        public void SetBidder(BidderEntity bidder, ref List<AuctionEntity> auctionEntities)
        {
            this.name = bidder.Name;   //ignore first string of id.
            this.no = bidder.BidderID_int;
            this.phone = bidder.Tel;
            this.fax = bidder.Fax;
            this.email = bidder.EMail;
            this.addr = bidder.Address;
            this.auctioneer = Utility.ToEnum<BiddingCompany>(bidder.Auctioneer);
            this.auctions = new Dictionary<string, Auction>();
            this.payGuaranteeState = Utility.ToEnum<PayGuarantee>(bidder.GuaranteeType);
            int guaranteeNum = Utility.ParseToInt(bidder.GuaranteeCost, true);
            this.payGuaranteeNum = guaranteeNum < 0 ? 0 : guaranteeNum;
            int.TryParse(bidder.ServiceFee, out this.serviceFee);
            for (int i = 0; i < auctionEntities.Count; i++)
            {
                AuctionEntity ae = auctionEntities[i];
                Auction auction = new Auction();
                auction.lot = ae.AuctionId;
                auction.artwork = ae.Artwork;
                auction.hammerPrice = ae.HammerPrice;
                auction.ComputeChargeAndTotal();
                auction.auctioneer = ae.Auctioneer;
                auction.checkoutNumber = ae.CheckoutNumber;
                auction.checkoutTime = ae.CheckoutTime;
                this.auctions[auction.lot] = auction;
            }
            MappingAuctions();

            this.paymentDocs = new Dictionary<string, PaymentDoc>();
            for (int i = 0; i < (int)BiddingCompany.Count; i++)
            {
                string auctioneer = Utility.GetEnumString(typeof(BiddingCompany), i);
                this.paymentDocs[auctioneer] = new PaymentDoc();
            }
        }

        private void MappingAuctions()
        {
            if (auctionMappings == null)
            {
                auctionMappings = new Dictionary<string, List<string>>();
            }

            string defaultAuctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
            foreach (Auction auc in auctions.Values)
            {
                string auctioneer = auc.auctioneer == "" ? defaultAuctioneer : auc.auctioneer;
                if (!auctionMappings.ContainsKey(auctioneer))
                {
                    auctionMappings[auctioneer] = new List<string>();
                }
                auctionMappings[auctioneer].Add(auc.lot);
            }
            if (BiddingCompany.N == Auction.DefaultBiddingCompany)
            {
                if (auctionMappings.ContainsKey("N") && 0 == auctionMappings["N"].Count &&
                    auctionMappings.ContainsKey("S"))
                    auctionMappings["N"] = auctionMappings["S"];
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
        拍出未取,
        拍出已取
    };

    public enum PayGuarantee
    {
        台幣現鈔 = 0,
        人民幣現鈔,
        美金現鈔,
        信用卡,
        銀聯卡,
        VIP,
        日幣現鈔
    };

    public enum ReturnGuarantee
    {
        台幣現鈔 = 0,
        人民幣現鈔,
        美金現鈔,
        信用卡,
        銀聯卡,
        轉貨款,
        VIP,
        日幣現鈔
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
        保證金抵付,
        日幣現鈔
    };

    public enum BiddingCompany
    {
        M = 0,
        S,
        N,
        SFJ,
        DS,
        G,
        Count
    };

    public enum BiddingCompanyName
    {
        沐春堂 = 0,
        世家,
        新象,
        禪機,
        大行,
        吉祥門,
        Count
    }

    public class DealerCheckoutItem
    {
        public const int InfoSize = 9;

        //圖錄號 DealerItemEntity:dealerItem.LotNO
        //作者 AuctionEntity:Artist
        //作品名稱 AuctionEntity:Artwork
        //落槌價 AuctionEntity:HammerPrice
        //圖錄費 SellerEntity:IfDealedPictureFee
        //佣金 SellerEntity:IfDealedServiceFee % * 落槌價
        //保險費 SellerEntity:IfDealedInsuranceFee % * 落槌價
        //其他 SellerEntity:FrameFee + SellerEntity:FireFee + SellerEntity:IdentifyFee
        //合計 落槌價 + 圖錄費 + 佣金 + 保險費 + 其他

        public string InfoLotNO;
        public string InfoArtist;
        public string InfoArtwork;
        public int InfoHammerPrice;
        public int InfoPictureFee;
        public int InfoServiceFee;
        public int InfoInsuranceFee;
        public int InfoOtherFee;
        public int InfoTotalPrice;

        public void CalcTotalPrice()
        {
            InfoTotalPrice = InfoHammerPrice + InfoPictureFee + InfoServiceFee + InfoInsuranceFee + InfoOtherFee;
        }

        public string[] GenListViewInfo()
        {
            return new string[InfoSize] 
            { 
                InfoLotNO,
                InfoArtist,
                InfoArtwork,
                InfoHammerPrice.ToString(),
                InfoPictureFee.ToString(),
                InfoServiceFee.ToString(),
                InfoInsuranceFee.ToString(),
                InfoOtherFee.ToString(),
                InfoTotalPrice.ToString(),
            };
        }
    }

    public class DealerTakeBackItem
    {
        public const int InfoSize = 4;

        public string InfoContractId;
        public string InfoLotNO;
        public string InfoArtist;
        public string InfoArtwork;

        public string[] GenListViewInfo()
        {
            return new string[InfoSize] 
            {
                InfoContractId,
                InfoLotNO,
                InfoArtist,
                InfoArtwork,
            };
        }
    }
}
