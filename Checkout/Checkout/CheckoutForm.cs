﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UtilityLibrary;
using Bidding;
using InternetLibrary;
using Microsoft.Office.Interop.Word;

namespace Checkout
{
    public partial class CheckoutForm : Form
    {
        #region Events
        #endregion

        #region Enums, Structs, and Classes
        #endregion

        #region Member Variables
        private Microsoft.Office.Interop.Word._Application m_wordApp;
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
        private int m_printCountIfBuyed;
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
        }
        #endregion

        #region Windows Form Events
        private void CheckoutForm_Load(object sender, EventArgs e)
        {
            string settingsFP = Path.Combine(System.Windows.Forms.Application.StartupPath, m_settingsFN);
            if (Utility.IsFileExist(settingsFP))
                LoadSettings(settingsFP);
            string ip = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入Server IP", "127.0.0.1", -1, -1);
            if (ip.Length == 0)
                return;
            m_auctionInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_auctionInternet.Connect();
            m_bidderInternet = new Internet<BidderEntity>(ip, "bidding_data", "buyer_table");
            m_bidderInternet.Connect();
            if (m_auctionInternet.IsConnected && m_bidderInternet.IsConnected)
            {
                SetButtonsEnable(true);
            }
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

            int bidderNo = 0;
            if (Int32.TryParse(bidderNoTextBox.Text, out bidderNo))
            {
                List<AuctionEntity> auctions = m_auctionInternet.Find<string>(ae => ae.BidderNumber, bidderNo.ToString());
                BidderEntity bidder = m_bidderInternet.FineOne<int>(b => b.BidderID_int, bidderNo);
                if (auctions.Count == 0 || bidder == null)
                {
                    MessageBox.Show("查詢不到此買家");
                    SetButtonsEnable(false);
                    return;
                }

                SetBidder(bidder, auctions);
                UpdateListView();
                saveButton.Enabled = true;
                printButton.Enabled = true;
                isUseCardButton.Enabled = true;
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
            SavePaymentDoc();
            CloseDocAndWord();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            SavePaymentDoc();

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    PrintDoc(auc.paymentDoc, 1);
                }
            }
            else
            {
                foreach (KeyValuePair<string, PaymentDoc> doc in m_bidder.paymentDocs)
                {
                    if (m_bidder.auctionMappings[doc.Key].Count > 0)
                    {
                        PrintDoc(doc.Value.doc, 3);
                    }
                    else
                    {
                        PrintDoc(doc.Value.doc, 1);
                    }
                }
            }

            CloseDocAndWord();
        }

        private void bidderNoTextBox_TextChanged(object sender, System.EventArgs e)
        {
            saveButton.Enabled = false;
            printButton.Enabled = false;
            isUseCardButton.Enabled = false;

            if (bidderNoTextBox.Text.Length == 0)
                return;

            if (Utility.ParseToInt(bidderNoTextBox.Text) == -1)
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
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.auctionsListView.ListViewItemSorter = new ListViewItemComparer(e.Column, auctionsListView.Sorting);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            string ip = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入Server IP", "127.0.0.1", -1, -1);
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
                if (lvi.SubItems[5].Text == m_noStr)
                {
                    m_bidder.auctions[lvi.Text].isUseCreditCard = true;
                    lvi.SubItems[5].Text = m_yesStr;
                    lvi.BackColor = Color.LightCyan;
                }
                else
                {
                    m_bidder.auctions[lvi.Text].isUseCreditCard = false;
                    lvi.SubItems[5].Text = m_noStr;
                    lvi.BackColor = System.Drawing.SystemColors.Window;
                }
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void SetDataInDoc()
        {
            m_wordApp = new Microsoft.Office.Interop.Word.Application();

            if (isPrintOneByOneCheckBox.Checked)
            {
                Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_paymentTemplateFN + Company.S.ToString() + ".dot";
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    auc.docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + auc.lot.ToString() + ".doc";

                    auc.paymentDoc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
                    Microsoft.Office.Interop.Word.Table bidderDataTable = auc.paymentDoc.Tables[1];
                    FillBidderTable(bidderDataTable);

                    Microsoft.Office.Interop.Word.Table auctionTable = auc.paymentDoc.Tables[2];

                    auctionTable.Rows.Add(auctionTable.Rows[2]);
                    FillAuctionRow(auctionTable, 2, auc.lot.ToString(), auc.name, auc.hammerPrice.ToString("n0"),
                        auc.serviceCharge.ToString("n0"), auc.total.ToString("n0"));

                    int creditCardFee = auc.isUseCreditCard ? Convert.ToInt32(auc.total * 0.035f) : 0;
                    //int tax = Convert.ToInt32(auc.total * 0.05f);
                    int amountDue = auc.isUseCreditCard ? auc.total + creditCardFee /*+ tax*/ : auc.total /*+ tax*/;

                    auctionTable.Cell(4, 2).Range.Text = auc.hammerPrice.ToString("n0");
                    auctionTable.Cell(4, 3).Range.Text = auc.serviceCharge.ToString("n0");
                    auctionTable.Cell(4, 4).Range.Text = auc.total.ToString("n0");
                    if (auc.isUseCreditCard)
                    {
                        auctionTable.Cell(5, 2).Range.Text = "NTD " + creditCardFee.ToString("n0");
                        //auctionTable.Cell(6, 2).Range.Text = "NTD " + tax.ToString("n0");
                        //auctionTable.Cell(7, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                        auctionTable.Cell(6, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                    }
                    else
                    {
                        auctionTable.Rows[5].Delete();
                        //auctionTable.Cell(5, 2).Range.Text = "NTD " + tax.ToString("n0");
                        //auctionTable.Cell(6, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                        auctionTable.Cell(5, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                    }
                }
            }
            else
            {
                for (int i = 0; i < (int)Company.Count; i++)
                {
                    string company = Utility.GetEnumString(typeof(Company), i);
                    Object tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_paymentTemplateFN + company + ".dot";
                    string docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + company + ".doc";

                    m_bidder.paymentDocs[company].doc = m_wordApp.Documents.Add(ref tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);

                    Microsoft.Office.Interop.Word.Table bidderDataTable = m_bidder.paymentDocs[company].doc.Tables[1];
                    FillBidderTable(bidderDataTable);

                    Microsoft.Office.Interop.Word.Table auctionTable = m_bidder.paymentDocs[company].doc.Tables[2];
                    int hammerSum = 0;
                    int serviceSum = 0;
                    int totalSum = 0;
                    int aucId = 0;
                    foreach (Auction auc in m_bidder.auctions.Values)
                    {
                        auctionTable.Rows.Add(auctionTable.Rows[2 + aucId]);
                        FillAuctionRow(auctionTable, 2 + aucId, auc.lot.ToString(), auc.name, auc.hammerPrice.ToString("n0"),
                            auc.serviceCharge.ToString("n0"), auc.total.ToString("n0"));
                        aucId += 1;
                        hammerSum += auc.hammerPrice;
                        serviceSum += auc.serviceCharge;
                        totalSum += auc.total;
                    }

                    int creditCardFee = Convert.ToInt32(totalSum * 0.035f);
                    //int tax = Convert.ToInt32(totalSum * 0.05f);
                    int amountDue = totalSum + creditCardFee;
                    int aucCount = m_bidder.auctions.Values.Count;

                    auctionTable.Cell(aucCount + 3, 2).Range.Text = hammerSum.ToString("n0");
                    auctionTable.Cell(aucCount + 3, 3).Range.Text = serviceSum.ToString("n0");
                    auctionTable.Cell(aucCount + 3, 4).Range.Text = totalSum.ToString("n0");
                    //auctionTable.Cell(aucCount + 4, 2).Range.Text = "NTD " + creditCardFee.ToString("n0");
                    //auctionTable.Cell(6, 2).Range.Text = "NTD " + tax.ToString("n0");
                    //auctionTable.Cell(7, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                    auctionTable.Cell(aucCount + 4, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                }
            }
        }

        private void FillBidderTable(Microsoft.Office.Interop.Word.Table bidderDataTable)
        {
            bidderDataTable.Cell(1, 2).Range.Text = m_bidder.name;
            bidderDataTable.Cell(1, 4).Range.Text = DateTime.Today.ToShortDateString();
            bidderDataTable.Cell(2, 2).Range.Text = m_bidder.phone;
            bidderDataTable.Cell(2, 4).Range.Text = m_bidder.no.ToString();
            bidderDataTable.Cell(3, 2).Range.Text = m_bidder.fax;
            bidderDataTable.Cell(3, 4).Range.Text = m_bidder.email;
            bidderDataTable.Cell(4, 2).Range.Text = m_bidder.addr;
        }

        private void FillAuctionRow(Microsoft.Office.Interop.Word.Table auctionTable, int rowId, string lot, string name, string hammerPrice,
            string serviceCharge, string total)
        {
            auctionTable.Cell(rowId, 1).Range.Text = lot;
            auctionTable.Cell(rowId, 2).Range.Text = name;
            auctionTable.Cell(rowId, 3).Range.Text = hammerPrice;
            auctionTable.Cell(rowId, 4).Range.Text = serviceCharge;
            auctionTable.Cell(rowId, 5).Range.Text = total;
        }

        private void UpdateListView()
        {
            auctionsListView.BeginUpdate();
            auctionsListView.Items.Clear();
            foreach (Auction auc in m_bidder.auctions.Values)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = auc.lot;
                lvi.SubItems.Add(auc.name);
                lvi.SubItems.Add(auc.hammerPrice.ToString("n0"));
                lvi.SubItems.Add(auc.serviceCharge.ToString("n0"));
                lvi.SubItems.Add(auc.total.ToString("n0"));
                lvi.SubItems.Add(auc.isUseCreditCard ? m_yesStr : m_noStr);
                auctionsListView.Items.Add(lvi);
            }
            auctionsListView.EndUpdate();
        }

        private bool SetBidder(BidderEntity bidder, List<AuctionEntity> auctions)
        {
            m_bidder = new Bidder();
            m_bidder.name = bidder.Name;   //ignore first string of id.
            m_bidder.no = bidder.BidderID_int;
            m_bidder.phone = bidder.Tel;
            m_bidder.fax = bidder.Fax;
            m_bidder.email = bidder.EMail;
            m_bidder.addr = bidder.EMail;
            m_bidder.auctions = new Dictionary<string, Auction>();
            for (int i = 0; i < auctions.Count; i++)
            {
                AuctionEntity ae = auctions[i];
                Auction auction = new Auction();
                auction.lot = ae.AuctionId;
                auction.name = ae.Name;
                auction.hammerPrice = ae.HammerPrice;
                auction.ComputeChargeAndTotal();
                m_bidder.auctions[auction.lot] = auction;
            }
            return true;
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
                sw.WriteLine(m_auctionInternet.IP + " " + m_serverPort);
            }
        }

        private void SavePaymentDoc()
        {
            SetDataInDoc();

            if (isPrintOneByOneCheckBox.Checked)
            {
                string folder = Path.Combine(System.Windows.Forms.Application.StartupPath, m_bidder.no.ToString());
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    if (!auc.paymentDoc.Saved)
                    {
                        string filePath = Path.Combine(folder, auc.docName);
                        auc.paymentDoc.SaveAs(filePath);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, PaymentDoc> paymentDoc in m_bidder.paymentDocs)
                {
                    if (!paymentDoc.Value.doc.Saved)
                        paymentDoc.Value.doc.SaveAs(Path.Combine(System.Windows.Forms.Application.StartupPath, paymentDoc.Value.name));
                }
            }
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

            foreach (KeyValuePair<string, PaymentDoc> paymentDoc in m_bidder.paymentDocs)
            {
                if (paymentDoc.Value.doc != null)
                {
                    CloseDoc(ref paymentDoc.Value.doc);
                    paymentDoc.Value.name = "";
                }
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
        #endregion
    }
}
