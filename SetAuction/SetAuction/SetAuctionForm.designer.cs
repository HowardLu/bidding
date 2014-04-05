namespace SetAuction
{
    partial class SetAuctionForm
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
            this.auctionsListView = new System.Windows.Forms.ListView();
            this.lotColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.artistColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.artworkColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.initialPriceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewsGroupBox = new System.Windows.Forms.GroupBox();
            this.unitLabel = new System.Windows.Forms.Label();
            this.unitComboBox = new System.Windows.Forms.ComboBox();
            this.tileRadioButton = new System.Windows.Forms.RadioButton();
            this.listRadioButton = new System.Windows.Forms.RadioButton();
            this.smallIconRadioButton = new System.Windows.Forms.RadioButton();
            this.detailsRadioButton = new System.Windows.Forms.RadioButton();
            this.largeIconRadioButton = new System.Windows.Forms.RadioButton();
            this.setAuctionGroupBox = new System.Windows.Forms.GroupBox();
            this.auctioneerComboBox = new System.Windows.Forms.ComboBox();
            this.companyLabel = new System.Windows.Forms.Label();
            this.openPhotoButton = new System.Windows.Forms.Button();
            this.photoTextBox = new System.Windows.Forms.TextBox();
            this.auctionPhotoLabel = new System.Windows.Forms.Label();
            this.initialPriceTextBox = new System.Windows.Forms.TextBox();
            this.artworkTextBox = new System.Windows.Forms.TextBox();
            this.artistTextBox = new System.Windows.Forms.TextBox();
            this.lotTextBox = new System.Windows.Forms.TextBox();
            this.artworkLabel = new System.Windows.Forms.Label();
            this.initialPriceLabel = new System.Windows.Forms.Label();
            this.artistLabel = new System.Windows.Forms.Label();
            this.lotLabel = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.viewsGroupBox.SuspendLayout();
            this.setAuctionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // auctionsListView
            // 
            this.auctionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.auctionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lotColumnHeader,
            this.artistColumnHeader,
            this.artworkColumnHeader,
            this.initialPriceColumnHeader});
            this.auctionsListView.FullRowSelect = true;
            this.auctionsListView.GridLines = true;
            this.auctionsListView.Location = new System.Drawing.Point(10, 10);
            this.auctionsListView.Name = "auctionsListView";
            this.auctionsListView.Size = new System.Drawing.Size(620, 545);
            this.auctionsListView.TabIndex = 0;
            this.auctionsListView.UseCompatibleStateImageBehavior = false;
            this.auctionsListView.View = System.Windows.Forms.View.Details;
            this.auctionsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.auctionsListView_ColumnClick);
            this.auctionsListView.SelectedIndexChanged += new System.EventHandler(this.auctionsListView_SelectedIndexChanged);
            // 
            // lotColumnHeader
            // 
            this.lotColumnHeader.Text = "Lot";
            // 
            // artistColumnHeader
            // 
            this.artistColumnHeader.Text = "作者";
            this.artistColumnHeader.Width = 160;
            // 
            // artworkColumnHeader
            // 
            this.artworkColumnHeader.Text = "拍品名稱";
            this.artworkColumnHeader.Width = 230;
            // 
            // initialPriceColumnHeader
            // 
            this.initialPriceColumnHeader.Text = "起拍價";
            this.initialPriceColumnHeader.Width = 166;
            // 
            // viewsGroupBox
            // 
            this.viewsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewsGroupBox.Controls.Add(this.unitLabel);
            this.viewsGroupBox.Controls.Add(this.unitComboBox);
            this.viewsGroupBox.Controls.Add(this.tileRadioButton);
            this.viewsGroupBox.Controls.Add(this.listRadioButton);
            this.viewsGroupBox.Controls.Add(this.smallIconRadioButton);
            this.viewsGroupBox.Controls.Add(this.detailsRadioButton);
            this.viewsGroupBox.Controls.Add(this.largeIconRadioButton);
            this.viewsGroupBox.Location = new System.Drawing.Point(635, 325);
            this.viewsGroupBox.Name = "viewsGroupBox";
            this.viewsGroupBox.Size = new System.Drawing.Size(181, 183);
            this.viewsGroupBox.TabIndex = 3;
            this.viewsGroupBox.TabStop = false;
            this.viewsGroupBox.Text = "顯示";
            // 
            // unitLabel
            // 
            this.unitLabel.AutoSize = true;
            this.unitLabel.Location = new System.Drawing.Point(9, 149);
            this.unitLabel.Name = "unitLabel";
            this.unitLabel.Size = new System.Drawing.Size(29, 12);
            this.unitLabel.TabIndex = 9;
            this.unitLabel.Text = "單位";
            // 
            // unitComboBox
            // 
            this.unitComboBox.FormattingEnabled = true;
            this.unitComboBox.Items.AddRange(new object[] {
            "個",
            "千",
            "萬"});
            this.unitComboBox.Location = new System.Drawing.Point(70, 146);
            this.unitComboBox.Name = "unitComboBox";
            this.unitComboBox.Size = new System.Drawing.Size(87, 20);
            this.unitComboBox.TabIndex = 8;
            this.unitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // tileRadioButton
            // 
            this.tileRadioButton.AutoSize = true;
            this.tileRadioButton.Location = new System.Drawing.Point(15, 112);
            this.tileRadioButton.Name = "tileRadioButton";
            this.tileRadioButton.Size = new System.Drawing.Size(47, 16);
            this.tileRadioButton.TabIndex = 7;
            this.tileRadioButton.Text = "並排";
            this.tileRadioButton.UseVisualStyleBackColor = true;
            this.tileRadioButton.CheckedChanged += new System.EventHandler(this.tileRadioButton_CheckedChanged);
            // 
            // listRadioButton
            // 
            this.listRadioButton.AutoSize = true;
            this.listRadioButton.Location = new System.Drawing.Point(15, 90);
            this.listRadioButton.Name = "listRadioButton";
            this.listRadioButton.Size = new System.Drawing.Size(47, 16);
            this.listRadioButton.TabIndex = 6;
            this.listRadioButton.Text = "清單";
            this.listRadioButton.UseVisualStyleBackColor = true;
            this.listRadioButton.CheckedChanged += new System.EventHandler(this.listRadioButton_CheckedChanged);
            // 
            // smallIconRadioButton
            // 
            this.smallIconRadioButton.AutoSize = true;
            this.smallIconRadioButton.Location = new System.Drawing.Point(15, 68);
            this.smallIconRadioButton.Name = "smallIconRadioButton";
            this.smallIconRadioButton.Size = new System.Drawing.Size(59, 16);
            this.smallIconRadioButton.TabIndex = 5;
            this.smallIconRadioButton.Text = "小圖示";
            this.smallIconRadioButton.UseVisualStyleBackColor = true;
            this.smallIconRadioButton.CheckedChanged += new System.EventHandler(this.smallIconRadioButton_CheckedChanged);
            // 
            // detailsRadioButton
            // 
            this.detailsRadioButton.AutoSize = true;
            this.detailsRadioButton.Checked = true;
            this.detailsRadioButton.Location = new System.Drawing.Point(15, 46);
            this.detailsRadioButton.Name = "detailsRadioButton";
            this.detailsRadioButton.Size = new System.Drawing.Size(47, 16);
            this.detailsRadioButton.TabIndex = 4;
            this.detailsRadioButton.TabStop = true;
            this.detailsRadioButton.Text = "詳細";
            this.detailsRadioButton.UseVisualStyleBackColor = true;
            this.detailsRadioButton.CheckedChanged += new System.EventHandler(this.detailsRadioButton_CheckedChanged);
            // 
            // largeIconRadioButton
            // 
            this.largeIconRadioButton.AutoSize = true;
            this.largeIconRadioButton.Location = new System.Drawing.Point(15, 24);
            this.largeIconRadioButton.Name = "largeIconRadioButton";
            this.largeIconRadioButton.Size = new System.Drawing.Size(59, 16);
            this.largeIconRadioButton.TabIndex = 3;
            this.largeIconRadioButton.Text = "大圖示";
            this.largeIconRadioButton.UseVisualStyleBackColor = true;
            this.largeIconRadioButton.CheckedChanged += new System.EventHandler(this.largeIconRadioButton_CheckedChanged);
            // 
            // setAuctionGroupBox
            // 
            this.setAuctionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.setAuctionGroupBox.Controls.Add(this.auctioneerComboBox);
            this.setAuctionGroupBox.Controls.Add(this.companyLabel);
            this.setAuctionGroupBox.Controls.Add(this.openPhotoButton);
            this.setAuctionGroupBox.Controls.Add(this.photoTextBox);
            this.setAuctionGroupBox.Controls.Add(this.auctionPhotoLabel);
            this.setAuctionGroupBox.Controls.Add(this.initialPriceTextBox);
            this.setAuctionGroupBox.Controls.Add(this.artworkTextBox);
            this.setAuctionGroupBox.Controls.Add(this.artistTextBox);
            this.setAuctionGroupBox.Controls.Add(this.lotTextBox);
            this.setAuctionGroupBox.Controls.Add(this.artworkLabel);
            this.setAuctionGroupBox.Controls.Add(this.initialPriceLabel);
            this.setAuctionGroupBox.Controls.Add(this.artistLabel);
            this.setAuctionGroupBox.Controls.Add(this.lotLabel);
            this.setAuctionGroupBox.Controls.Add(this.deleteButton);
            this.setAuctionGroupBox.Controls.Add(this.saveButton);
            this.setAuctionGroupBox.Controls.Add(this.addButton);
            this.setAuctionGroupBox.Location = new System.Drawing.Point(635, 10);
            this.setAuctionGroupBox.Name = "setAuctionGroupBox";
            this.setAuctionGroupBox.Size = new System.Drawing.Size(214, 262);
            this.setAuctionGroupBox.TabIndex = 4;
            this.setAuctionGroupBox.TabStop = false;
            this.setAuctionGroupBox.Text = "設定";
            // 
            // auctioneerComboBox
            // 
            this.auctioneerComboBox.FormattingEnabled = true;
            this.auctioneerComboBox.Location = new System.Drawing.Point(70, 194);
            this.auctioneerComboBox.Name = "auctioneerComboBox";
            this.auctioneerComboBox.Size = new System.Drawing.Size(121, 20);
            this.auctioneerComboBox.TabIndex = 15;
            this.auctioneerComboBox.Visible = false;
            // 
            // companyLabel
            // 
            this.companyLabel.AutoSize = true;
            this.companyLabel.Location = new System.Drawing.Point(9, 199);
            this.companyLabel.Name = "companyLabel";
            this.companyLabel.Size = new System.Drawing.Size(29, 12);
            this.companyLabel.TabIndex = 14;
            this.companyLabel.Text = "公司";
            this.companyLabel.Visible = false;
            // 
            // openPhotoButton
            // 
            this.openPhotoButton.Location = new System.Drawing.Point(181, 169);
            this.openPhotoButton.Name = "openPhotoButton";
            this.openPhotoButton.Size = new System.Drawing.Size(29, 19);
            this.openPhotoButton.TabIndex = 13;
            this.openPhotoButton.Text = "...";
            this.openPhotoButton.UseVisualStyleBackColor = true;
            this.openPhotoButton.Click += new System.EventHandler(this.openPhotoButton_Click);
            // 
            // photoTextBox
            // 
            this.photoTextBox.BackColor = System.Drawing.Color.White;
            this.photoTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.photoTextBox.Location = new System.Drawing.Point(70, 165);
            this.photoTextBox.Name = "photoTextBox";
            this.photoTextBox.ReadOnly = true;
            this.photoTextBox.Size = new System.Drawing.Size(111, 22);
            this.photoTextBox.TabIndex = 12;
            // 
            // auctionPhotoLabel
            // 
            this.auctionPhotoLabel.AutoSize = true;
            this.auctionPhotoLabel.Location = new System.Drawing.Point(9, 168);
            this.auctionPhotoLabel.Name = "auctionPhotoLabel";
            this.auctionPhotoLabel.Size = new System.Drawing.Size(53, 12);
            this.auctionPhotoLabel.TabIndex = 11;
            this.auctionPhotoLabel.Text = "拍品圖片";
            // 
            // initialPriceTextBox
            // 
            this.initialPriceTextBox.Location = new System.Drawing.Point(70, 132);
            this.initialPriceTextBox.Name = "initialPriceTextBox";
            this.initialPriceTextBox.Size = new System.Drawing.Size(123, 22);
            this.initialPriceTextBox.TabIndex = 10;
            this.initialPriceTextBox.TextChanged += new System.EventHandler(this.initialPriceTextBox_TextChanged);
            // 
            // artworkTextBox
            // 
            this.artworkTextBox.Location = new System.Drawing.Point(70, 85);
            this.artworkTextBox.Multiline = true;
            this.artworkTextBox.Name = "artworkTextBox";
            this.artworkTextBox.Size = new System.Drawing.Size(123, 41);
            this.artworkTextBox.TabIndex = 9;
            this.artworkTextBox.TextChanged += new System.EventHandler(this.artworkTextBox_TextChanged);
            // 
            // artistTextBox
            // 
            this.artistTextBox.Location = new System.Drawing.Point(70, 57);
            this.artistTextBox.Name = "artistTextBox";
            this.artistTextBox.Size = new System.Drawing.Size(123, 22);
            this.artistTextBox.TabIndex = 8;
            this.artistTextBox.TextChanged += new System.EventHandler(this.artistTextBox_TextChanged);
            // 
            // lotTextBox
            // 
            this.lotTextBox.Location = new System.Drawing.Point(70, 31);
            this.lotTextBox.Name = "lotTextBox";
            this.lotTextBox.Size = new System.Drawing.Size(84, 22);
            this.lotTextBox.TabIndex = 7;
            this.lotTextBox.TextChanged += new System.EventHandler(this.lotTextBox_TextChanged);
            // 
            // artworkLabel
            // 
            this.artworkLabel.AutoSize = true;
            this.artworkLabel.Location = new System.Drawing.Point(7, 88);
            this.artworkLabel.Name = "artworkLabel";
            this.artworkLabel.Size = new System.Drawing.Size(53, 12);
            this.artworkLabel.TabIndex = 6;
            this.artworkLabel.Text = "拍品名稱";
            // 
            // initialPriceLabel
            // 
            this.initialPriceLabel.AutoSize = true;
            this.initialPriceLabel.Location = new System.Drawing.Point(9, 135);
            this.initialPriceLabel.Name = "initialPriceLabel";
            this.initialPriceLabel.Size = new System.Drawing.Size(41, 12);
            this.initialPriceLabel.TabIndex = 5;
            this.initialPriceLabel.Text = "起拍價";
            // 
            // artistLabel
            // 
            this.artistLabel.AutoSize = true;
            this.artistLabel.Location = new System.Drawing.Point(7, 60);
            this.artistLabel.Name = "artistLabel";
            this.artistLabel.Size = new System.Drawing.Size(29, 12);
            this.artistLabel.TabIndex = 4;
            this.artistLabel.Text = "作者";
            // 
            // lotLabel
            // 
            this.lotLabel.AutoSize = true;
            this.lotLabel.Location = new System.Drawing.Point(7, 34);
            this.lotLabel.Name = "lotLabel";
            this.lotLabel.Size = new System.Drawing.Size(21, 12);
            this.lotLabel.TabIndex = 3;
            this.lotLabel.Text = "Lot";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(159, 232);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(50, 20);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "刪除";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(65, 232);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(50, 20);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "儲存";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(6, 232);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(50, 20);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "新增";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // SetAuctionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(851, 562);
            this.Controls.Add(this.setAuctionGroupBox);
            this.Controls.Add(this.viewsGroupBox);
            this.Controls.Add(this.auctionsListView);
            this.Name = "SetAuctionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拍品設定";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SetAuctionForm_FormClosed);
            this.Load += new System.EventHandler(this.SetAuctionForm_Load);
            this.Resize += new System.EventHandler(this.SetAuctionForm_Resize);
            this.viewsGroupBox.ResumeLayout(false);
            this.viewsGroupBox.PerformLayout();
            this.setAuctionGroupBox.ResumeLayout(false);
            this.setAuctionGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView auctionsListView;
        private System.Windows.Forms.ColumnHeader lotColumnHeader;
        private System.Windows.Forms.ColumnHeader artistColumnHeader;
        private System.Windows.Forms.ColumnHeader artworkColumnHeader;
        private System.Windows.Forms.ColumnHeader initialPriceColumnHeader;
        private System.Windows.Forms.GroupBox viewsGroupBox;
        private System.Windows.Forms.RadioButton detailsRadioButton;
        private System.Windows.Forms.RadioButton largeIconRadioButton;
        private System.Windows.Forms.RadioButton tileRadioButton;
        private System.Windows.Forms.RadioButton listRadioButton;
        private System.Windows.Forms.RadioButton smallIconRadioButton;
        private System.Windows.Forms.GroupBox setAuctionGroupBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label artworkLabel;
        private System.Windows.Forms.Label initialPriceLabel;
        private System.Windows.Forms.Label artistLabel;
        private System.Windows.Forms.Label lotLabel;
        private System.Windows.Forms.TextBox initialPriceTextBox;
        private System.Windows.Forms.TextBox artworkTextBox;
        private System.Windows.Forms.TextBox artistTextBox;
        private System.Windows.Forms.TextBox lotTextBox;
        private System.Windows.Forms.Label auctionPhotoLabel;
        private System.Windows.Forms.Button openPhotoButton;
        private System.Windows.Forms.TextBox photoTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label companyLabel;
        private System.Windows.Forms.ComboBox auctioneerComboBox;
        private System.Windows.Forms.ComboBox unitComboBox;
        private System.Windows.Forms.Label unitLabel;
    }
}