﻿namespace CheckoutDealer
{
    partial class CheckoutDealerForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDealerName = new System.Windows.Forms.Label();
            this.textBoxDealerName = new System.Windows.Forms.TextBox();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.listViewResult = new System.Windows.Forms.ListView();
            this.columnLotId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnItemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHammerPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPicPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnServicePrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnInsurancePrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnOtherPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTotalPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDealerName
            // 
            this.labelDealerName.AutoSize = true;
            this.labelDealerName.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelDealerName.Location = new System.Drawing.Point(12, 9);
            this.labelDealerName.Name = "labelDealerName";
            this.labelDealerName.Size = new System.Drawing.Size(72, 16);
            this.labelDealerName.TabIndex = 0;
            this.labelDealerName.Text = "賣家姓名";
            this.labelDealerName.Click += new System.EventHandler(this.labelDealerName_Click);
            // 
            // textBoxDealerName
            // 
            this.textBoxDealerName.Enabled = false;
            this.textBoxDealerName.Location = new System.Drawing.Point(88, 6);
            this.textBoxDealerName.Name = "textBoxDealerName";
            this.textBoxDealerName.Size = new System.Drawing.Size(100, 22);
            this.textBoxDealerName.TabIndex = 1;
            // 
            // buttonQuery
            // 
            this.buttonQuery.Enabled = false;
            this.buttonQuery.Location = new System.Drawing.Point(194, 5);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(75, 23);
            this.buttonQuery.TabIndex = 2;
            this.buttonQuery.Text = "查詢";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // listViewResult
            // 
            this.listViewResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLotId,
            this.columnAuthor,
            this.columnItemName,
            this.columnHammerPrice,
            this.columnPicPrice,
            this.columnServicePrice,
            this.columnInsurancePrice,
            this.columnOtherPrice,
            this.columnTotalPrice});
            this.listViewResult.GridLines = true;
            this.listViewResult.Location = new System.Drawing.Point(12, 34);
            this.listViewResult.Name = "listViewResult";
            this.listViewResult.Size = new System.Drawing.Size(587, 359);
            this.listViewResult.TabIndex = 3;
            this.listViewResult.UseCompatibleStateImageBehavior = false;
            this.listViewResult.View = System.Windows.Forms.View.Details;
            // 
            // columnLotId
            // 
            this.columnLotId.Text = "圖錄號";
            // 
            // columnAuthor
            // 
            this.columnAuthor.Text = "作者";
            // 
            // columnItemName
            // 
            this.columnItemName.Text = "作品名稱";
            this.columnItemName.Width = 110;
            // 
            // columnHammerPrice
            // 
            this.columnHammerPrice.Text = "落槌價";
            // 
            // columnPicPrice
            // 
            this.columnPicPrice.Text = "圖錄費";
            // 
            // columnServicePrice
            // 
            this.columnServicePrice.Text = "佣金";
            // 
            // columnInsurancePrice
            // 
            this.columnInsurancePrice.Text = "保險費";
            // 
            // columnOtherPrice
            // 
            this.columnOtherPrice.Text = "其他";
            // 
            // columnTotalPrice
            // 
            this.columnTotalPrice.Text = "合計";
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelMain});
            this.statusStripMain.Location = new System.Drawing.Point(0, 408);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(611, 22);
            this.statusStripMain.TabIndex = 5;
            // 
            // toolStripStatusLabelMain
            // 
            this.toolStripStatusLabelMain.Name = "toolStripStatusLabelMain";
            this.toolStripStatusLabelMain.Size = new System.Drawing.Size(0, 17);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(274, 5);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "儲存";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(354, 5);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(75, 23);
            this.buttonPrint.TabIndex = 7;
            this.buttonPrint.Text = "列印";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // CheckoutDealerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 430);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.listViewResult);
            this.Controls.Add(this.buttonQuery);
            this.Controls.Add(this.textBoxDealerName);
            this.Controls.Add(this.labelDealerName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CheckoutDealerForm";
            this.Text = "賣家結帳";
            this.Load += new System.EventHandler(this.CheckoutDealerForm_Load);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDealerName;
        private System.Windows.Forms.TextBox textBoxDealerName;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.ListView listViewResult;
        private System.Windows.Forms.ColumnHeader columnLotId;
        private System.Windows.Forms.ColumnHeader columnItemName;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMain;
        private System.Windows.Forms.ColumnHeader columnAuthor;
        private System.Windows.Forms.ColumnHeader columnHammerPrice;
        private System.Windows.Forms.ColumnHeader columnPicPrice;
        private System.Windows.Forms.ColumnHeader columnServicePrice;
        private System.Windows.Forms.ColumnHeader columnInsurancePrice;
        private System.Windows.Forms.ColumnHeader columnOtherPrice;
        private System.Windows.Forms.ColumnHeader columnTotalPrice;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonPrint;
    }
}

