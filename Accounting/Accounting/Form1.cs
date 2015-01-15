//#define MUCHUNTANG
//#define SHIJIA

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
using System.Linq;

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
        private Internet<DealerEntity> m_dealerInternet;
        private Internet<MemberEntity> m_memberInternet;
        private Internet<BidderEntity> m_beInternet;
        private bool m_isSuperUser = false;
        private _Application m_excelApp = null;
        private int m_unit = 10000;
        private int m_lastUnit = 1;
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
#if SHIJIA
            this.unitComboBox.SelectedIndex = 2;
#endif
#if MUCHUNTANG
            this.unitComboBox.SelectedIndex = 0;
            m_unit = 1;
#endif

            ConnectToServer();
            // LoadExcelTemplate();
            if (m_auctionsInternet != null && m_auctionsInternet.IsConnected)
            {
                LoadCollectionToDataGridView();
            }
            this.WindowState = FormWindowState.Maximized;
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

                        float hammerPrice = Utility.ParseToFloat(hpCell.Value.ToString(), true);
                        int newBuyerServiceCharge = Utility.ParseToInt(bscCell.Value.ToString(), false);
                        float finalPrice = GetFinalPrice(hammerPrice, newBuyerServiceCharge);
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.BuyerServiceCharge, newBuyerServiceCharge);
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.FinalPrice, (int)(finalPrice * m_unit));
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
                        int guaranteeCost = (int)(Utility.ParseToFloat(cell.Value.ToString(), false) * m_unit);
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
                        int returnGuaranteeNumber = (int)(Utility.ParseToFloat(cell.Value.ToString(), false) * m_unit);
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

        private void unitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_lastUnit = m_unit;
            switch (this.unitComboBox.SelectedIndex)
            {
                case 0:
                    m_unit = 1;
                    break;
                case 1:
                    m_unit = 1000;
                    break;
                case 2:
                    m_unit = 10000;
                    break;
                default:
                    break;
            }
            RefreshDataAfterUnitChanged();
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

            if ("Ma Lei" == account)
                m_isSuperUser = true;
        }

        private void ConnectToServer()
        {
            string ip = Utility.InputIp();
            if (ip.Length == 0)
                return;
            m_auctionsInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_dealerItemInternet = new Internet<DealerItemEntity>(ip, "bidding_data", "dealer_item_table");
            m_dealerInternet = new Internet<DealerEntity>(ip, "bidding_data", "dealer_table");
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

#if SHIJIA
            Login();
#endif
#if MUCHUNTANG
            m_isSuperUser = true;
#endif
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
            this.dataGridView1.ReadOnly = false;

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
            Dictionary<string, DealerEntity> dealerEntities = null;
            try
            {
                auctionEntities = m_auctionsInternet.GetCollectionList();
                dealerItemEntities = m_dealerItemInternet.GetCollectionList();
                dealerEntities = m_dealerInternet.GetCollectionList().ToDictionary(x => x.Name, x => x);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (auctionEntities != null)
            {
                StringBuilder sb = new StringBuilder();
                int newLineCount = 8;
                int count = 0;
                foreach (AuctionEntity auction in auctionEntities)
                {
                    DealerItemEntity dealerItem = m_dealerItemInternet.FineOne((di => di.LotNO), auction.AuctionId);
                    DealerEntity dealer = new DealerEntity();
                    int bidderNum = Utility.ParseToInt(auction.BidderNumber, true);
                    BidderEntity bidder = m_beInternet.FineOne<int>(be => be.BidderID_int, bidderNum);
                    if (dealerItem == null)
                    {
                        sb.Append(auction.AuctionId + " ");
                        dealerItem = new DealerItemEntity();
                        count++;
                        if (count % newLineCount == 0)
                            sb.Append("\n");
                    }
                    else
                    {
                        if (dealerEntities.ContainsKey(dealerItem.SrcDealer))
                            dealer = dealerEntities[dealerItem.SrcDealer];
                        else
                            MessageBox.Show("賣家設定中找不到此賣家:\n\n" + dealerItem.SrcDealer);
                    }
                    if (bidder == null)
                    {
                        bidder = new BidderEntity();
                    }

                    AuctionEntity auc = auction;
                    AddDataRowToDataGridView(ref auc, ref bidder, ref dealerItem, ref dealer);
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
                    DealerEntity dealer = new DealerEntity();
                    if (dealerItem != null && dealerEntities.ContainsKey(dealerItem.SrcDealer))
                        dealer = dealerEntities[dealerItem.SrcDealer];
                    AddDataRowToDataGridView(ref auction, ref bidder, ref dItem, ref dealer);
                }
            }

            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void AddDataRowToDataGridView(ref AuctionEntity auction, ref BidderEntity bidder, ref DealerItemEntity dealerItem,
                                              ref DealerEntity dealer)
        {
            float hammerPrice = auction.HammerPrice / (float)m_unit;
            int buyerServiceCharge = auction.BuyerServiceCharge;
            float finalPrice = auction.FinalPrice / (float)m_unit;
            float guaranteeCost = int.Parse(bidder.GuaranteeCost) / (float)m_unit;
            float returnGuaranteeNumber = auction.ReturnGuaranteeNumber / (float)m_unit;
            float reservePrice = 0;//int.Parse(dealerItem.ReservePrice);
            reservePrice = float.TryParse(dealerItem.ReservePrice, out reservePrice) ? reservePrice / m_unit : 0;
            //float sellerAccountPayable = auction.SellerAccountPayable / m_unit;
            int ifDealedInsuranceFee = (dealer == null) ? 0 : Utility.ParseToInt(dealer.IfDealedInsuranceFee, true);
            int ifDealedServiceFee = (dealer == null) ? 0 : Utility.ParseToInt(dealer.IfDealedServiceFee, true);
            int insuranceNService = ifDealedInsuranceFee + ifDealedServiceFee;
            insuranceNService = insuranceNService > 0 ? insuranceNService : 0;
            float sellerAccountPayable = hammerPrice * (100 - insuranceNService) * 0.01f;

            if (m_isSuperUser)
            {
                dataGridView1.Rows.Add(dealerItem._id, auction.AuctionId, auction.Artwork, dealerItem.Spec, auction.BidderNumber, bidder.Name,
                    dealerItem.Remain, dealerItem.SrcDealer, Utility.GetEnumString(typeof(ReturnState), auction.ReturnState),
                    hammerPrice, buyerServiceCharge, finalPrice, bidder.GuaranteeType, guaranteeCost,
                    Utility.GetEnumString(typeof(ReturnGuarantee), auction.ReturnGuaranteeState), returnGuaranteeNumber,
                    Utility.GetEnumString(typeof(PayGuarantee), auction.PayWayState),
                    insuranceNService, reservePrice, sellerAccountPayable);

                if (auction.HammerPrice < Utility.ParseToFloat(dealerItem.ReservePrice, true) &&
                    auction.BidderNumber != "" && auction.HammerPrice != 0)
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
                    if (!dgv.Columns[j].Visible)
                        continue;
                    object value = dgv.Rows[i].Cells[j].Value;
                    sheet.Cells[i + 2, j + 1] = (value == null) ? "" : value.ToString();
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

        private float GetFinalPrice(float hammerPrice, int newBuyerServiceCharge)
        {
            float finalPrice = (float)(hammerPrice * (1.0f + newBuyerServiceCharge * 0.01d));
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

        private void RefreshDataAfterUnitChanged()
        { 
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                for (int j = 0; j < this.dataGridView1.Rows.Count; j++)
                {
                    string colName = this.dataGridView1.Columns[i].Name;
                    switch (colName)
                    {
                        case "落槌價":
                        case "成交價":
                        case "保證金繳納金額":
                        case "保證金退還金額":
                        case "保留價":
                        case "應付賣家金額":
                            {
                                DataGridViewCell cell = this.dataGridView1.Rows[j].Cells[i] as DataGridViewCell;
                                float oldValue = Utility.ParseToFloat(cell.Value.ToString(), false) * m_lastUnit;
                                cell.Value = (object)(oldValue / m_unit);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
