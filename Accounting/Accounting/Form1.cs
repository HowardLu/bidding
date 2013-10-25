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
using Bidding;

namespace Accounting
{
    public partial class Form1 : Form
    {
        #region Properties
        //private String m_connectionStr = "mongodb://1.34.233.143:27017";
        private Internet<AuctionEntity> m_internet = new Internet<AuctionEntity>("mongodb://localhost:27017", "test", "entities");
        //private Internet<AuctionEntityTW> m_internet = new Internet<AuctionEntityTW>("mongodb://localhost:27017", "bidding_data", "auction_table");
        #endregion

        #region Constructors
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region windows form event handler
        private void Form1_Load(object sender, EventArgs e)
        {
            // LoadExcelTemplate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xls)|*.xls";
            sfd.FileName = "account.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ToCsV(dataGridView1, sfd.FileName); // Here dataGridview1 is your grid view name 
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_internet.Connect();
            //m_internet.Update();
            LoadCollectionToDataGridView();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {

        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {

        }
        #endregion

        #region Private Methods
        private void LoadExcelTemplate()
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
        }

        private void SetComboBoxCol(int index, Type enumType)
        {
            DataGridViewColumn col = this.dataGridView1.Columns[index];
            DataGridViewComboBoxColumn cbcol = new DataGridViewComboBoxColumn();
            cbcol.Name = col.Name;
            cbcol.HeaderText = col.HeaderText;
            foreach (string rs in Enum.GetNames(enumType))
            {
                cbcol.Items.Add(rs);
            }
            this.dataGridView1.Columns.RemoveAt(index);
            this.dataGridView1.Columns.Insert(index, cbcol);
        }

        private void LoadCollectionToDataGridView()
        {
            dataGridView1.ReadOnly = false;

            dataGridView1.ColumnCount = (int)AuctionColumnHeader.Count;
            for (int i = 0; i < (int)AuctionColumnHeader.Count; i++)
            {
                DataGridViewColumn col = dataGridView1.Columns[i];
                col.HeaderText = Enum.GetName(typeof(AuctionColumnHeader), i);
                if (col.HeaderText == "歸還狀態")
                {
                    SetComboBoxCol(i, typeof(ReturnState));
                }
                else if (col.HeaderText == "保證金繳納")
                {
                    SetComboBoxCol(i, typeof(PayGuarantee));
                }
                else if (col.HeaderText == "保證金退還")
                {
                    SetComboBoxCol(i, typeof(ReturnGuarantee));
                }
                else if (col.HeaderText == "付款方式")
                {
                    SetComboBoxCol(i, typeof(PayWay));
                }
            }

            List<AuctionEntity> auctionEntities = m_internet.Collection.FindAll().ToList<AuctionEntity>();
            foreach (AuctionEntity auction in auctionEntities)
            {
                dataGridView1.Rows.Add(new string[] { auction.AuctionId, auction.Name, auction.Size, auction.BidderNumber, auction.StockState,
                                                      auction.Seller, auction.ReturnState.ToString(), auction.HammerPrice.ToString(),
                                                      auction.BuyerServiceCharge.ToString(), auction.FinalPrice.ToString(),
                                                      auction.PayGuaranteeState.ToString(), auction.PayGuaranteeNumber.ToString(),
                                                      auction.ReturnGuaranteeState.ToString(), auction.ReturnGuaranteeNumber.ToString(),
                                                      auction.PayWayState.ToString(), auction.SellerServiceCharge.ToString(),
                                                      auction.ReservePrice.ToString(), auction.SellerAccountPayable.ToString()});
            }

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
        }

        private void ToCsV(DataGridView dgv, string filename)
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
