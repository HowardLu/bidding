using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Bidding;
using InternetLibrary;
using Microsoft.Office.Interop.Excel;
using UtilityLibrary;

namespace Accounting
{
    public partial class Form1 : Form
    {
        #region Enum, Internal Class
        #endregion

        #region Events
        #endregion

        #region Member Variables
        //private String m_connectionStr = "mongodb://1.34.233.143:27017";
        private Internet<AuctionEntity> m_auctionsInternet;
        private Internet<DealerItemEntity> m_dealerItemInternet;
        private Internet<MemberEntity> m_memberInternet;
        private Internet<BidderEntity> m_beInternet;
        private bool m_isSuperUser = false;
        private _Application m_excelApp = null;
        private int m_unit = 10000;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Windows Form Events
        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectToServer();
            // LoadExcelTemplate();
            if (m_auctionsInternet != null && m_auctionsInternet.IsConnected)
            {
                LoadCollectionToDataGridView();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xls)|*.xls";
            sfd.FileName = "account.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataGridViewToXls(sfd.FileName); // Here dataGridview1 is your grid view name 
                MessageBox.Show(sfd.FileName + "已儲存!");
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ip = Utility.InputIp();
            if (ip.Length == 0)
                return;

            if (m_auctionsInternet.Connect())
            {
                toolStripStatusLabel1.Text = "連線成功!";
            }
            else
            {
                toolStripStatusLabel1.Text = "連線失敗!";
            }

            if (m_auctionsInternet.IsConnected)
            {
                LoadCollectionToDataGridView();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("dataGridView1_CellValueChanged " + e.RowIndex.ToString() + " " + e.ColumnIndex.ToString());
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            string auctionId = this.dataGridView1.Rows[e.RowIndex].Cells[AuctionColumnHeader.拍品編號.ToString()].Value.ToString();
            string bidderId = this.dataGridView1.Rows[e.RowIndex].Cells[AuctionColumnHeader.得標牌號.ToString()].Value.ToString();
            string colName = this.dataGridView1.Columns[e.ColumnIndex].Name;
            switch (colName)
            {
                case "庫存狀態":
                    m_auctionsInternet.UpdateField<string, string>(ae => ae.AuctionId, auctionId, ae => ae.StockState, this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    break;
                case "歸還狀態":
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.ReturnState, Utility.ToEnumInt<ReturnState>(cell.Value.ToString()));
                    }
                    break;
                case "買家適用服務費":
                    {
                        DataGridViewCell hpCell = this.dataGridView1.Rows[e.RowIndex].Cells[AuctionColumnHeader.落槌價.ToString()] as DataGridViewCell;
                        DataGridViewCell bscCell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCell;
                        DataGridViewCell fpCell = this.dataGridView1.Rows[e.RowIndex].Cells[AuctionColumnHeader.成交價.ToString()] as DataGridViewCell;

                        int hammerPrice = Utility.ParseToInt(hpCell.Value.ToString(), true);
                        int newBuyerServiceCharge = Utility.ParseToInt(bscCell.Value.ToString(), false);
                        int finalPrice = GetFinalPrice(hammerPrice, newBuyerServiceCharge);
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.BuyerServiceCharge, newBuyerServiceCharge);
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.FinalPrice, finalPrice * m_unit);
                        fpCell.Value = finalPrice;
                    }
                    break;
                case "保證金繳納":
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_beInternet.UpdateField<string, string>(be => be.BidderID, bidderId, be => be.GuaranteeType, cell.Value.ToString());
                    }
                    break;
                case "保證金繳納金額":
                    {
                        DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCell;
                        int guaranteeCost = Utility.ParseToInt(cell.Value.ToString(), false) * m_unit;
                        m_beInternet.UpdateField<string, string>(be => be.BidderID, bidderId, be => be.GuaranteeCost, guaranteeCost.ToString());
                    }
                    break;
                case "保證金退還":
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.ReturnGuaranteeState, Utility.ToEnumInt<ReturnGuarantee>(cell.Value.ToString()));
                    }
                    break;
                case "保證金退還金額":
                    {
                        DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCell;
                        int returnGuaranteeNumber = Utility.ParseToInt(cell.Value.ToString(), false) * m_unit;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.ReturnGuaranteeNumber, returnGuaranteeNumber);
                    }
                    break;
                case "付款方式":
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.PayWayState, Utility.ToEnumInt<PayWay>(cell.Value.ToString()));
                    }
                    break;
                default:
                    break;
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            Console.WriteLine("dataGridView1_CellValidating " + e.RowIndex.ToString() + " " + e.ColumnIndex.ToString() + " " + e.FormattedValue.ToString());
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("dataGridView1_CellValidated " + e.RowIndex.ToString() + " " + e.ColumnIndex.ToString());
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //MessageBox.Show("Error happened " + e.Context.ToString());

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change");
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("parsing error");
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((e.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string colName = this.checkedListBox1.GetItemText(this.checkedListBox1.Items[e.Index]);
            this.dataGridView1.Columns[colName].Visible = (e.NewValue == CheckState.Checked) ? true : false;
        }

        private void checkAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox cb = sender as System.Windows.Forms.CheckBox;
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemChecked(i, cb.Checked);
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        /*private void LoadExcelTemplate()
        {
            String sheetName = "Sheet1";
            String fileName = @"../AccountingTemplate.xls";
            String conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileName +
                            ";Extended Properties='Excel 8.0;HDR=YES;';";

            OleDbConnection conn = new OleDbConnection(conStr);
            OleDbCommand comm = new OleDbCommand("Select * From [" + sheetName + "$]", conn);
            conn.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(comm);
            System.Data.DataTable data = new System.Data.DataTable();
            da.Fill(data);
            this.dataGridView1.DataSource = data;

            data.Dispose();
            da.Dispose();
            comm.Dispose();
            conn.Close();
            conn.Dispose();

            for(int i=0; i < this.dataGridView1.Columns.Count; i++)
            {
                DataGridViewColumn col = this.dataGridView1.Columns[i];
                if (col.Name == "歸還狀態")
                {
                    SetComboBoxCol(i, typeof(ReturnState));
                }
                else if (col.Name == "保證金繳納")
                {
                    SetComboBoxCol(i, typeof(PayGuarantee));
                }
                else if (col.Name == "保證金退還")
                {
                    SetComboBoxCol(i, typeof(ReturnGuarantee));
                }
                else if (col.Name == "付款方式")
                {
                    SetComboBoxCol(i, typeof(PayWay));
                }
            }
        }*/

        private void Login()
        {
            string account = Utility.InputBox("", "請輸入帳號:", "", -1, -1);
            if (account.Length == 0)
                return;

            MemberEntity me = m_memberInternet.FineOne<string>(m => m.Account, account);
            if (me != null)
                m_isSuperUser = true;
        }

        private void ConnectToServer()
        {
            string ip = Utility.InputIp();
            if (ip.Length == 0)
                return;
            m_auctionsInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_dealerItemInternet = new Internet<DealerItemEntity>(ip, "bidding_data", "dealer_item_table");
            m_memberInternet = new Internet<MemberEntity>(ip, "bidding_data", "member_table");
            m_beInternet = new Internet<BidderEntity>(ip, "bidding_data", "buyer_table");

            if (m_auctionsInternet.Connect())
            {
                toolStripStatusLabel1.Text = "連線成功!";
            }
            else
            {
                toolStripStatusLabel1.Text = "連線失敗!";
            }

            Login();
        }

        private void AddCol(string headerText, Color color, bool readOnly)
        {
            DataGridViewColumn col = new DataGridViewColumn();
            col.Name = col.HeaderText = headerText;
            col.DefaultCellStyle.BackColor = color;
            col.ReadOnly = readOnly;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            this.dataGridView1.Columns.Add(col);
        }

        private void AddComboBoxCol(Type enumType, string headerText, Color color)
        {
            DataGridViewComboBoxColumn cbcol = new DataGridViewComboBoxColumn();
            cbcol.Name = cbcol.HeaderText = headerText;
            cbcol.DefaultCellStyle.BackColor = color;
            foreach (string rs in Enum.GetNames(enumType))
            {
                cbcol.Items.Add(rs);
            }
            cbcol.SortMode = DataGridViewColumnSortMode.Automatic;
            cbcol.ReadOnly = false;
            this.dataGridView1.Columns.Add(cbcol);
        }

        private void LoadCollectionToDataGridView()
        {
            List<AuctionEntity> auctions = m_auctionsInternet.GetCollectionList();
            //if (auctions.Count == 0)
            //{
            //    AuctionEntity auction = new AuctionEntity();
            //    auction.AuctionId = "111";
            //    auction.Artwork = "國寶";
            //    auction.BidderNumber = "100";
            //    auction.StockState = "home";
            //    auction.ReturnState = 1;
            //    auction.ReturnGuaranteeState = 2;
            //    auction.PayWayState = 3;
            //    m_auctionsInternet.Insert(auction);
            //}

            dataGridView1.ReadOnly = false;

            int columnCount = (int)AuctionColumnHeader.Count;
            for (int i = 0; i < columnCount; i++)
            {
                string headerText = Enum.GetName(typeof(AuctionColumnHeader), i);

                if (headerText == AuctionColumnHeader.歸還狀態.ToString())
                {
                    AddComboBoxCol(typeof(ReturnState), headerText, Color.LightCyan);
                }
                else if (headerText == AuctionColumnHeader.保證金繳納.ToString())
                {
                    AddComboBoxCol(typeof(PayGuarantee), headerText, Color.LightCyan);
                }
                else if (headerText == AuctionColumnHeader.保證金退還.ToString())
                {
                    AddComboBoxCol(typeof(ReturnGuarantee), headerText, Color.LightCyan);
                }
                else if (headerText == AuctionColumnHeader.付款方式.ToString())
                {
                    AddComboBoxCol(typeof(PayWay), headerText, Color.LightCyan);
                }
                else
                {
                    if (headerText == AuctionColumnHeader.庫存狀態.ToString() || headerText == AuctionColumnHeader.買家適用服務費.ToString() ||
                        headerText == AuctionColumnHeader.保證金繳納金額.ToString() || headerText == AuctionColumnHeader.保證金退還金額.ToString())
                    {
                        AddCol(headerText, Color.LightCyan, false);
                    }
                    else
                    {
                        if (!m_isSuperUser)
                        {
                            if (headerText == AuctionColumnHeader.賣家.ToString() ||
                               headerText == AuctionColumnHeader.賣家服務及保險費.ToString() ||
                               headerText == AuctionColumnHeader.保留價.ToString() ||
                               headerText == AuctionColumnHeader.應付賣家金額.ToString())
                            {
                                continue;
                            }
                        }

                        AddCol(headerText, Color.White, true);
                    }
                }
            }
            AddColumnsToCheckedListBox();

            List<AuctionEntity> auctionEntities = null;
            List<DealerItemEntity> dealerItemEntities = null;
            try
            {
                auctionEntities = m_auctionsInternet.GetCollectionList();
                dealerItemEntities = m_dealerItemInternet.GetCollectionList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (auctionEntities != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (AuctionEntity auction in auctionEntities)
                {
                    DealerItemEntity dealerItem = m_dealerItemInternet.FineOne((di => di.LotNO), auction.AuctionId);
                    BidderEntity bidder = m_beInternet.FineOne<string>(be => be.BidderID, auction.BidderNumber);
                    if (dealerItem == null)
                    {
                        sb.AppendLine(auction.AuctionId);
                        dealerItem = new DealerItemEntity();
                    }
                    if (bidder == null)
                    {
                        bidder = new BidderEntity();
                    }

                    AuctionEntity auc = auction;
                    AddDataRowToDataGridView(ref auc, ref bidder, ref dealerItem);
                }

                if (sb.Length > 0)
                    MessageBox.Show("找不到以下拍品在[賣家建檔資料表]:\n\n" + sb.ToString());
            }

            if (dealerItemEntities != null)
            {
                foreach (DealerItemEntity dealerItem in dealerItemEntities)
                {
                    if (dealerItem.LotNO != "")
                        continue;

                    AuctionEntity auction = new AuctionEntity();
                    BidderEntity bidder = new BidderEntity();
                    DealerItemEntity dItem = dealerItem;
                    AddDataRowToDataGridView(ref auction, ref bidder, ref dItem);
                }
            }

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        private void AddDataRowToDataGridView(ref AuctionEntity auction, ref BidderEntity bidder, ref DealerItemEntity dealerItem)
        {
            int hammerPrice = auction.HammerPrice / m_unit;
            int buyerServiceCharge = auction.BuyerServiceCharge / m_unit;
            int finalPrice = auction.FinalPrice / m_unit;
            int guaranteeCost = int.Parse(bidder.GuaranteeCost) / m_unit;
            int returnGuaranteeNumber = auction.ReturnGuaranteeNumber / m_unit;
            int reservePrice = 0;//int.Parse(dealerItem.ReservePrice);
            reservePrice = int.TryParse(dealerItem.ReservePrice, out reservePrice) ? reservePrice / m_unit : 0;
            int sellerAccountPayable = auction.SellerAccountPayable / m_unit;

            if (m_isSuperUser)
            {
                dataGridView1.Rows.Add(dealerItem._id, auction.AuctionId, auction.Artwork, dealerItem.Spec, auction.BidderNumber, bidder.Name,
                    dealerItem.Remain, dealerItem.SrcDealer, Utility.GetEnumString(typeof(ReturnState), auction.ReturnState),
                    hammerPrice, buyerServiceCharge, finalPrice, bidder.GuaranteeType, guaranteeCost,
                    Utility.GetEnumString(typeof(ReturnGuarantee), auction.ReturnGuaranteeState), returnGuaranteeNumber,
                    Utility.GetEnumString(typeof(PayGuarantee), auction.PayWayState), auction.SellerServiceCharge,
                    reservePrice, sellerAccountPayable);

                if (auction.HammerPrice < Utility.ParseToInt(dealerItem.ReservePrice, true))
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].ErrorText = "落槌價低於保留價!!!";
                }
            }
            else
            {
                dataGridView1.Rows.Add(dealerItem._id, auction.AuctionId, auction.Artwork, dealerItem.Spec, auction.BidderNumber, bidder.Name,
                    dealerItem.Remain, Utility.GetEnumString(typeof(ReturnState), auction.ReturnState), hammerPrice,
                    buyerServiceCharge, finalPrice, bidder.GuaranteeType, guaranteeCost,
                    Utility.GetEnumString(typeof(ReturnGuarantee), auction.ReturnGuaranteeState), returnGuaranteeNumber,
                    Utility.GetEnumString(typeof(PayGuarantee), auction.PayWayState));
            }
        }

        private void DataGridViewToCsv(string filename)
        {
            DataGridView dgv = dataGridView1;
            string stOutput = "";
            // Export titles:
            string sHeaders = "";

            for (int j = 0; j < dgv.Columns.Count; j++)
                sHeaders = sHeaders.ToString() + Convert.ToString(dgv.Columns[j].HeaderText) + "\t";
            stOutput += sHeaders + "\r\n";
            // Export data.
            for (int i = 0; i < dgv.RowCount - 1; i++)
            {
                string stLine = "";
                for (int j = 0; j < dgv.Rows[i].Cells.Count; j++)
                    stLine = stLine.ToString() + Convert.ToString(dgv.Rows[i].Cells[j].Value) + "\t";
                stOutput += stLine + "\r\n";
            }
            //Encoding utf16 = Encoding.GetEncoding(1254);
            Encoding utf16 = Encoding.GetEncoding("big5");
            byte[] output = utf16.GetBytes(stOutput);
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(output, 0, output.Length); //write the encoded file
            bw.Flush();
            bw.Close();
            fs.Close();
        }

        private DataGridView CopyDataGridView(DataGridView dgv_org)
        {
            DataGridView dgv_copy = new DataGridView();
            /*try
            {*/
            if (dgv_copy.Columns.Count == 0)
            {
                foreach (DataGridViewColumn dgvc in dgv_org.Columns)
                {
                    DataGridViewColumn dgvcTemp = new DataGridViewColumn();
                    dgvcTemp = dgvc.Clone() as DataGridViewColumn;
                    dgvcTemp.CellTemplate = dgvc.CellTemplate;
                    dgv_copy.Columns.Add(dgvcTemp);
                }
            }

            DataGridViewRow row = new DataGridViewRow();

            for (int i = 0; i < dgv_org.Rows.Count; i++)
            {
                row = (DataGridViewRow)dgv_org.Rows[i].Clone();
                int intColIndex = 0;
                foreach (DataGridViewCell cell in dgv_org.Rows[i].Cells)
                {
                    row.Cells[intColIndex].Value = cell.Value;
                    intColIndex++;
                }
                dgv_copy.Rows.Add(row);
            }
            dgv_copy.AllowUserToAddRows = false;
            dgv_copy.Refresh();

            /*}
            catch (Exception ex)
            {
                MessageBox.Show("Copy DataGridViw " + ex.ToString());
            }*/
            return dgv_copy;
        }

        private void DataGridViewToXls(string filename)
        {
            if (m_excelApp == null)
                m_excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = m_excelApp.Workbooks.Add();
            Worksheet sheet = workbook.Worksheets.get_Item(1);
            DataGridView dgv = CopyDataGridView(this.dataGridView1);

            for (int i = dgv.ColumnCount - 1; i > -1; i--)
            {
                if (!dgv.Columns[i].Visible)
                {
                    dgv.Columns.RemoveAt(i);
                }
            }

            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (!dgv.Columns[i].Visible)
                    continue;
                sheet.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            }

            sheet.get_Range("A1").EntireRow.Font.Bold = true;
            //sheet.get_Range("A1").EntireRow.Interior.Color = System.Drawing.ColorTranslator.ToWin32(Color.LightCyan);

            for (int i = 0; i < dgv.RowCount; i++)
            {
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    if (!dgv.Columns[i].Visible)
                        continue;
                    object value = dgv.Rows[i].Cells[j].Value;
                    sheet.Cells[i + 2, j + 1] = value == null ? "" : value.ToString();
                }
            }

            sheet.get_Range("A1", "T100").EntireColumn.AutoFit();
            sheet.get_Range("A1", "T100").EntireRow.AutoFit();
            workbook.SaveAs(filename, XlFileFormat.xlWorkbookNormal/*, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing*/);
            workbook.Close(true/*, misValue, misValue*/);
            m_excelApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_excelApp);
            dgv = null;
            sheet = null;
            workbook = null;
            m_excelApp = null;
        }

        private int GetFinalPrice(int hammerPrice, int newBuyerServiceCharge)
        {
            int finalPrice = (int)(hammerPrice * (1.0f + newBuyerServiceCharge * 0.01d));
            return finalPrice;
        }

        private void AddColumnsToCheckedListBox()
        {
            DataGridViewColumnCollection cols = this.dataGridView1.Columns;
            for (int i = 0; i < cols.Count; i++)
            {
                this.checkedListBox1.Items.Add(cols[i].Name, true);
            }
        }
        #endregion
    }
}
