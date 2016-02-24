using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InternetLibrary;
using BiddingLibrary;
using UtilityLibrary;
using System.IO;

namespace TakeBackDealer
{
    public partial class MainForm : Form
    {
        #region Member Variables
        private Internet<AuctionEntity> m_auctionInternet;
        private Internet<DealerItemEntity> m_dealerItemInternet;
        private Internet<DealerEntity> m_dealerInternet;
        private DealerEntity m_dealer;
        private List<DealerTakeBackItem> m_takeBackItems = new List<DealerTakeBackItem>();

        private Microsoft.Office.Interop.Word._Application m_wordApp = null;
        private Microsoft.Office.Interop.Word._Document m_wordDoc = null;
        private string m_templateDoc = "DealerTakeBack.dot";
        private Object m_oMissing = System.Reflection.Missing.Value;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string ip = Utility.InputIp();
            if (ip.Length == 0)
            {
                MessageBox.Show("IP不可為空!!!");
                return;
            }

            m_auctionInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_dealerItemInternet = new Internet<DealerItemEntity>(ip, "bidding_data", "dealer_item_table");
            m_dealerInternet = new Internet<DealerEntity>(ip, "bidding_data", "dealer_table");

            if (m_auctionInternet.Connect() &&
                m_dealerItemInternet.Connect() &&
                m_dealerInternet.Connect()
                )
            {
                buttonQuery.Enabled = true;
                textBoxDealerName.Enabled = true;
                toolStripStatusLabelMain.Text = "連線成功!請開始查詢。";
            }
            else
            {
                toolStripStatusLabelMain.Text = "連線失敗!";
            }
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            string dealerName = textBoxDealerName.Text;

            //取得賣家資訊
            List<DealerEntity> dealerList = m_dealerInternet.Find<string>(ae => ae.Name, dealerName);
            if (dealerList.Count != 1)
            {
                MessageBox.Show("查無此賣家");
                return;
            }

            m_dealer = dealerList[0];
            int pictureFee, insuranceFeeP, serviceFeeP, otherFee;

            m_dealer.parseDealedFee(out pictureFee, out insuranceFeeP, out serviceFeeP);
            m_dealer.parseOtherFee(out otherFee);

            //先拿到賣家旗下所有的商品牌號
            List<DealerItemEntity> dealerItemList = m_dealerItemInternet.Find<string>(ae => ae.SrcDealer, dealerName);

            //再用這些牌號去取得相對應的資訊
            m_takeBackItems.Clear();
            foreach (DealerItemEntity dealerItem in dealerItemList)
            {
                //Console.WriteLine("[CheckoutDealerForm.buttonQuery_Click] lotNO = " + dealerItem.LotNO);

                int lotNO;
                if (!int.TryParse(dealerItem.LotNO, out lotNO))
                {
                    Console.WriteLine("[CheckoutDealerForm.buttonQuery_Click] invalid dealerItem.LotNO = " + dealerItem.LotNO);
                    continue;
                }

                List<AuctionEntity> itemList = m_auctionInternet.Find<string>(ae => ae.AuctionId, lotNO.ToString());
                if (itemList.Count != 1)
                {
                    Console.WriteLine("[CheckoutDealerForm.buttonQuery_Click] invalid itemList.Count = " + itemList.Count);
                    continue;
                }

                AuctionEntity auctionItem = itemList[0];
                if (auctionItem.HammerPrice != 0)
                {
                    continue;
                }

                DealerTakeBackItem takeBackItem = new DealerTakeBackItem();
                takeBackItem.InfoContractId = m_dealer.ContractID;
                takeBackItem.InfoLotNO = lotNO.ToString();
                takeBackItem.InfoArtist = auctionItem.Artist;
                takeBackItem.InfoArtwork = auctionItem.Artwork;

                m_takeBackItems.Add(takeBackItem);
            }

            listViewResult.BeginUpdate();
            listViewResult.Items.Clear();
            foreach (DealerTakeBackItem takeBackItem in m_takeBackItems)
            {
                ListViewItem listViewItem = new ListViewItem(takeBackItem.GenListViewInfo());

                listViewResult.Items.Add(listViewItem);
            }
            listViewResult.EndUpdate();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _saveDoc(false);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            _saveDoc();
        }

        private void _saveDoc(bool toPrint = true)
        {
            if (m_dealer == null)
            {
                MessageBox.Show(string.Format("請先查詢資料"));
                return;
            }

            if (m_takeBackItems.Count == 0)
            {
                MessageBox.Show(string.Format("無資料可列印"));
                return;
            }

            m_wordApp = new Microsoft.Office.Interop.Word.Application();
            string auctioneer = Utility.GetEnumString(typeof(Auctioneer), (int)Auction.DefaultAuctioneer);

            Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" +
                Settings.docTempFolder + @"\" + m_templateDoc;

            string docName = string.Format("賣家退貨-{0}.doc", m_dealer.Name);

            m_wordDoc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
            Microsoft.Office.Interop.Word.Table wordTable = m_wordDoc.Tables[1];

            int rowPtr = 2;
            wordTable.Cell(rowPtr++, 2).Range.Text = m_dealer.ContractID;
            wordTable.Cell(rowPtr++, 2).Range.Text = m_dealer.Name;
            wordTable.Cell(rowPtr++, 2).Range.Text = m_dealer.Tel;
            wordTable.Cell(rowPtr++, 2).Range.Text = m_dealer.CellPhone;
            wordTable.Cell(rowPtr++, 2).Range.Text = m_dealer.Address;

            rowPtr = 10;
            for (int i = 1; i < m_takeBackItems.Count; i++)
            {
                wordTable.Rows.Add(wordTable.Rows[rowPtr]);
            }

            int counter = 0;
            DateTime now = DateTime.Now;
            string todayStr = string.Format("{0}/{1:00}/{2:00}", now.Year, now.Month, now.Day);

            foreach (DealerTakeBackItem item in m_takeBackItems)
            {
                wordTable.Cell(rowPtr, 1).Range.Text = (++counter).ToString();
                wordTable.Cell(rowPtr, 2).Range.Text = item.InfoLotNO;
                wordTable.Cell(rowPtr, 3).Range.Text = item.InfoArtist;
                wordTable.Cell(rowPtr, 4).Range.Text = item.InfoArtwork;
                wordTable.Cell(rowPtr, 5).Range.Text = todayStr;

                rowPtr++;
            }

            wordTable.Cell(rowPtr, 1).Range.Text = string.Format("退貨合計{0}件", counter);

            //save doc
            string savePath = Path.Combine(System.Windows.Forms.Application.StartupPath, docName);

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            m_wordDoc.SaveAs(savePath);

            //print?
            if (toPrint)
            {
                Utility.PrintDoc(m_wordDoc);
            }

            //close doc
            object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            m_wordDoc.Close(ref saveOption, ref m_oMissing, ref m_oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_wordDoc);
            m_wordDoc = null;

            //close app
            if (m_wordApp != null)
            {
                if (m_wordApp.NormalTemplate != null)
                    m_wordApp.NormalTemplate.Saved = true;
                m_wordApp.Quit(ref m_oMissing, ref m_oMissing, ref m_oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_wordApp);
                m_wordApp = null;
            }

            string printMsg = toPrint ? "並印出" : "";
            toolStripStatusLabelMain.Text = string.Format("檔案已儲存至{0}{1}", savePath, printMsg);
        }

    }
}
