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

namespace Accounting
{
    public partial class Form1 : Form
    {
        #region Properties
        // Contains a reference to the hosting application
        private Microsoft.Office.Interop.Excel.Application m_XlApplication = null;
        // Contains a reference to the active workbook
        private Workbook m_Workbook = null;
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
            this.excelWrapper1.OpenFile(Path.Combine(System.Windows.Forms.Application.StartupPath, @"316.xls"));
            //this.excelWrapper1.OpenFile(@"https://www.google.com.tw/");
        }
        #endregion

        #region Methods
        #endregion
    }
}
