﻿//#define MCT
//#define SJ
//#define SJ_FOR_JP

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BiddingLibrary;
using InternetLibrary;
using Microsoft.Office.Interop.Word;
using UtilityLibrary;

namespace Checkout
{
    public partial class CheckoutForm : Form
    {
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        #endregion

        #region Member Variables
        private Microsoft.Office.Interop.Word._Application m_wordApp = null;
        private string m_dealTemplateFN = "DealTemp_";
        private Bidder m_bidder;
        private Internet<AuctionEntity> m_auctionInternet;
        private Internet<BidderEntity> m_bidderInternet;
        private Object m_oMissing = System.Reflection.Missing.Value;
        private Size m_listViewSize;
        private int[] m_colWidths;
        private string m_yesStr = "是";
        private string m_noStr = "否";
        private string m_cashFlowTemplateFN = "CashFlowTemp_";
        private int m_maxCheckoutNumber = 0;
        private Dictionary<int, string> m_checkoutTime;
        private float m_taxRate = 0f;
        private ListViewItemComparer m_sorter;
        private const string m_wordExtension = ".doc";
        #endregion

        #region Properties
        #endregion

        #region Constructors and Finalizers
        public CheckoutForm()
        {
            InitializeComponent();
            m_sorter = new ListViewItemComparer(0, SortOrder.Ascending);    // default column to 0 and default order
            this.auctionsListView.ListViewItemSorter = m_sorter;
            m_listViewSize = this.auctionsListView.Size;
            m_colWidths = new int[auctionsListView.Columns.Count];
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                m_colWidths[i] = auctionsListView.Columns[i].Width;
            m_checkoutTime = new Dictionary<int, string>();
        }
        #endregion

        #region Windows Form Events
        private void CheckoutForm_Load(object sender, EventArgs e)
        {
            Settings.Load();
            serviceChargeRateTextBox.Text = Settings.serviceChargeRate.ToString();
            creditCardRateTextBox.Text = Settings.creditCardRate.ToString();
            dealDocPrintCntTextBox.Text = Settings.dealDocPrintCount.ToString();
            cashFlowDocPrintCntTextBox.Text = Settings.cashFlowDocPrintCount.ToString();

            string ip = Utility.InputIp();
            if (ip.Length == 0)
            {
                return;
            }
            m_auctionInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_auctionInternet.Connect();
            m_bidderInternet = new Internet<BidderEntity>(ip, "bidding_data", "buyer_table");
            m_bidderInternet.Connect();
            if (m_auctionInternet.IsConnected && m_bidderInternet.IsConnected)
            {
                SetButtonsEnable(true);
            }

            string defaultAuctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
            m_cashFlowTemplateFN = m_cashFlowTemplateFN + defaultAuctioneer + ".dot";
            if (BiddingCompany.SFJ == Auction.DefaultBiddingCompany)
            {
                m_taxRate = 0.08f;
            }
            if (BiddingCompany.DS != Auction.DefaultBiddingCompany)
            {
                totalCountLabel.Visible = false;
                totalHammerPriceLabel.Visible = false;
                totalServiceChargeLabel.Visible = false;
                totalTotalLabel.Visible = false;
            }
            if (BiddingCompany.G != Auction.DefaultBiddingCompany)
            {
                directDiscountLabel.Visible = directDiscountTextBox.Visible = false;
            }

            auctionsListView.CheckBoxes = true;
        }

        private void CheckoutForm_Resize(object sender, System.EventArgs e)
        {
            if (m_listViewSize.Width == 0 || m_listViewSize.Height == 0)
                return;
            float xRatio = (float)this.auctionsListView.Width / m_listViewSize.Width;
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                auctionsListView.Columns[i].Width = Convert.ToInt32(m_colWidths[i] * xRatio);
        }

        private void CheckoutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseDocAndWord();
            Settings.Save();

            m_wordApp = null;
            m_bidder = null;
            m_auctionInternet = null;
            m_bidderInternet = null;
            if (null != m_checkoutTime)
            {
                m_checkoutTime.Clear();
                m_checkoutTime = null;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (!m_auctionInternet.IsConnected)
            {
                SetButtonsEnable(false);
                MessageBox.Show("連線失敗，請檢查網路!");
                return;
            }

            if (m_bidder != null)
                m_bidder = null;

            int bidderNo = 0;
            if (Int32.TryParse(bidderNoTextBox.Text, out bidderNo))
            {
                List<AuctionEntity> auctionEnts = m_auctionInternet.Find<string>(ae => ae.BidderNumber, bidderNo.ToString());
                GetCheckoutInfo(ref auctionEnts);
                BidderEntity bidder = m_bidderInternet.FineOne<int>(b => b.BidderID_int, bidderNo);
                if (bidder == null)
                {
                    if (auctionEnts.Count == 0)
                    {
                        MessageBox.Show("查詢不到此買家!");
                        return;
                    }
                    else
                    {
                        bidder = new BidderEntity();
                        bidder.BidderID_int = bidderNo;
                        MessageBox.Show("此買家無登記資料!");
                    }
                }
                else
                {
                    if (auctionEnts.Count == 0)
                    {
                        MessageBox.Show("此買家尚未有得標品!");
                        return;
                    }
                }

                m_bidder = new Bidder();
                m_bidder.SetBidder(bidder, ref auctionEnts);
                UpdateListView();

                saveButton.Enabled = true;
                printButton.Enabled = true;
                useCardButton.Enabled = true;
                UpdateTotalLabels();
                toolStripStatusLabel1.Text = "查詢成功";
                isAllUseCreditCardCheckBox.Checked = false;
            }
        }

        private void SetButtonsEnable(bool isConnected)
        {
            if (isConnected)
            {
                bidderNoTextBox.Enabled = true;
                searchButton.Enabled = true;
                connectButton.Enabled = false;
                toolStripStatusLabel1.Text = "連線成功!請開始查詢。";
            }
            else
            {
                connectButton.Enabled = true;
                bidderNoTextBox.Enabled = false;
                searchButton.Enabled = false;
                saveButton.Enabled = false;
                printButton.Enabled = false;
                useCardButton.Enabled = false;
                toolStripStatusLabel1.Text = "連線失敗!請重新連線!";
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveAllDoc();
            CloseDocAndWord();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != MessageBox.Show("按列印將以買家目前得標品結帳，\n確定?", "是否結帳?", MessageBoxButtons.OKCancel))
            {
                return;
            }

            UpdateCheckoutInfo();

            SaveAllDoc();

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    PrintDoc(auc.dealDoc, 1);
                }
            }
            else if (0 != Settings.dealDocPrintCount)
            {
                foreach (KeyValuePair<string, DealDoc> dealDoc in m_bidder.dealDocs)
                {
                    if (null != dealDoc.Value.doc)
                        PrintDoc(dealDoc.Value.doc, Settings.dealDocPrintCount);
                }
            }

            if (0 != Settings.cashFlowDocPrintCount && null != m_bidder.cashFlowDoc)
                PrintDoc(m_bidder.cashFlowDoc, Settings.cashFlowDocPrintCount);

            CloseDocAndWord();
        }

        private void bidderNoTextBox_TextChanged(object sender, System.EventArgs e)
        {
            saveButton.Enabled = false;
            printButton.Enabled = false;
            useCardButton.Enabled = false;

            if (bidderNoTextBox.Text.Length == 0)
                return;

            if (Utility.ParseToInt(bidderNoTextBox.Text, false) == -1)
            {
                bidderNoTextBox.Text = "";
            }

            CloseDocAndWord();
        }

        private void auctionsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != m_sorter.Column)
            {
                // Set the sort column to the new column.
                m_sorter.Column = e.Column;
                // Set the sort order to ascending by default.
                m_sorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (m_sorter.Order == SortOrder.Ascending)
                    m_sorter.Order = SortOrder.Descending;
                else
                    m_sorter.Order = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            auctionsListView.Sort();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            string ip = Utility.InputIp();
            if (ip.Length == 0)
                return;
            m_auctionInternet.IP = ip;
            if (m_auctionInternet.Connect())
            {
                bidderNoTextBox.Enabled = true;
                searchButton.Enabled = true;
                connectButton.Enabled = false;
                toolStripStatusLabel1.Text = "連線成功!請開始查詢。";
            }
            else
            {
                toolStripStatusLabel1.Text = "連線失敗!";
            }
        }

        private void useCardButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in auctionsListView.SelectedItems)
            {
                Auction auc = m_bidder.auctions[lvi.Text];
                if (lvi.SubItems[5].Text == m_noStr)
                {
                    auc.isUseCreditCard = true;
                    auc.serviceCharge = auc.serviceCharge + Convert.ToInt32(Auction.CreditCardRate * (float)auc.hammerPrice);
                    auc.total = auc.hammerPrice + auc.serviceCharge;
                    lvi.SubItems[5].Text = m_yesStr;
                    lvi.BackColor = Color.LightCyan;
                }
                else
                {
                    auc.isUseCreditCard = false;
                    auc.serviceCharge = Convert.ToInt32(Auction.ServiceChargeRate * (float)auc.hammerPrice);
                    auc.total = auc.hammerPrice + auc.serviceCharge;
                    lvi.SubItems[5].Text = m_noStr;
                    lvi.BackColor = System.Drawing.SystemColors.Window;
                }
                lvi.SubItems[3].Text = auc.serviceCharge.ToString("n0");
                lvi.SubItems[4].Text = auc.total.ToString("n0");
            }

            if (Auction.DefaultBiddingCompany == BiddingCompany.DS)
                UpdateTotalLabels();
        }

        private void serviceChargeRateTextBox_TextChanged(object sender, EventArgs e)
        {
            float serviceChargeRate = Utility.ParseToFloat(serviceChargeRateTextBox.Text, false);
            if (-1 == serviceChargeRate)
            {
                serviceChargeRateTextBox.Text = "20";
                return;
            }
            Auction.ServiceChargeRate = serviceChargeRate / 100.0f;
            Settings.serviceChargeRate = serviceChargeRate;

            UpdateAllInListView();

            if (Auction.DefaultBiddingCompany == BiddingCompany.DS)
                UpdateTotalLabels();
        }

        private void creditCardRateTextBox_TextChanged(object sender, EventArgs e)
        {
            float creditCardRate = Utility.ParseToFloat(creditCardRateTextBox.Text, false);
            if (-1 == creditCardRate)
            {
                creditCardRateTextBox.Text = "8";
                return;
            }
            Auction.CreditCardRate = creditCardRate / 100.0f;
            Settings.creditCardRate = creditCardRate;

            UpdateAllInListView();

            if (Auction.DefaultBiddingCompany == BiddingCompany.DS)
                UpdateTotalLabels();
        }

        private void dealDocPrintCntTextBox_TextChanged(object sender, EventArgs e)
        {
            int printCount = Utility.ParseToInt(dealDocPrintCntTextBox.Text, false);
            if (-1 == printCount)
            {
                dealDocPrintCntTextBox.Text = "3";
                return;
            }
            Settings.dealDocPrintCount = printCount;
        }

        private void cashFlowDocPrintCntTextBox_TextChanged(object sender, EventArgs e)
        {
            int printCount = Utility.ParseToInt(cashFlowDocPrintCntTextBox.Text, false);
            if (-1 == printCount)
            {
                cashFlowDocPrintCntTextBox.Text = "1";
                return;
            }
            Settings.cashFlowDocPrintCount = printCount;
        }

        private void isAllUseCreditCardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool isAllUseCard = isAllUseCreditCardCheckBox.Checked;
            foreach (ListViewItem lvi in auctionsListView.Items)
            {
                Auction auc = m_bidder.auctions[lvi.Text];
                if (isAllUseCard)
                {
                    if (auc.isUseCreditCard)
                        continue;
                    auc.isUseCreditCard = true;
                    if (BiddingCompany.DS == Auction.DefaultBiddingCompany)
                    {
                        auc.serviceCharge = auc.serviceCharge + Convert.ToInt32(Auction.CreditCardRate * (float)(auc.serviceCharge + auc.hammerPrice));
                    }
                    else
                    {
                        auc.serviceCharge = auc.serviceCharge + Convert.ToInt32(Auction.CreditCardRate * (float)auc.hammerPrice);
                    }
                    auc.total = auc.hammerPrice + auc.serviceCharge;
                    lvi.SubItems[5].Text = m_yesStr;
                    lvi.BackColor = Color.LightCyan;
                }
                else
                {
                    auc.isUseCreditCard = false;
                    auc.serviceCharge = Convert.ToInt32(Auction.ServiceChargeRate * (float)auc.hammerPrice);
                    auc.total = auc.hammerPrice + auc.serviceCharge;
                    lvi.SubItems[5].Text = m_noStr;
                    lvi.BackColor = System.Drawing.SystemColors.Window;
                }
                lvi.SubItems[3].Text = auc.serviceCharge.ToString("n0");
                lvi.SubItems[4].Text = auc.total.ToString("n0");
            }

            if (Auction.DefaultBiddingCompany == BiddingCompany.DS)
                UpdateTotalLabels();
        }

        private void auctionsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (Auction.DefaultBiddingCompany == BiddingCompany.DS)
                UpdateTotalLabels();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void UpdateTotalLabels()
        {
            int totalCount = 0;
            int totalHammerPrice = 0;
            int totalServiceCharge = 0;
            int totalTotal = 0;
            foreach (ListViewItem lvi in auctionsListView.CheckedItems)
            {
                Auction auc = m_bidder.auctions[lvi.Text];
                totalCount++;
                totalHammerPrice += auc.hammerPrice;
                totalServiceCharge += auc.serviceCharge;
                totalTotal += auc.total;
            }
            totalCountLabel.Text = "總件數：" + totalCount.ToString("n0");
            totalHammerPriceLabel.Text = "總落槌價：" + totalHammerPrice.ToString("n0");
            totalServiceChargeLabel.Text = "總服務費：" + totalServiceCharge.ToString("n0");
            totalTotalLabel.Text = "總成交價：" + totalTotal.ToString("n0");
        }

        private void SetDataInDoc()
        {
            if (m_wordApp == null)
                m_wordApp = new Microsoft.Office.Interop.Word.Application();
            Dictionary<string, int> totalSums = new Dictionary<string, int>();
            if (isPrintOneByOneCheckBox.Checked)
            {
                string defaultAuctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
                Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_dealTemplateFN + defaultAuctioneer + ".dot";
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    auc.docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + auc.lot.ToString() + m_wordExtension;

                    auc.dealDoc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
                    Microsoft.Office.Interop.Word.Table bidderDataTable = auc.dealDoc.Tables[1];
                    FillBidderTable(bidderDataTable);

                    Microsoft.Office.Interop.Word.Table auctionTable = auc.dealDoc.Tables[2];

                    auctionTable.Rows.Add(auctionTable.Rows[2]);
                    FillAuctionRow(auctionTable, 2, auc.lot.ToString(), auc.artwork, auc.hammerPrice,
                        auc.serviceCharge, 0, auc.total);

                    int creditCardFee = auc.isUseCreditCard ? Convert.ToInt32(auc.total * 0.035f) : 0;
                    int amountDue = auc.isUseCreditCard ? auc.total + creditCardFee /*+ tax*/ : auc.total /*+ tax*/;

                    auctionTable.Cell(4, 2).Range.Text = auc.hammerPrice.ToString("n0");
                    auctionTable.Cell(4, 3).Range.Text = auc.serviceCharge.ToString("n0");
                    auctionTable.Cell(4, 4).Range.Text = auc.total.ToString("n0");

                    if (auc.isUseCreditCard)
                    {
                        auctionTable.Cell(5, 2).Range.Text = "NTD " + creditCardFee.ToString("n0");
                        auctionTable.Cell(6, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                    }
                    else
                    {
                        auctionTable.Rows[5].Delete();
                        auctionTable.Cell(5, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                    }
                }
            }   // end of isPrintOneByOneCheckBox.Checked == true
            else
            {
                string auctioneer = Utility.GetEnumString(typeof(BiddingCompany), (int)Auction.DefaultBiddingCompany);
                Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" +
                    Settings.docTempFolder + @"\" + m_dealTemplateFN + auctioneer + ".dot";
                string docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + auctioneer + m_wordExtension;

                Microsoft.Office.Interop.Word._Document doc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing,
                                                                                        ref m_oMissing, ref m_oMissing);
                Microsoft.Office.Interop.Word.Table bidderDataTable = doc.Tables[1];
                FillBidderTable(bidderDataTable);

                Microsoft.Office.Interop.Word.Table aucTable = doc.Tables[2];
                int hammerSum = 0;
                int serviceSum = 0;
                int taxSum = 0;
                int sums = 0;
                int aucCount = 0;
                Dictionary<string, Auction> auctionsOfAuctioneer = m_bidder.auctions;   // use GetAuctions(auctioneer) to multi bidding company
                if (auctionsOfAuctioneer != null)
                {
                    foreach (ListViewItem aucChecked in auctionsListView.CheckedItems)
                    {
                        Auction auc = auctionsOfAuctioneer[aucChecked.SubItems[0].Text];
                        aucTable.Rows.Add(aucTable.Rows[2 + aucCount]);
                        int tax = Convert.ToInt32(auc.serviceCharge * m_taxRate);
                        FillAuctionRow(aucTable, 2 + aucCount, auc.lot, auc.artwork, auc.hammerPrice,
                            auc.serviceCharge, tax, auc.total + tax);
                        aucCount += 1;
                        hammerSum += auc.hammerPrice;
                        serviceSum += auc.serviceCharge;
                        if (BiddingCompany.SFJ == Auction.DefaultBiddingCompany)
                        {
                            taxSum += tax;
                            sums += (auc.total + tax);
                        }
                        else
                        {
                            sums += auc.total;
                        }
                    }
                }

                int amountDue = sums;
                aucTable.Cell(aucCount + 3, 2).Range.Text = hammerSum.ToString("n0");
                aucTable.Cell(aucCount + 3, 3).Range.Text = serviceSum.ToString("n0");

                if (BiddingCompany.SFJ == Auction.DefaultBiddingCompany)
                {
                    aucTable.Cell(aucCount + 3, 4).Range.Text = taxSum.ToString("n0");
                    aucTable.Cell(aucCount + 3, 5).Range.Text = sums.ToString("n0");
                    aucTable.Cell(aucCount + 4, 2).Range.Text = GenTotalDesc(amountDue, "JPY");
                }
                else
                {
                    aucTable.Cell(aucCount + 3, 4).Range.Text = sums.ToString("n0");
                    aucTable.Cell(aucCount + 4, 2).Range.Text = GenTotalDesc(amountDue, "NTD");
                }

                string time = DateTime.Now.ToString(@"yyyy/MM/dd HH:mm");
                if (BiddingCompany.M != Auction.DefaultBiddingCompany)
                {
                    aucTable.Cell(aucCount + 4, 4).Range.Text = time;
                }
                totalSums.Add(time, amountDue);

                m_bidder.dealDocs[auctioneer].name = docName;
                m_bidder.dealDocs[auctioneer].doc = doc;

                SetCashFlowDoc(ref totalSums);
            }
        }

        //產生文字
        //原 NTD 2,750,000
        //抵帳 NTD 750,000
        //總計NTD 2,000,000
        private string GenTotalDesc(int total, string coinType)
        {
            int directDiscount = 0;

            int.TryParse(directDiscountTextBox.Text, out directDiscount);

            string result = "";
            if (directDiscount > 0)
            {
                result += string.Format("原 {0} {1}\n", coinType, total.ToString("n0"));
                result += string.Format("抵 {0} {1}\n", coinType, directDiscount.ToString("n0"));
                result += string.Format("= ");
            }

            result += string.Format("{0} {1}", coinType, GenTotalPrice(total).ToString("n0"));

            return result;
        }

        private int GenTotalPrice(int total)
        {
            int directDiscount = 0;

            int.TryParse(directDiscountTextBox.Text, out directDiscount);

            return total - directDiscount;
        }

        private string GetCheckoutTime(int tableId)
        {
            int checkoutNum = tableId + 1;
            if (m_checkoutTime.ContainsKey(checkoutNum))
                return m_checkoutTime[checkoutNum];
            else
                return DateTime.Now.ToString(@"yyyy/MM/dd HH:mm");
        }

        private void FillBidderTable(Microsoft.Office.Interop.Word.Table bidderDataTable)
        {
            bidderDataTable.Cell(1, 2).Range.Text = m_bidder.name;
            //bidderDataTable.Cell(1, 4).Range.Text = DateTime.Today.ToShortDateString();
            bidderDataTable.Cell(2, 2).Range.Text = m_bidder.phone;
            bidderDataTable.Cell(2, 4).Range.Text = m_bidder.no.ToString();
            bidderDataTable.Cell(3, 2).Range.Text = m_bidder.fax;
            bidderDataTable.Cell(3, 4).Range.Text = m_bidder.email;
            bidderDataTable.Cell(4, 2).Range.Text = m_bidder.addr;

            if (BiddingCompany.G == Auction.DefaultBiddingCompany)
            {
                bidderDataTable.Cell(4, 4).Range.Text = string.Format("{0}%", Settings.serviceChargeRate);
            }
            else
            {
                bidderDataTable.Cell(4, 3).Range.Text = m_bidder.auctioneer.ToString();
            }
        }

        private void FillAuctionRow
        (
            Microsoft.Office.Interop.Word.Table auctionTable,
            int rowId,
            string lot,
            string name,
            int hammerPrice,
            int serviceCharge,
            int tax,
            int total
        )
        {
            auctionTable.Cell(rowId, 1).Range.Text = lot;
            auctionTable.Cell(rowId, 2).Range.Text = name;
            auctionTable.Cell(rowId, 3).Range.Text = hammerPrice.ToString("n0");
            auctionTable.Cell(rowId, 4).Range.Text = serviceCharge.ToString("n0");
            if (BiddingCompany.SFJ == Auction.DefaultBiddingCompany)
            {
                auctionTable.Cell(rowId, 5).Range.Text = tax.ToString("n0");
                auctionTable.Cell(rowId, 6).Range.Text = total.ToString("n0");
            }
            else
            {
                auctionTable.Cell(rowId, 5).Range.Text = total.ToString("n0");
            }
        }

        private void SetCashFlowDoc(ref Dictionary<string, int> totalSums)
        {
            Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" +
                Settings.docTempFolder + @"\" + m_cashFlowTemplateFN;
            m_bidder.cashFlowDocName = m_bidder.no.ToString() + "_" + m_bidder.name + "_金流單" + m_wordExtension;
            m_bidder.cashFlowDoc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
            object oBookMark = "Today";
            if (BiddingCompany.M == Auction.DefaultBiddingCompany || BiddingCompany.N == Auction.DefaultBiddingCompany)
                m_bidder.cashFlowDoc.Bookmarks.get_Item(ref oBookMark).Range.Text = " " + DateTime.Now.ToString(@"yyyy/MM/dd HH:mm");

            Microsoft.Office.Interop.Word.Table bidderDataTable = m_bidder.cashFlowDoc.Tables[1];
            bidderDataTable.Cell(1, 2).Range.Text = m_bidder.name;
            bidderDataTable.Cell(2, 2).Range.Text = m_bidder.phone;
            bidderDataTable.Cell(2, 4).Range.Text = m_bidder.no.ToString();
            bidderDataTable.Cell(3, 2).Range.Text = m_bidder.fax;
            bidderDataTable.Cell(3, 4).Range.Text = m_bidder.email;
            bidderDataTable.Cell(4, 2).Range.Text = m_bidder.addr;
            bidderDataTable.Cell(4, 3).Range.Text = m_bidder.auctioneer.ToString();

            Microsoft.Office.Interop.Word.Table depositReceiveTable = m_bidder.cashFlowDoc.Tables[2];
            if (BiddingCompany.M == Auction.DefaultBiddingCompany)
            {
                depositReceiveTable.Cell(0, 4).Range.Text = m_bidder.payGuaranteeState.ToString();
                depositReceiveTable.Cell(0, 6).Range.Text = m_bidder.payGuaranteeNum.ToString("n0");
            }
            if (BiddingCompany.S == Auction.DefaultBiddingCompany || BiddingCompany.N == Auction.DefaultBiddingCompany)
            {
                depositReceiveTable.Cell(0, 2).Range.Text = m_bidder.payGuaranteeState.ToString();
                depositReceiveTable.Cell(0, 4).Range.Text = m_bidder.payGuaranteeNum.ToString("n0");
            }

            Microsoft.Office.Interop.Word.Table totalDataTable = m_bidder.cashFlowDoc.Tables[3];
            int counter = 0;
            string auctioneerStr = totalDataTable.Cell(2, 1).Range.Text;
            foreach (KeyValuePair<string, int> sum in totalSums)
            {
                if (counter != totalSums.Count - 1)
                    totalDataTable.Rows.Add(totalDataTable.Rows[2 + counter]);

                if (BiddingCompany.M == Auction.DefaultBiddingCompany)
                    totalDataTable.Cell(2 + counter, 1).Range.Text = auctioneerStr;
                if (BiddingCompany.S == Auction.DefaultBiddingCompany || BiddingCompany.N == Auction.DefaultBiddingCompany)
                    totalDataTable.Cell(2 + counter, 1).Range.Text = sum.Key;

                totalDataTable.Cell(2 + counter, 2).Range.Text = GenTotalPrice(sum.Value).ToString("n0");
                counter++;
            }
            /*for (int i = 0; i < (int)Auctioneer.Count; i++)
                totalDataTable.Cell(2 + i, 2).Range.Text = totalSums[i].ToString("n0");*/

            /*Microsoft.Office.Interop.Word.Table depositReturnTable = m_bidder.cashFlowDoc.Tables[4];
            depositReturnTable.Cell(0, 2).Range.Text = Utility.GetEnumString(typeof(AuctioneerName), (int)m_bidder.auctioneer);*/
        }

        private void UpdateListView()
        {
            auctionsListView.BeginUpdate();
            auctionsListView.Items.Clear();
            foreach (Auction auc in m_bidder.auctions.Values)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = auc.lot;
                lvi.SubItems.Add(auc.artwork);
                lvi.SubItems.Add(auc.hammerPrice.ToString("n0"));
                lvi.SubItems.Add(auc.serviceCharge.ToString("n0"));
                lvi.SubItems.Add(auc.total.ToString("n0"));
                lvi.SubItems.Add(auc.isUseCreditCard ? m_yesStr : m_noStr);
                lvi.Checked = true;
                auctionsListView.Items.Add(lvi);
            }
            auctionsListView.EndUpdate();
        }

        private void SaveAllDoc()
        {
            SetDataInDoc();

            string folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath,
                m_bidder.no.ToString() + "_" + m_bidder.name.ToString());
            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                }
                catch (Exception e) // invalid folder name, create simple name instead.
                {
                    folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath,
                                    m_bidder.no.ToString());
                    Directory.CreateDirectory(folderPath);
                }
            }
            else
            {
                if (DialogResult.OK == MessageBox.Show(String.Format("是否覆蓋目前買家{0}的結帳資料", m_bidder.no), "", MessageBoxButtons.OKCancel))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                else
                {
                    return;
                }
            }

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    if (!auc.dealDoc.Saved)
                    {
                        string filePath = Path.Combine(folderPath, auc.docName);
                        if (File.Exists(filePath))
                            continue;
                        auc.dealDoc.SaveAs(filePath);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, DealDoc> dealDoc in m_bidder.dealDocs)
                {
                    if (null == dealDoc.Value.doc || 0 == Settings.dealDocPrintCount)
                        continue;
                    string filePath = Path.Combine(folderPath, dealDoc.Value.name);
                    if (File.Exists(filePath))
                        continue;
                    if (!dealDoc.Value.doc.Saved)
                    {
                        try
                        {
                            dealDoc.Value.doc.SaveAs(filePath);
                        }
                        catch (Exception e)
                        {
                            filePath = Path.Combine(folderPath, m_bidder.no + "_" + dealDoc.Key + m_wordExtension);
                            dealDoc.Value.doc.SaveAs(filePath);
                        }
                    }
                }
            }

            if (!m_bidder.cashFlowDoc.Saved && 0 != Settings.cashFlowDocPrintCount)
            {
                string filePath = Path.Combine(folderPath, m_bidder.cashFlowDocName);
                if (!File.Exists(filePath))
                {
                    m_bidder.cashFlowDoc.SaveAs(filePath);
                }
            }
            MessageBox.Show(String.Format("買家 {0} 結帳資料已儲存在\n{1}", m_bidder.no.ToString(), folderPath), "", MessageBoxButtons.OK);
            toolStripStatusLabel1.Text = String.Format("結帳資料已儲存在{0}", folderPath);
        }

        private void CloseDocAndWord()
        {
            if (m_bidder == null)
                return;

            foreach (Auction auc in m_bidder.auctions.Values)
            {
                if (auc.dealDoc != null)
                {
                    CloseDoc(ref auc.dealDoc);
                }
            }

            if (m_bidder.dealDocs != null)
            {
                foreach (KeyValuePair<string, DealDoc> dealDoc in m_bidder.dealDocs)
                {
                    if (dealDoc.Value.doc != null)
                    {
                        CloseDoc(ref dealDoc.Value.doc);
                        dealDoc.Value.name = "";
                    }
                }
            }

            if (m_bidder.cashFlowDoc != null)
            {
                CloseDoc(ref m_bidder.cashFlowDoc);
            }

            if (m_wordApp != null)
            {
                if (m_wordApp.NormalTemplate != null)
                    m_wordApp.NormalTemplate.Saved = true;
                m_wordApp.Quit(ref m_oMissing, ref m_oMissing, ref m_oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_wordApp);
                m_wordApp = null;
            }
        }

        private void PrintDoc(Microsoft.Office.Interop.Word._Document doc, int copies)
        {
            object Background = true;
            object Range = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
            object Copies = copies;
            object PageType = Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages;
            object PrintToFile = false;
            object Collate = false;
            object ActivePrinterMacGX = m_oMissing;
            object ManualDuplexPrint = false;
            object PrintZoomColumn = 1;
            object PrintZoomRow = 1;

            doc.PrintOut(ref Background, ref m_oMissing, ref Range, ref m_oMissing,
                ref m_oMissing, ref m_oMissing, ref m_oMissing, ref Copies,
                ref m_oMissing, ref PageType, ref PrintToFile, ref Collate,
                ref m_oMissing, ref ManualDuplexPrint, ref PrintZoomColumn,
                ref PrintZoomRow, ref m_oMissing, ref m_oMissing);
        }

        private void CloseDoc(ref Microsoft.Office.Interop.Word._Document doc)
        {
            object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            doc.Close(ref saveOption, ref m_oMissing, ref m_oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
            doc = null;
        }

        private void GetCheckoutInfo(ref List<AuctionEntity> auctionEnts)
        {
            int maxNum = 0;
            m_checkoutTime.Clear();
            for (int i = 0; i < auctionEnts.Count; i++)
            {
                int coNum = auctionEnts[i].CheckoutNumber;
                string time = auctionEnts[i].CheckoutTime;
                if (coNum > maxNum)
                    maxNum = coNum;

                if (coNum != 0 && time != "" && !m_checkoutTime.ContainsKey(coNum))
                    m_checkoutTime.Add(coNum, time);
            }
            m_maxCheckoutNumber = maxNum;
        }

        private void UpdateCheckoutInfo()
        {
            int checkoutNum = m_maxCheckoutNumber + 1;
            bool hasNewBuyed = false;
            foreach (Auction auc in m_bidder.auctions.Values)
            {
                if (auc.checkoutNumber == 0)
                {
                    auc.checkoutNumber = checkoutNum;
                    auc.checkoutTime = DateTime.Now.ToString(@"yyyy/MM/dd HH:mm");
                    m_auctionInternet.UpdateField<string, int>(ae => ae.AuctionId, auc.lot, ae => ae.CheckoutNumber, auc.checkoutNumber);
                    m_auctionInternet.UpdateField<string, string>(ae => ae.AuctionId, auc.lot, ae => ae.CheckoutTime, auc.checkoutTime);
                    if (!hasNewBuyed)
                        hasNewBuyed = true;
                }
            }
            if (hasNewBuyed)
                m_maxCheckoutNumber = checkoutNum;
        }

        private void UpdateAllInListView()
        {
            foreach (ListViewItem lvi in auctionsListView.Items)
            {
                Auction auc = m_bidder.auctions[lvi.Text];
                if (lvi.SubItems[5].Text != m_noStr)
                {
                    auc.serviceCharge = Convert.ToInt32((Auction.ServiceChargeRate + Auction.CreditCardRate) * (float)auc.hammerPrice);
                    auc.total = auc.hammerPrice + auc.serviceCharge;
                }
                else
                {
                    auc.serviceCharge = Convert.ToInt32(Auction.ServiceChargeRate * (float)auc.hammerPrice);
                    auc.total = auc.hammerPrice + auc.serviceCharge;
                }
                lvi.SubItems[3].Text = auc.serviceCharge.ToString("n0");
                lvi.SubItems[4].Text = auc.total.ToString("n0");
            }
        }
        #endregion
    }
}
