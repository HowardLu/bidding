//#define MUCHUNTANG
//#define SHIJIA
//#define SJ_FOR_JP

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Bidding;
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
        private string m_paymentTemplateFN = "拍賣成交單_";
        private Bidder m_bidder;
        private Internet<AuctionEntity> m_auctionInternet;
        private Internet<BidderEntity> m_bidderInternet;
        private Object m_oMissing = System.Reflection.Missing.Value;
        private string m_serverIp = @"127.0.0.1";
        private int m_serverPort = 27017;
        private Size m_listViewSize;
        private int[] m_colWidths;
        private int m_sortColumnId = -1;
        private string m_settingsFN = "settings.txt";
        private string m_yesStr = "是";
        private string m_noStr = "否";
        private string m_cashFlowTemplateFN = "金流單_";
        private int m_maxCheckoutNumber = 0;
        private Dictionary<int, string> m_checkoutTime;
#if SJ_FOR_JP
        private const float m_taxRate = 0.08f;
#endif
        #endregion

        #region Properties
        #endregion

        #region Constructors and Finalizers
        public CheckoutForm()
        {
            InitializeComponent();
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
            string settingsFP = Path.Combine(System.Windows.Forms.Application.StartupPath, m_settingsFN);
            if (Utility.IsFileExist(settingsFP, false))
                LoadSettings(settingsFP);
            m_serverIp = Utility.InputIp();
            if (m_serverIp.Length == 0)
            {
                MessageBox.Show("IP不可為空!!!");
                return;
            }
            m_auctionInternet = new Internet<AuctionEntity>(m_serverIp, "bidding_data", "auctions_table");
            m_auctionInternet.Connect();
            m_bidderInternet = new Internet<BidderEntity>(m_serverIp, "bidding_data", "buyer_table");
            m_bidderInternet.Connect();
            if (m_auctionInternet.IsConnected && m_bidderInternet.IsConnected)
            {
                SetButtonsEnable(true);
            }
            string defaultAuctioneer = Utility.GetEnumString(typeof(Auctioneer), 0);
#if SJ_FOR_JP
            m_cashFlowTemplateFN = m_cashFlowTemplateFN + "SFJ" + ".dot";
#else
            m_cashFlowTemplateFN = m_cashFlowTemplateFN + defaultAuctioneer + ".dot";
#endif
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
            //m_client.Close();
            SaveSettings();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (!m_auctionInternet.IsConnected)
                SetButtonsEnable(false);
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
                isUseCardButton.Enabled = true;
                serviceChargeRateTextBox.Enabled = false;
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
                isUseCardButton.Enabled = false;
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
            UpdateCheckoutInfo();

            SaveAllDoc();

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    PrintDoc(auc.paymentDoc, 1);
                }
            }
            else
            {
                foreach (KeyValuePair<string, PaymentDoc> paymentDoc in m_bidder.paymentDocs)
                {
                    PrintDoc(paymentDoc.Value.doc, 3);
/*
                    if (!m_bidder.auctionMappings.ContainsKey(paymentDoc.Key))
                        continue;   // dont print the doc that buy nothing.

                    if (m_bidder.auctionMappings[paymentDoc.Key].Count > 0)
                    {
                        PrintDoc(paymentDoc.Value.doc, 3);
                    }
                    else
                    {
                        PrintDoc(paymentDoc.Value.doc, 1);
                    }
*/
                }
            }

            PrintDoc(m_bidder.cashFlowDoc, 1);
            CloseDocAndWord();
        }

        private void bidderNoTextBox_TextChanged(object sender, System.EventArgs e)
        {
            saveButton.Enabled = false;
            printButton.Enabled = false;
            isUseCardButton.Enabled = false;

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
            if (e.Column != m_sortColumnId)
            {
                // Set the sort column to the new column.
                m_sortColumnId = e.Column;
                // Set the sort order to ascending by default.
                auctionsListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (auctionsListView.Sorting == SortOrder.Ascending)
                    auctionsListView.Sorting = SortOrder.Descending;
                else
                    auctionsListView.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            auctionsListView.Sort();
            this.auctionsListView.ListViewItemSorter = new ListViewItemComparer(e.Column, auctionsListView.Sorting);
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

        private void isUseCardButton_Click(object sender, EventArgs e)
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
        }

        private void serviceChargeRateTextBox_TextChanged(object sender, EventArgs e)
        {
            int serviceChargeRate = Utility.ParseToInt(serviceChargeRateTextBox.Text, false);
            if (-1 == serviceChargeRate)
            {
                serviceChargeRateTextBox.Text = "20";
                return;
            }
            Auction.ServiceChargeRate = (float)serviceChargeRate / 100.0f;
        }

        private void creditCardRateTextBox_TextChanged(object sender, EventArgs e)
        {
            int creditCardRate = Utility.ParseToInt(creditCardRateTextBox.Text, false);
            if (-1 == creditCardRate)
            {
                creditCardRateTextBox.Text = "8";
                return;
            }
            Auction.CreditCardRate = (float)creditCardRate / 100.0f;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void SetDataInDoc()
        {
            if (m_wordApp == null)
                m_wordApp = new Microsoft.Office.Interop.Word.Application();
            Dictionary<string, int> totalSums = new Dictionary<string, int>();
            if (isPrintOneByOneCheckBox.Checked)
            {
                string defaultAuctioneer = Utility.GetEnumString(typeof(Auctioneer), 0);
                Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_paymentTemplateFN + defaultAuctioneer + ".dot";
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    auc.docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + auc.lot.ToString() + ".doc";

                    auc.paymentDoc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
                    Microsoft.Office.Interop.Word.Table bidderDataTable = auc.paymentDoc.Tables[1];
                    FillBidderTable(bidderDataTable);

                    Microsoft.Office.Interop.Word.Table auctionTable = auc.paymentDoc.Tables[2];

                    auctionTable.Rows.Add(auctionTable.Rows[2]);
#if (!SJ_FOR_JP)
                    FillAuctionRow(auctionTable, 2, auc.lot.ToString(), auc.artwork, auc.hammerPrice.ToString("n0"),
                        auc.serviceCharge.ToString("n0"), auc.total.ToString("n0"));
#endif

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
                for (int i = 0; i < (int)Auctioneer.Count; i++)
                {
                    string auctioneer = Utility.GetEnumString(typeof(Auctioneer), i);
#if SJ_FOR_JP
                    Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_paymentTemplateFN + "SFJ" + ".dot";
#else
                    Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_paymentTemplateFN + auctioneer + ".dot";
#endif
                    string docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + auctioneer + ".doc";

                    Microsoft.Office.Interop.Word._Document doc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing,
                                                                                            ref m_oMissing, ref m_oMissing);
                    Microsoft.Office.Interop.Word.Table bidderDataTable = doc.Tables[1];
                    FillBidderTable(bidderDataTable);

                    Microsoft.Office.Interop.Word.Table aucTableTmp = doc.Tables[2];
                    aucTableTmp.Range.Copy();
                    Microsoft.Office.Interop.Word.Range rng = aucTableTmp.Range;
                    rng.SetRange(aucTableTmp.Range.End, aucTableTmp.Range.End);
                    rng.InsertBreak(Microsoft.Office.Interop.Word.WdBreakType.wdLineBreak);

                    int tableCount = m_maxCheckoutNumber > 0 ? m_maxCheckoutNumber : 1;
                    Microsoft.Office.Interop.Word.Table[] aucTables = new Microsoft.Office.Interop.Word.Table[tableCount];
                    aucTables[0] = aucTableTmp;
                    for (int tableId = 1; tableId < tableCount; tableId++)
                    {
                        aucTables[tableId] = doc.Tables.Add(rng, aucTableTmp.Rows.Count, aucTableTmp.Columns.Count);
                        aucTables[tableId].Range.Paste();
                        rng.SetRange(aucTables[tableId].Range.End, aucTables[tableId].Range.End);
                        rng.InsertBreak(Microsoft.Office.Interop.Word.WdBreakType.wdLineBreak);
                    }

                    int[] hammerSum = new int[tableCount];
                    int[] serviceSum = new int[tableCount];
#if SJ_FOR_JP
                    int[] taxSum = new int[tableCount];
#endif
                    int[] sums = new int[tableCount];
                    int[] aucCount = new int[tableCount];
                    //List<Auction> auctionsOfAuctioneer = m_bidder.GetAuctions(auctioneer); 
                    List<Auction> auctionsOfAuctioneer = m_bidder.GetAuctions("S"); // dirty way 20150118
                    if (auctionsOfAuctioneer == null)
                        auctionsOfAuctioneer = m_bidder.GetAuctions("N"); // dirty way 20150118
                    if (auctionsOfAuctioneer != null)
                    {
                        foreach (Auction auc in auctionsOfAuctioneer)
                        {
                            int tableId = auc.checkoutNumber > 0 ? auc.checkoutNumber - 1 : 0;
                            aucTables[tableId].Rows.Add(aucTables[tableId].Rows[2 + aucCount[tableId]]);
#if SJ_FOR_JP
                            int tax = 0;
                            tax = Convert.ToInt32(auc.serviceCharge * m_taxRate);
                            FillAuctionRow(aucTables[tableId], 2 + aucCount[tableId], auc.lot, auc.artwork, auc.hammerPrice,
                                auc.serviceCharge, tax, auc.total + tax);

#else
                            FillAuctionRow(aucTables[tableId], 2 + aucCount[tableId], auc.lot.ToString(), auc.artwork, auc.hammerPrice.ToString("n0"),
                                auc.serviceCharge.ToString("n0"), auc.total.ToString("n0"));
#endif
                            aucCount[tableId] += 1;
                            hammerSum[tableId] += auc.hammerPrice;
                            serviceSum[tableId] += auc.serviceCharge;
#if SJ_FOR_JP
                            taxSum[tableId] += tax;
                            sums[tableId] += (auc.total + tax);
#else
                            sums[tableId] += auc.total;
#endif
                        }
                    }

                    for (int tableId = 0; tableId < tableCount; tableId++)
                    {
                        int amountDue = sums[tableId];
                        string time = GetCheckoutTime(tableId);
                        aucTables[tableId].Cell(aucCount[tableId] + 3, 2).Range.Text = hammerSum[tableId].ToString("n0");
                        aucTables[tableId].Cell(aucCount[tableId] + 3, 3).Range.Text = serviceSum[tableId].ToString("n0");
#if SJ_FOR_JP
                        aucTables[tableId].Cell(aucCount[tableId] + 3, 4).Range.Text = taxSum[tableId].ToString("n0");
                        aucTables[tableId].Cell(aucCount[tableId] + 3, 5).Range.Text = sums[tableId].ToString("n0");
#else
                        aucTables[tableId].Cell(aucCount[tableId] + 3, 4).Range.Text = sums[tableId].ToString("n0");
#endif
#if SJ_FOR_JP
                        aucTables[tableId].Cell(aucCount[tableId] + 4, 2).Range.Text = "JPY " + amountDue.ToString("n0");
#else
                        aucTables[tableId].Cell(aucCount[tableId] + 4, 2).Range.Text = "NTD " + amountDue.ToString("n0");
#endif
#if SHIJIA || IGS
                        aucTables[tableId].Cell(aucCount[tableId] + 4, 4).Range.Text = time;
#endif
                        totalSums.Add(time, amountDue);
                    }

                    m_bidder.paymentDocs[auctioneer].name = docName;
                    m_bidder.paymentDocs[auctioneer].doc = doc;
                }

                SetCashFlowDoc(ref totalSums);
            }
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
            bidderDataTable.Cell(4, 3).Range.Text = m_bidder.auctioneer.ToString();
        }
#if SJ_FOR_JP
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
            auctionTable.Cell(rowId, 5).Range.Text = tax.ToString("n0");
            auctionTable.Cell(rowId, 6).Range.Text = total.ToString("n0");
        }
#else
        private void FillAuctionRow(Microsoft.Office.Interop.Word.Table auctionTable, int rowId, string lot, string name, string hammerPrice,
            string serviceCharge, string total)
        {
            auctionTable.Cell(rowId, 1).Range.Text = lot;
            auctionTable.Cell(rowId, 2).Range.Text = name;
            auctionTable.Cell(rowId, 3).Range.Text = hammerPrice;
            auctionTable.Cell(rowId, 4).Range.Text = serviceCharge;
            auctionTable.Cell(rowId, 5).Range.Text = total;
        }
#endif

        private void SetCashFlowDoc(ref Dictionary<string, int> totalSums)
        {
            Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_cashFlowTemplateFN;
            m_bidder.cashFlowDocName = m_bidder.no.ToString() + "_" + m_bidder.name + "_金流單.doc";
            m_bidder.cashFlowDoc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
            object oBookMark = "Today";
#if MUCHUNTANG || IGS
            m_bidder.cashFlowDoc.Bookmarks.get_Item(ref oBookMark).Range.Text = " " + DateTime.Now.ToString(@"yyyy/MM/dd HH:mm");
#endif
            Microsoft.Office.Interop.Word.Table bidderDataTable = m_bidder.cashFlowDoc.Tables[1];
            bidderDataTable.Cell(1, 2).Range.Text = m_bidder.name;
            bidderDataTable.Cell(2, 2).Range.Text = m_bidder.phone;
            bidderDataTable.Cell(2, 4).Range.Text = m_bidder.no.ToString();
            bidderDataTable.Cell(3, 2).Range.Text = m_bidder.fax;
            bidderDataTable.Cell(3, 4).Range.Text = m_bidder.email;
            bidderDataTable.Cell(4, 2).Range.Text = m_bidder.addr;
            bidderDataTable.Cell(4, 3).Range.Text = m_bidder.auctioneer.ToString();

            Microsoft.Office.Interop.Word.Table depositReceiveTable = m_bidder.cashFlowDoc.Tables[2];
#if MUCHUNTANG
            depositReceiveTable.Cell(0, 4).Range.Text = m_bidder.payGuaranteeState.ToString();
            depositReceiveTable.Cell(0, 6).Range.Text = m_bidder.payGuaranteeNum.ToString("n0");
#endif
#if SHIJIA || IGS
            depositReceiveTable.Cell(0, 2).Range.Text = m_bidder.payGuaranteeState.ToString();
            depositReceiveTable.Cell(0, 4).Range.Text = m_bidder.payGuaranteeNum.ToString("n0");
#endif

            Microsoft.Office.Interop.Word.Table totalDataTable = m_bidder.cashFlowDoc.Tables[3];
            int counter = 0;
            string auctioneerStr = totalDataTable.Cell(2, 1).Range.Text;
            foreach (KeyValuePair<string, int> sum in totalSums)
            {
                if (counter != totalSums.Count - 1)
                    totalDataTable.Rows.Add(totalDataTable.Rows[2 + counter]);

#if MUCHUNTANG
                totalDataTable.Cell(2 + counter, 1).Range.Text = auctioneerStr;
#endif
#if SHIJIA || IGS
                totalDataTable.Cell(2 + counter, 1).Range.Text = sum.Key;
#endif

                totalDataTable.Cell(2 + counter, 2).Range.Text = sum.Value.ToString("n0");
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
                if (auc.checkoutNumber > 0)
                    lvi.BackColor = Color.LightGray;
                auctionsListView.Items.Add(lvi);
            }
            auctionsListView.EndUpdate();
        }

        private void LoadSettings(string fp)
        {
            using (StreamReader sr = new StreamReader(fp))
            {
                string line = sr.ReadLine();
                if (line == null)
                {
                    MessageBox.Show(fp + "檔格式錯誤!");
                    return;
                }

                string[] info = line.Split(' ');
                m_serverIp = info[0];
                if (!int.TryParse(info[1], out m_serverPort))
                {
                    MessageBox.Show(fp + "檔格式錯誤!");
                    return;
                }
            }
        }

        private void SaveSettings()
        {
            string fp = Path.Combine(System.Windows.Forms.Application.StartupPath, m_settingsFN);
            using (StreamWriter sw = new StreamWriter(fp))
            {
                sw.WriteLine(m_serverIp + " " + m_serverPort);
            }
        }

        private void SaveAllDoc()
        {
            SetDataInDoc();

            string folder = Path.Combine(System.Windows.Forms.Application.StartupPath,
                m_bidder.no.ToString());
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    if (!auc.paymentDoc.Saved)
                    {
                        string filePath = Path.Combine(folder, auc.docName);
                        if (File.Exists(filePath))
                            continue;
                        auc.paymentDoc.SaveAs(filePath);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, PaymentDoc> paymentDoc in m_bidder.paymentDocs)
                {
                    string filePath = Path.Combine(folder, paymentDoc.Value.name);
                    if (File.Exists(filePath))
                        continue;
                    if (!paymentDoc.Value.doc.Saved)
                        paymentDoc.Value.doc.SaveAs(filePath);
                }
            }

            if (!m_bidder.cashFlowDoc.Saved)
            {
                string filePath = Path.Combine(folder, m_bidder.cashFlowDocName);
                if (!File.Exists(filePath))
                {
                    m_bidder.cashFlowDoc.SaveAs(filePath);
                }
            }
            toolStripStatusLabel1.Text = "Saved!";
        }

        private void CloseDocAndWord()
        {
            if (m_bidder == null)
                return;

            foreach (Auction auc in m_bidder.auctions.Values)
            {
                if (auc.paymentDoc != null)
                {
                    CloseDoc(ref auc.paymentDoc);
                }
            }

            if (m_bidder.paymentDocs != null)
            {
                foreach (KeyValuePair<string, PaymentDoc> paymentDoc in m_bidder.paymentDocs)
                {
                    if (paymentDoc.Value.doc != null)
                    {
                        CloseDoc(ref paymentDoc.Value.doc);
                        paymentDoc.Value.name = "";
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
        #endregion
    }
}
