using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Data.OleDb;
using InternetLibrary;
using UtilityLibrary;
using Bidding;

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
        private bool m_isLogined = false;
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
            if (m_auctionsInternet.IsConnected)
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
                DataGridViewToCsv(dataGridView1, sfd.FileName); // Here dataGridview1 is your grid view name 
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ip = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入Server IP", m_auctionsInternet.IP, -1, -1);
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

            string auctionId = this.dataGridView1.Rows[e.RowIndex].Cells[(int)AuctionColumnHeader.拍品編號].Value.ToString();
            switch ((AuctionColumnHeader)e.ColumnIndex)
            {
                case AuctionColumnHeader.庫存狀態:
                    m_auctionsInternet.UpdateField<string, string>(ae => ae.AuctionId, auctionId, ae => ae.StockState, this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    break;
                case AuctionColumnHeader.歸還狀態:
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.ReturnState, Utility.ToEnumInt<ReturnState>(cell.Value.ToString()));
                    }
                    break;
                case AuctionColumnHeader.買家適用服務費:
                    {
                        DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.BuyerServiceCharge, Utility.ParseToInt(cell.Value.ToString()));
                    }
                    break;
                case AuctionColumnHeader.保證金繳納:
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.PayGuaranteeState, Utility.ToEnumInt<PayGuarantee>(cell.Value.ToString()));
                    }
                    break;
                case AuctionColumnHeader.保證金繳納金額:
                    {
                        DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.PayGuaranteeNumber, Utility.ParseToInt(cell.Value.ToString()));
                    }
                    break;
                case AuctionColumnHeader.保證金退還:
                    {
                        DataGridViewComboBoxCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.ReturnGuaranteeState, Utility.ToEnumInt<ReturnGuarantee>(cell.Value.ToString()));
                    }
                    break;
                case AuctionColumnHeader.保證金退還金額:
                    {
                        DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCell;
                        m_auctionsInternet.UpdateField<string, int>(ae => ae.AuctionId, auctionId, ae => ae.ReturnGuaranteeNumber, Utility.ParseToInt(cell.Value.ToString()));
                    }
                    break;
                case AuctionColumnHeader.付款方式:
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
            string account = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入帳號:", "", -1, -1);
            if (account.Length == 0)
                return;

            MemberEntity me = m_memberInternet.FineOne<string>(m => m.Account, account);
            if (me != null)
                m_isLogined = true;
        }

        private void ConnectToServer()
        {
            string ip = Microsoft.VisualBasic.Interaction.InputBox("", "請輸入Server IP:", "127.0.0.1", -1, -1);
            if (ip.Length == 0)
                return;
            m_auctionsInternet = new Internet<AuctionEntity>(ip, "bidding_data", "auctions_table");
            m_dealerItemInternet = new Internet<DealerItemEntity>(ip, "bidding_data", "dealer_item_table");
            m_memberInternet = new Internet<MemberEntity>(ip, "bidding_data", "member_table");
            Login();

            if (m_auctionsInternet.Connect())
            {
                toolStripStatusLabel1.Text = "連線成功!";
            }
            else
            {
                toolStripStatusLabel1.Text = "連線失敗!";
            }
        }

        private void AddCol(string headerText, Color color, bool readOnly)
        {
            DataGridViewColumn col = new DataGridViewColumn();
            col.Name = "";
            col.HeaderText = headerText;
            col.DefaultCellStyle.BackColor = color;
            col.ReadOnly = readOnly;
            col.CellTemplate = new DataGridViewTextBoxCell();
            this.dataGridView1.Columns.Add(col);
        }

        private void AddComboBoxCol(Type enumType, string headerText, Color color)
        {
            DataGridViewComboBoxColumn cbcol = new DataGridViewComboBoxColumn();
            cbcol.Name = "";
            cbcol.HeaderText = headerText;
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
            if (auctions.Count == 0)
            {
                AuctionEntity auction = new AuctionEntity();
                auction.AuctionId = "111";
                auction.Name = "國寶";
                auction.BidderNumber = "100";
                auction.StockState = "home";
                auction.ReturnState = 1;
                auction.PayGuaranteeState = 2;
                auction.ReturnGuaranteeState = 2;
                auction.PayWayState = 3;
                m_auctionsInternet.Insert(auction);
            }

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
                    if (headerText == AuctionColumnHeader.庫存狀態.ToString() ||
                        headerText == AuctionColumnHeader.保證金繳納金額.ToString() ||
                        headerText == AuctionColumnHeader.保證金退還金額.ToString())
                    {
                        AddCol(headerText, Color.LightCyan, false);
                    }
                    else
                    {
                        if (!m_isLogined)
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

            List<AuctionEntity> auctionEntities = null;
            try
            {
                auctionEntities = m_auctionsInternet.GetCollectionList();
            }
            catch (MongoDB.Driver.MongoException e)
            {
                MessageBox.Show(e.ToString());
            }

            if (auctionEntities == null)
                return;
            foreach (AuctionEntity auction in auctionEntities)
            {
                DealerItemEntity dealerItem = m_dealerItemInternet.FineOne((di => di.LotNO), auction.AuctionId);
                if (dealerItem == null)
                {
                    MessageBox.Show("找不到此拍品在dealer_item_table");
                    continue;
                }
                if (m_isLogined)
                {
                    dataGridView1.Rows.Add(auction.AuctionId, auction.Name, dealerItem.Spec, auction.BidderNumber, dealerItem.Remain,
                        dealerItem.SrcDealer, Utility.GetEnumString(typeof(ReturnState), auction.ReturnState), auction.HammerPrice,
                        auction.BuyerServiceCharge, auction.FinalPrice,
                        Utility.GetEnumString(typeof(PayGuarantee), auction.PayGuaranteeState), auction.PayGuaranteeNumber,
                        Utility.GetEnumString(typeof(ReturnGuarantee), auction.ReturnGuaranteeState), auction.ReturnGuaranteeNumber,
                        Utility.GetEnumString(typeof(PayGuarantee), auction.PayWayState), auction.SellerServiceCharge,
                        dealerItem.ReservePrice, auction.SellerAccountPayable);
                }
                else
                {
                    dataGridView1.Rows.Add(auction.AuctionId, auction.Name, dealerItem.Spec, auction.BidderNumber, dealerItem.Remain,
                        Utility.GetEnumString(typeof(ReturnState), auction.ReturnState), auction.HammerPrice,
                        auction.BuyerServiceCharge, auction.FinalPrice,
                        Utility.GetEnumString(typeof(PayGuarantee), auction.PayGuaranteeState), auction.PayGuaranteeNumber,
                        Utility.GetEnumString(typeof(ReturnGuarantee), auction.ReturnGuaranteeState), auction.ReturnGuaranteeNumber,
                        Utility.GetEnumString(typeof(PayGuarantee), auction.PayWayState));
                }
            }

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void DataGridViewToCsv(DataGridView dgv, string filename)
        {
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
            Encoding utf16 = Encoding.GetEncoding(1254);
            byte[] output = utf16.GetBytes(stOutput);
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(output, 0, output.Length); //write the encoded file
            bw.Flush();
            bw.Close();
            fs.Close();
        }
        #endregion
    }
}
