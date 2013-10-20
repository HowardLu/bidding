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
            Microsoft.Office.Interop.Excel.Application a = new Microsoft.Office.Interop.Excel.Application();
            _Application a = new _Application();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_internet.Connect();
            m_internet.Update();
            LoadCollectionToDataGridView();
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
            dataGridView1.DataSource = m_internet.Collection.FindAll().ToList<AuctionEntity>();
        }
        #endregion
    }
}
