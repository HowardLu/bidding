using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Checkout
{
    public partial class CheckoutForm : Form
    {
        private Microsoft.Office.Interop.Word._Application m_wordApp;
        private string m_paymentTemplateFN = "拍賣成交單-空白.dot";
        private Object m_tmpDocFN;
        private Bidder m_bidder;
        private SocketClient2 m_client;
        //private Thread receiveThread;
        private Object m_oMissing = System.Reflection.Missing.Value;
        private string m_serverIp = @"127.0.0.1";
        private int m_serverPort = 5566;
        private Size m_listViewSize;
        private int[] m_colWidths;
        private int m_sortColumnId = -1;
        private string m_settingsFN = "settings.txt";
        private string m_yesStr = "是";
        private string m_noStr = "否";
        private Microsoft.Office.Interop.Word._Document m_totalPaymentDoc;
        private string m_totalPaymentDocName;

        public CheckoutForm()
        {
            InitializeComponent();
            m_listViewSize = this.auctionsListView.Size;
            m_colWidths = new int[auctionsListView.Columns.Count];
            for (int i = 0; i < auctionsListView.Columns.Count; i++)
                m_colWidths[i] = auctionsListView.Columns[i].Width;
        }

        #region windows form event handler
        private void CheckoutForm_Load(object sender, EventArgs e)
        {
            string settingsFP = Path.Combine(Application.StartupPath, m_settingsFN);
            if (Utility.IsFileExist(settingsFP, m_settingsFN))
                LoadSettings(settingsFP);
            m_client = new SocketClient2(m_serverIp, m_serverPort);
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
            m_client.Close();
            SaveSettings();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            if (!m_client.IsConnected)
                ResetButtons();

            int bidderNo = 0;
            if (Int32.TryParse(bidderNoTextBox.Text, out bidderNo))
            {
                if (!m_client.Send(bidderNo.ToString()))
                {
                    ResetButtons();
                    return;
                }
                //receiveThread = new Thread(m_client.Receive);
                //receiveThread.Start();

                //while (!receiveThread.IsAlive) ;
                m_client.Receive();
                if (ParseAndSetBidder(m_client.m_responseString))
                {
                    UpdateListView();
                    saveButton.Enabled = true;
                    printButton.Enabled = true;
                    isUseCardButton.Enabled = true;
                }
                else
                {
                    MessageBox.Show("查詢不到此買家");
                }
            }
        }

        private void ResetButtons()
        {
            connectButton.Enabled = true;
            bidderNoTextBox.Enabled = false;
            previewButton.Enabled = false;
            saveButton.Enabled = false;
            printButton.Enabled = false;
            isUseCardButton.Enabled = false;
            toolStripStatusLabel1.Text = "連線中斷!請重新連線!";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SavePaymentDoc();
            CloseDocAndWord();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            SavePaymentDoc();

            object Background = true;
            object Range = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
            object Copies = 1;
            object PageType = Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages;
            object PrintToFile = false;
            object Collate = false;
            object ActivePrinterMacGX = m_oMissing;
            object ManualDuplexPrint = false;
            object PrintZoomColumn = 1;
            object PrintZoomRow = 1;

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    auc.paymentDoc.PrintOut(ref Background, ref m_oMissing, ref Range, ref m_oMissing,
                        ref m_oMissing, ref m_oMissing, ref m_oMissing, ref Copies,
                        ref m_oMissing, ref PageType, ref PrintToFile, ref Collate,
                        ref m_oMissing, ref ManualDuplexPrint, ref PrintZoomColumn,
                        ref PrintZoomRow, ref m_oMissing, ref m_oMissing);
                }
            }
            else
            {
                m_totalPaymentDoc.PrintOut(ref Background, ref m_oMissing, ref Range, ref m_oMissing,
                        ref m_oMissing, ref m_oMissing, ref m_oMissing, ref Copies,
                        ref m_oMissing, ref PageType, ref PrintToFile, ref Collate,
                        ref m_oMissing, ref ManualDuplexPrint, ref PrintZoomColumn,
                        ref PrintZoomRow, ref m_oMissing, ref m_oMissing);
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

            if (Utility.IsNumber(bidderNoTextBox.Text) == -1)
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
            string ip = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入Server IP", m_client.IP.ToString(), -1, -1);
            if (ip.Length == 0)
                return;
            if (m_client.Connect(ip))
            {
                bidderNoTextBox.Enabled = true;
                previewButton.Enabled = true;
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

        #region Methods
        private void SetDataInDoc()
        {
            m_wordApp = new Microsoft.Office.Interop.Word.Application();
            m_tmpDocFN = System.Windows.Forms.Application.StartupPath + @"\" + m_paymentTemplateFN;

            if (isPrintOneByOneCheckBox.Checked)
            {
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    auc.docName = m_bidder.no.ToString() + "_" + m_bidder.name + "_" + auc.lot.ToString() + ".doc";
                    auc.paymentDoc = m_wordApp.Documents.Add(ref m_tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);
                    Microsoft.Office.Interop.Word.Table bidderDataTable = auc.paymentDoc.Tables[1];
                    bidderDataTable.Cell(1, 2).Range.Text = m_bidder.name;
                    bidderDataTable.Cell(1, 4).Range.Text = DateTime.Today.ToShortDateString();
                    bidderDataTable.Cell(2, 2).Range.Text = m_bidder.phone;
                    bidderDataTable.Cell(2, 4).Range.Text = m_bidder.no.ToString();
                    bidderDataTable.Cell(3, 2).Range.Text = m_bidder.fax;
                    bidderDataTable.Cell(3, 4).Range.Text = m_bidder.email;
                    bidderDataTable.Cell(4, 2).Range.Text = m_bidder.addr;

                    Microsoft.Office.Interop.Word.Table auctionTable = auc.paymentDoc.Tables[2];

                    auctionTable.Rows.Add(auctionTable.Rows[2]);
                    auctionTable.Cell(2, 1).Range.Text = auc.lot.ToString();
                    auctionTable.Cell(2, 2).Range.Text = auc.name;
                    auctionTable.Cell(2, 3).Range.Text = auc.hammerPrice.ToString("n0");
                    auctionTable.Cell(2, 4).Range.Text = auc.serviceCharge.ToString("n0");
                    auctionTable.Cell(2, 5).Range.Text = auc.total.ToString("n0");

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
                m_totalPaymentDocName = m_bidder.no.ToString() + "_" + m_bidder.name + ".doc";
                m_totalPaymentDoc = m_wordApp.Documents.Add(ref m_tmpDocFN, ref m_oMissing, ref m_oMissing, ref m_oMissing);

                Microsoft.Office.Interop.Word.Table bidderDataTable = m_totalPaymentDoc.Tables[1];
                bidderDataTable.Cell(1, 2).Range.Text = m_bidder.name;
                bidderDataTable.Cell(1, 4).Range.Text = DateTime.Today.ToShortDateString();
                bidderDataTable.Cell(2, 2).Range.Text = m_bidder.phone;
                bidderDataTable.Cell(2, 4).Range.Text = m_bidder.no.ToString();
                bidderDataTable.Cell(3, 2).Range.Text = m_bidder.fax;
                bidderDataTable.Cell(3, 4).Range.Text = m_bidder.email;
                bidderDataTable.Cell(4, 2).Range.Text = m_bidder.addr;

                Microsoft.Office.Interop.Word.Table auctionTable = m_totalPaymentDoc.Tables[2];
                int hammerSum = 0;
                int serviceSum = 0;
                int totalSum = 0;
                int aucId = 0;
                foreach (Auction auc in m_bidder.auctions.Values)
                {
                    auctionTable.Rows.Add(auctionTable.Rows[2 + aucId]);
                    auctionTable.Cell(2 + aucId, 1).Range.Text = auc.lot.ToString();
                    auctionTable.Cell(2 + aucId, 2).Range.Text = auc.name;
                    auctionTable.Cell(2 + aucId, 3).Range.Text = auc.hammerPrice.ToString("n0");
                    auctionTable.Cell(2 + aucId, 4).Range.Text = auc.serviceCharge.ToString("n0");
                    auctionTable.Cell(2 + aucId, 5).Range.Text = auc.total.ToString("n0");
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
                auctionTable.Cell(aucCount + 4, 2).Range.Text = "NTD " + creditCardFee.ToString("n0");
                //auctionTable.Cell(6, 2).Range.Text = "NTD " + tax.ToString("n0");
                //auctionTable.Cell(7, 2).Range.Text = "NTD " + amountDue.ToString("n0");
                auctionTable.Cell(aucCount + 5, 2).Range.Text = "NTD " + amountDue.ToString("n0");
            }
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

        private bool ParseAndSetBidder(string receive)
        {
            string[] str = receive.Split('\t');
            if (str.Length < 10)    //買家欄位:7+一個拍品欄位:3
                return false;

            m_bidder = new Bidder();
            m_bidder.name = str[1];   //ignore first string of id.
            int no = 0;
            if (!int.TryParse(str[2], out no))
                return false;
            m_bidder.no = no;
            m_bidder.phone = str[3];
            m_bidder.fax = str[4];
            m_bidder.email = str[5];
            m_bidder.addr = str[6];
            m_bidder.auctions = new Dictionary<string, Auction>();
            for (int i = 7; i < str.Length; i += 3)
            {
                if (i + 2 >= str.Length)
                    break;
                Auction auction = new Auction();
                auction.lot = str[i];
                auction.name = str[i + 1];
                int.TryParse(str[i + 2], out auction.hammerPrice);
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
            string fp = Path.Combine(Application.StartupPath, m_settingsFN);
            using (StreamWriter sw = new StreamWriter(fp))
            {
                sw.WriteLine(m_client.IP + " " + m_serverPort);
            }
        }

        private void SavePaymentDoc()
        {
            SetDataInDoc();

            if (isPrintOneByOneCheckBox.Checked)
            {
                string folder = Path.Combine(Application.StartupPath, m_bidder.no.ToString());
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
                if (!m_totalPaymentDoc.Saved)
                    m_totalPaymentDoc.SaveAs(Path.Combine(Application.StartupPath, m_totalPaymentDocName));
            }
        }

        private void CloseDocAndWord()
        {
            if (m_bidder == null)
                return;
            object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            foreach (Auction auc in m_bidder.auctions.Values)
            {
                if (auc.paymentDoc != null)
                {
                    auc.paymentDoc.Close(ref saveOption, ref m_oMissing, ref m_oMissing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(auc.paymentDoc);
                    auc.paymentDoc = null;
                }
            }

            if (m_totalPaymentDoc != null)
            {
                m_totalPaymentDoc.Close(ref saveOption, ref m_oMissing, ref m_oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_totalPaymentDoc);
                m_totalPaymentDoc = null;
                m_totalPaymentDocName = "";
            }

            if (m_wordApp != null)
            {
                if (m_wordApp.NormalTemplate != null)
                    m_wordApp.NormalTemplate.Saved = true;
                m_wordApp.Quit(ref saveOption, ref m_oMissing, ref m_oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_wordApp);
                m_wordApp = null;
            }
        }
        #endregion
    }
}
