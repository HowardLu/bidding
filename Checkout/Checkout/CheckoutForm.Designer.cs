namespace Checkout
{
    partial class CheckoutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckoutForm));
            this.bidderNoLabel = new System.Windows.Forms.Label();
            this.bidderNoTextBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.printButton = new System.Windows.Forms.Button();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.auctionsListView = new System.Windows.Forms.ListView();
            this.lotColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hammerPriceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.serviceChargeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.totalColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectButton = new System.Windows.Forms.Button();
            this.isUseCardButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.isPrintOneByOneCheckBox = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bidderNoLabel
            // 
            this.bidderNoLabel.AutoSize = true;
            this.bidderNoLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.bidderNoLabel.Location = new System.Drawing.Point(12, 8);
            this.bidderNoLabel.Name = "bidderNoLabel";
            this.bidderNoLabel.Size = new System.Drawing.Size(93, 16);
            this.bidderNoLabel.TabIndex = 0;
            this.bidderNoLabel.Text = "競投牌號：";
            // 
            // bidderNoTextBox
            // 
            this.bidderNoTextBox.Enabled = false;
            this.bidderNoTextBox.Location = new System.Drawing.Point(103, 5);
            this.bidderNoTextBox.Name = "bidderNoTextBox";
            this.bidderNoTextBox.Size = new System.Drawing.Size(76, 22);
            this.bidderNoTextBox.TabIndex = 1;
            this.bidderNoTextBox.TextChanged += new System.EventHandler(this.bidderNoTextBox_TextChanged);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.saveButton.Location = new System.Drawing.Point(360, 5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(72, 23);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "儲存";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Enabled = false;
            this.searchButton.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.searchButton.Location = new System.Drawing.Point(185, 5);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(72, 23);
            this.searchButton.TabIndex = 3;
            this.searchButton.Text = "查詢";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // printButton
            // 
            this.printButton.Enabled = false;
            this.printButton.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.printButton.Location = new System.Drawing.Point(440, 5);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(72, 23);
            this.printButton.TabIndex = 4;
            this.printButton.Text = "列印";
            this.printButton.UseVisualStyleBackColor = true;
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // auctionsListView
            // 
            this.auctionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.auctionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lotColumnHeader,
            this.nameColumnHeader,
            this.hammerPriceColumnHeader,
            this.serviceChargeColumnHeader,
            this.totalColumnHeader});
            this.auctionsListView.Font = new System.Drawing.Font("PMingLiU", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.auctionsListView.FullRowSelect = true;
            this.auctionsListView.GridLines = true;
            this.auctionsListView.Location = new System.Drawing.Point(15, 48);
            this.auctionsListView.Name = "auctionsListView";
            this.auctionsListView.Size = new System.Drawing.Size(664, 500);
            this.auctionsListView.TabIndex = 5;
            this.auctionsListView.UseCompatibleStateImageBehavior = false;
            this.auctionsListView.View = System.Windows.Forms.View.Details;
            this.auctionsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.auctionsListView_ColumnClick);
            // 
            // lotColumnHeader
            // 
            this.lotColumnHeader.Text = "編號(Lot)";
            this.lotColumnHeader.Width = 70;
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "拍品名稱(Lot Name)";
            this.nameColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nameColumnHeader.Width = 220;
            // 
            // hammerPriceColumnHeader
            // 
            this.hammerPriceColumnHeader.Text = "落槌價(NTD)";
            this.hammerPriceColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.hammerPriceColumnHeader.Width = 100;
            // 
            // serviceChargeColumnHeader
            // 
            this.serviceChargeColumnHeader.Text = "服務費(NTD)";
            this.serviceChargeColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.serviceChargeColumnHeader.Width = 100;
            // 
            // totalColumnHeader
            // 
            this.totalColumnHeader.Text = "成交價(NTD)";
            this.totalColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.totalColumnHeader.Width = 170;
            // 
            // connectButton
            // 
            this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connectButton.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.connectButton.Location = new System.Drawing.Point(627, 5);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(52, 23);
            this.connectButton.TabIndex = 7;
            this.connectButton.Text = "連線";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // isUseCardButton
            // 
            this.isUseCardButton.Enabled = false;
            this.isUseCardButton.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.isUseCardButton.Location = new System.Drawing.Point(280, 5);
            this.isUseCardButton.Name = "isUseCardButton";
            this.isUseCardButton.Size = new System.Drawing.Size(72, 23);
            this.isUseCardButton.TabIndex = 8;
            this.isUseCardButton.Text = "是/否刷卡";
            this.isUseCardButton.UseVisualStyleBackColor = true;
            this.isUseCardButton.Visible = false;
            this.isUseCardButton.Click += new System.EventHandler(this.isUseCardButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 560);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(692, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(77, 17);
            this.toolStripStatusLabel1.Text = "請先連線!";
            // 
            // isPrintOneByOneCheckBox
            // 
            this.isPrintOneByOneCheckBox.AutoSize = true;
            this.isPrintOneByOneCheckBox.Enabled = false;
            this.isPrintOneByOneCheckBox.Location = new System.Drawing.Point(512, 8);
            this.isPrintOneByOneCheckBox.Name = "isPrintOneByOneCheckBox";
            this.isPrintOneByOneCheckBox.Size = new System.Drawing.Size(72, 16);
            this.isPrintOneByOneCheckBox.TabIndex = 10;
            this.isPrintOneByOneCheckBox.Text = "單項列印";
            this.isPrintOneByOneCheckBox.UseVisualStyleBackColor = true;
            // 
            // CheckoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 582);
            this.Controls.Add(this.isPrintOneByOneCheckBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.isUseCardButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.auctionsListView);
            this.Controls.Add(this.printButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.bidderNoTextBox);
            this.Controls.Add(this.bidderNoLabel);
            this.Name = "CheckoutForm";
            this.Text = "結帳";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CheckoutForm_FormClosed);
            this.Load += new System.EventHandler(this.CheckoutForm_Load);
            this.Resize += new System.EventHandler(this.CheckoutForm_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label bidderNoLabel;
        private System.Windows.Forms.TextBox bidderNoTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button printButton;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ListView auctionsListView;
        private System.Windows.Forms.ColumnHeader lotColumnHeader;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader hammerPriceColumnHeader;
        private System.Windows.Forms.ColumnHeader serviceChargeColumnHeader;
        private System.Windows.Forms.ColumnHeader totalColumnHeader;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button isUseCardButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.CheckBox isPrintOneByOneCheckBox;
    }
}

