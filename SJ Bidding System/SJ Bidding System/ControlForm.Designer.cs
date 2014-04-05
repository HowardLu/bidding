namespace SJ_Bidding_System
{
    partial class ControlForm
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LotLabel = new System.Windows.Forms.Label();
            this.LotNumberLabel = new System.Windows.Forms.Label();
            this.ArtistNameLabel = new System.Windows.Forms.Label();
            this.ArtistLabel = new System.Windows.Forms.Label();
            this.ArtworkNameLabel = new System.Windows.Forms.Label();
            this.ArtworkLabel = new System.Windows.Forms.Label();
            this.initPriceLabel = new System.Windows.Forms.Label();
            this.ipLabel = new System.Windows.Forms.Label();
            this.nowPriceLabel = new System.Windows.Forms.Label();
            this.nowPriceTextBox = new System.Windows.Forms.TextBox();
            this.increaseByLevelBtn = new System.Windows.Forms.Button();
            this.prevsBtn = new System.Windows.Forms.Button();
            this.resetBtn = new System.Windows.Forms.Button();
            this.rateLabel = new System.Windows.Forms.Label();
            this.rmbLabel = new System.Windows.Forms.Label();
            this.usdLabel = new System.Windows.Forms.Label();
            this.rmbTextBox = new System.Windows.Forms.TextBox();
            this.usdTextBox = new System.Windows.Forms.TextBox();
            this.auctionComboBox = new System.Windows.Forms.ComboBox();
            this.resetAllBtn = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.hkTextBox = new System.Windows.Forms.TextBox();
            this.hkLabel = new System.Windows.Forms.Label();
            this.winBidderTextBox = new System.Windows.Forms.TextBox();
            this.winBidderLabel = new System.Windows.Forms.Label();
            this.confirmBidderButton = new System.Windows.Forms.Button();
            this.clearBidderButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.nextBtn = new System.Windows.Forms.Button();
            this.auctionPictureBox = new System.Windows.Forms.PictureBox();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.sessionLabel = new System.Windows.Forms.Label();
            this.sessionComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.auctionPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // LotLabel
            // 
            this.LotLabel.AutoSize = true;
            this.LotLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.LotLabel.ForeColor = System.Drawing.Color.White;
            this.LotLabel.Location = new System.Drawing.Point(70, 247);
            this.LotLabel.Name = "LotLabel";
            this.LotLabel.Size = new System.Drawing.Size(62, 31);
            this.LotLabel.TabIndex = 0;
            this.LotLabel.Text = "Lot :";
            // 
            // LotNumberLabel
            // 
            this.LotNumberLabel.AutoSize = true;
            this.LotNumberLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.LotNumberLabel.ForeColor = System.Drawing.Color.White;
            this.LotNumberLabel.Location = new System.Drawing.Point(191, 247);
            this.LotNumberLabel.Name = "LotNumberLabel";
            this.LotNumberLabel.Size = new System.Drawing.Size(0, 31);
            this.LotNumberLabel.TabIndex = 1;
            // 
            // ArtistNameLabel
            // 
            this.ArtistNameLabel.AutoSize = true;
            this.ArtistNameLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ArtistNameLabel.ForeColor = System.Drawing.Color.White;
            this.ArtistNameLabel.Location = new System.Drawing.Point(191, 278);
            this.ArtistNameLabel.Name = "ArtistNameLabel";
            this.ArtistNameLabel.Size = new System.Drawing.Size(0, 31);
            this.ArtistNameLabel.TabIndex = 3;
            // 
            // ArtistLabel
            // 
            this.ArtistLabel.AutoSize = true;
            this.ArtistLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ArtistLabel.ForeColor = System.Drawing.Color.White;
            this.ArtistLabel.Location = new System.Drawing.Point(70, 278);
            this.ArtistLabel.Name = "ArtistLabel";
            this.ArtistLabel.Size = new System.Drawing.Size(87, 31);
            this.ArtistLabel.TabIndex = 2;
            this.ArtistLabel.Text = "Artist :";
            // 
            // ArtworkNameLabel
            // 
            this.ArtworkNameLabel.AutoSize = true;
            this.ArtworkNameLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ArtworkNameLabel.ForeColor = System.Drawing.Color.White;
            this.ArtworkNameLabel.Location = new System.Drawing.Point(191, 309);
            this.ArtworkNameLabel.Name = "ArtworkNameLabel";
            this.ArtworkNameLabel.Size = new System.Drawing.Size(0, 31);
            this.ArtworkNameLabel.TabIndex = 5;
            // 
            // ArtworkLabel
            // 
            this.ArtworkLabel.AutoSize = true;
            this.ArtworkLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ArtworkLabel.ForeColor = System.Drawing.Color.White;
            this.ArtworkLabel.Location = new System.Drawing.Point(70, 309);
            this.ArtworkLabel.Name = "ArtworkLabel";
            this.ArtworkLabel.Size = new System.Drawing.Size(117, 31);
            this.ArtworkLabel.TabIndex = 4;
            this.ArtworkLabel.Text = "Artwork :";
            // 
            // initPriceLabel
            // 
            this.initPriceLabel.AutoSize = true;
            this.initPriceLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.initPriceLabel.ForeColor = System.Drawing.Color.White;
            this.initPriceLabel.Location = new System.Drawing.Point(191, 340);
            this.initPriceLabel.Name = "initPriceLabel";
            this.initPriceLabel.Size = new System.Drawing.Size(0, 31);
            this.initPriceLabel.TabIndex = 7;
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ipLabel.ForeColor = System.Drawing.Color.White;
            this.ipLabel.Location = new System.Drawing.Point(70, 340);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(98, 31);
            this.ipLabel.TabIndex = 6;
            this.ipLabel.Text = "起拍價 :";
            // 
            // nowPriceLabel
            // 
            this.nowPriceLabel.AutoSize = true;
            this.nowPriceLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.nowPriceLabel.ForeColor = System.Drawing.Color.White;
            this.nowPriceLabel.Location = new System.Drawing.Point(63, 426);
            this.nowPriceLabel.Name = "nowPriceLabel";
            this.nowPriceLabel.Size = new System.Drawing.Size(122, 31);
            this.nowPriceLabel.TabIndex = 8;
            this.nowPriceLabel.Text = "目前拍價 :";
            // 
            // nowPriceTextBox
            // 
            this.nowPriceTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.nowPriceTextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.nowPriceTextBox.ForeColor = System.Drawing.Color.White;
            this.nowPriceTextBox.Location = new System.Drawing.Point(191, 422);
            this.nowPriceTextBox.Name = "nowPriceTextBox";
            this.nowPriceTextBox.Size = new System.Drawing.Size(192, 39);
            this.nowPriceTextBox.TabIndex = 14;
            this.nowPriceTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nowPriceTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nowPriceTextBox_KeyPress);
            // 
            // increaseByLevelBtn
            // 
            this.increaseByLevelBtn.BackColor = System.Drawing.Color.White;
            this.increaseByLevelBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.increaseByLevelBtn.Location = new System.Drawing.Point(69, 550);
            this.increaseByLevelBtn.Name = "increaseByLevelBtn";
            this.increaseByLevelBtn.Size = new System.Drawing.Size(154, 40);
            this.increaseByLevelBtn.TabIndex = 15;
            this.increaseByLevelBtn.Text = "照階跳價";
            this.increaseByLevelBtn.UseVisualStyleBackColor = false;
            this.increaseByLevelBtn.Click += new System.EventHandler(this.increaseByLevelBtn_Click);
            // 
            // prevsBtn
            // 
            this.prevsBtn.BackColor = System.Drawing.Color.Transparent;
            this.prevsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.prevsBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.prevsBtn.FlatAppearance.BorderSize = 0;
            this.prevsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.prevsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.prevsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.prevsBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.prevsBtn.ForeColor = System.Drawing.Color.Transparent;
            this.prevsBtn.Image = global::SJ_Bidding_System.Properties.Resources.Knob_Left;
            this.prevsBtn.Location = new System.Drawing.Point(491, 367);
            this.prevsBtn.Name = "prevsBtn";
            this.prevsBtn.Size = new System.Drawing.Size(32, 32);
            this.prevsBtn.TabIndex = 17;
            this.prevsBtn.UseVisualStyleBackColor = false;
            this.prevsBtn.Click += new System.EventHandler(this.prevsBtn_Click);
            // 
            // resetBtn
            // 
            this.resetBtn.BackColor = System.Drawing.Color.White;
            this.resetBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.resetBtn.Location = new System.Drawing.Point(268, 550);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(92, 40);
            this.resetBtn.TabIndex = 19;
            this.resetBtn.Text = "重置";
            this.resetBtn.UseVisualStyleBackColor = false;
            this.resetBtn.Click += new System.EventHandler(this.resetBtn_Click);
            // 
            // rateLabel
            // 
            this.rateLabel.AutoSize = true;
            this.rateLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rateLabel.ForeColor = System.Drawing.Color.White;
            this.rateLabel.Location = new System.Drawing.Point(78, 93);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.Size = new System.Drawing.Size(122, 31);
            this.rateLabel.TabIndex = 21;
            this.rateLabel.Text = "當天匯率 :";
            // 
            // rmbLabel
            // 
            this.rmbLabel.AutoSize = true;
            this.rmbLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rmbLabel.ForeColor = System.Drawing.Color.White;
            this.rmbLabel.Location = new System.Drawing.Point(90, 126);
            this.rmbLabel.Name = "rmbLabel";
            this.rmbLabel.Size = new System.Drawing.Size(68, 27);
            this.rmbLabel.TabIndex = 22;
            this.rmbLabel.Text = "RMB :";
            // 
            // usdLabel
            // 
            this.usdLabel.AutoSize = true;
            this.usdLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.usdLabel.ForeColor = System.Drawing.Color.White;
            this.usdLabel.Location = new System.Drawing.Point(288, 126);
            this.usdLabel.Name = "usdLabel";
            this.usdLabel.Size = new System.Drawing.Size(66, 27);
            this.usdLabel.TabIndex = 23;
            this.usdLabel.Text = "USD :";
            // 
            // rmbTextBox
            // 
            this.rmbTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.rmbTextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rmbTextBox.ForeColor = System.Drawing.Color.White;
            this.rmbTextBox.Location = new System.Drawing.Point(161, 124);
            this.rmbTextBox.Name = "rmbTextBox";
            this.rmbTextBox.Size = new System.Drawing.Size(121, 33);
            this.rmbTextBox.TabIndex = 24;
            this.rmbTextBox.TextChanged += new System.EventHandler(this.rmbTextBox_TextChanged);
            this.rmbTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rmbTextBox_KeyPress);
            // 
            // usdTextBox
            // 
            this.usdTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.usdTextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.usdTextBox.ForeColor = System.Drawing.Color.White;
            this.usdTextBox.Location = new System.Drawing.Point(358, 124);
            this.usdTextBox.Name = "usdTextBox";
            this.usdTextBox.Size = new System.Drawing.Size(121, 33);
            this.usdTextBox.TabIndex = 25;
            this.usdTextBox.TextChanged += new System.EventHandler(this.usdTextBox_TextChanged);
            this.usdTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.usdTextBox_KeyPress);
            // 
            // auctionComboBox
            // 
            this.auctionComboBox.FormattingEnabled = true;
            this.auctionComboBox.Location = new System.Drawing.Point(809, 551);
            this.auctionComboBox.Name = "auctionComboBox";
            this.auctionComboBox.Size = new System.Drawing.Size(102, 20);
            this.auctionComboBox.TabIndex = 26;
            this.auctionComboBox.SelectedIndexChanged += new System.EventHandler(this.auctionComboBox_SelectedIndexChanged);
            // 
            // resetAllBtn
            // 
            this.resetAllBtn.BackColor = System.Drawing.Color.White;
            this.resetAllBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.resetAllBtn.Location = new System.Drawing.Point(366, 550);
            this.resetAllBtn.Name = "resetAllBtn";
            this.resetAllBtn.Size = new System.Drawing.Size(131, 40);
            this.resetAllBtn.TabIndex = 27;
            this.resetAllBtn.Text = "重置全部";
            this.resetAllBtn.UseVisualStyleBackColor = false;
            this.resetAllBtn.Click += new System.EventHandler(this.resetAllBtn_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // hkTextBox
            // 
            this.hkTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.hkTextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.hkTextBox.ForeColor = System.Drawing.Color.White;
            this.hkTextBox.Location = new System.Drawing.Point(555, 124);
            this.hkTextBox.Name = "hkTextBox";
            this.hkTextBox.Size = new System.Drawing.Size(121, 33);
            this.hkTextBox.TabIndex = 29;
            this.hkTextBox.TextChanged += new System.EventHandler(this.hkTextBox_TextChanged);
            this.hkTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.hkTextBox_KeyPress);
            // 
            // hkLabel
            // 
            this.hkLabel.AutoSize = true;
            this.hkLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.hkLabel.ForeColor = System.Drawing.Color.White;
            this.hkLabel.Location = new System.Drawing.Point(485, 126);
            this.hkLabel.Name = "hkLabel";
            this.hkLabel.Size = new System.Drawing.Size(67, 27);
            this.hkLabel.TabIndex = 28;
            this.hkLabel.Text = "HKD :";
            // 
            // winBidderTextBox
            // 
            this.winBidderTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.winBidderTextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.winBidderTextBox.ForeColor = System.Drawing.Color.White;
            this.winBidderTextBox.Location = new System.Drawing.Point(191, 470);
            this.winBidderTextBox.Name = "winBidderTextBox";
            this.winBidderTextBox.Size = new System.Drawing.Size(109, 39);
            this.winBidderTextBox.TabIndex = 31;
            this.winBidderTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // winBidderLabel
            // 
            this.winBidderLabel.AutoSize = true;
            this.winBidderLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.winBidderLabel.ForeColor = System.Drawing.Color.White;
            this.winBidderLabel.Location = new System.Drawing.Point(87, 474);
            this.winBidderLabel.Name = "winBidderLabel";
            this.winBidderLabel.Size = new System.Drawing.Size(98, 31);
            this.winBidderLabel.TabIndex = 30;
            this.winBidderLabel.Text = "得標者 :";
            // 
            // confirmBidderButton
            // 
            this.confirmBidderButton.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.confirmBidderButton.Location = new System.Drawing.Point(311, 476);
            this.confirmBidderButton.Name = "confirmBidderButton";
            this.confirmBidderButton.Size = new System.Drawing.Size(56, 33);
            this.confirmBidderButton.TabIndex = 33;
            this.confirmBidderButton.Text = "確定";
            this.confirmBidderButton.UseVisualStyleBackColor = true;
            this.confirmBidderButton.Click += new System.EventHandler(this.confirmBidderButton_Click);
            // 
            // clearBidderButton
            // 
            this.clearBidderButton.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.clearBidderButton.Location = new System.Drawing.Point(373, 476);
            this.clearBidderButton.Name = "clearBidderButton";
            this.clearBidderButton.Size = new System.Drawing.Size(56, 33);
            this.clearBidderButton.TabIndex = 34;
            this.clearBidderButton.Text = "清除";
            this.clearBidderButton.UseVisualStyleBackColor = true;
            this.clearBidderButton.Click += new System.EventHandler(this.clearBidderButton_Click);
            // 
            // playButton
            // 
            this.playButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.playButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Image = global::SJ_Bidding_System.Properties.Resources.Knob_Play_Green;
            this.playButton.Location = new System.Drawing.Point(597, 552);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(32, 32);
            this.playButton.TabIndex = 35;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.stopButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.stopButton.FlatAppearance.BorderSize = 0;
            this.stopButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.stopButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopButton.Image = global::SJ_Bidding_System.Properties.Resources.Knob_Stop;
            this.stopButton.Location = new System.Drawing.Point(644, 552);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(32, 32);
            this.stopButton.TabIndex = 36;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // nextBtn
            // 
            this.nextBtn.BackColor = System.Drawing.Color.Transparent;
            this.nextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.nextBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.nextBtn.FlatAppearance.BorderSize = 0;
            this.nextBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.nextBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.nextBtn.ForeColor = System.Drawing.Color.Transparent;
            this.nextBtn.Image = global::SJ_Bidding_System.Properties.Resources.Knob_Forward;
            this.nextBtn.Location = new System.Drawing.Point(929, 367);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(32, 32);
            this.nextBtn.TabIndex = 18;
            this.nextBtn.UseVisualStyleBackColor = false;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // auctionPictureBox
            // 
            this.auctionPictureBox.Location = new System.Drawing.Point(543, 216);
            this.auctionPictureBox.Name = "auctionPictureBox";
            this.auctionPictureBox.Size = new System.Drawing.Size(368, 329);
            this.auctionPictureBox.TabIndex = 13;
            this.auctionPictureBox.TabStop = false;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = global::SJ_Bidding_System.Properties.Resources.LOGO_S;
            this.logoPictureBox.Location = new System.Drawing.Point(892, 24);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(100, 101);
            this.logoPictureBox.TabIndex = 10;
            this.logoPictureBox.TabStop = false;
            this.logoPictureBox.Visible = false;
            // 
            // sessionLabel
            // 
            this.sessionLabel.AutoSize = true;
            this.sessionLabel.ForeColor = System.Drawing.Color.White;
            this.sessionLabel.Location = new System.Drawing.Point(789, 181);
            this.sessionLabel.Name = "sessionLabel";
            this.sessionLabel.Size = new System.Drawing.Size(41, 12);
            this.sessionLabel.TabIndex = 37;
            this.sessionLabel.Text = "場次：";
            // 
            // sessionComboBox
            // 
            this.sessionComboBox.FormattingEnabled = true;
            this.sessionComboBox.Location = new System.Drawing.Point(836, 178);
            this.sessionComboBox.Name = "sessionComboBox";
            this.sessionComboBox.Size = new System.Drawing.Size(75, 20);
            this.sessionComboBox.TabIndex = 38;
            this.sessionComboBox.SelectedIndexChanged += new System.EventHandler(this.sessionComboBox_SelectedIndexChanged);
            // 
            // ControlForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1016, 730);
            this.Controls.Add(this.sessionComboBox);
            this.Controls.Add(this.sessionLabel);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.clearBidderButton);
            this.Controls.Add(this.confirmBidderButton);
            this.Controls.Add(this.winBidderTextBox);
            this.Controls.Add(this.winBidderLabel);
            this.Controls.Add(this.hkTextBox);
            this.Controls.Add(this.hkLabel);
            this.Controls.Add(this.resetAllBtn);
            this.Controls.Add(this.nowPriceTextBox);
            this.Controls.Add(this.auctionComboBox);
            this.Controls.Add(this.usdTextBox);
            this.Controls.Add(this.rmbTextBox);
            this.Controls.Add(this.usdLabel);
            this.Controls.Add(this.rmbLabel);
            this.Controls.Add(this.rateLabel);
            this.Controls.Add(this.resetBtn);
            this.Controls.Add(this.nextBtn);
            this.Controls.Add(this.prevsBtn);
            this.Controls.Add(this.increaseByLevelBtn);
            this.Controls.Add(this.auctionPictureBox);
            this.Controls.Add(this.logoPictureBox);
            this.Controls.Add(this.nowPriceLabel);
            this.Controls.Add(this.initPriceLabel);
            this.Controls.Add(this.ipLabel);
            this.Controls.Add(this.ArtworkNameLabel);
            this.Controls.Add(this.ArtworkLabel);
            this.Controls.Add(this.ArtistNameLabel);
            this.Controls.Add(this.ArtistLabel);
            this.Controls.Add(this.LotNumberLabel);
            this.Controls.Add(this.LotLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "ControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "台灣沐春堂拍賣系統";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlForm_FormClosing);
            this.Load += new System.EventHandler(this.ControlForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.auctionPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LotLabel;
        private System.Windows.Forms.Label LotNumberLabel;
        private System.Windows.Forms.Label ArtistNameLabel;
        private System.Windows.Forms.Label ArtistLabel;
        private System.Windows.Forms.Label ArtworkNameLabel;
        private System.Windows.Forms.Label ArtworkLabel;
        private System.Windows.Forms.Label initPriceLabel;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Label nowPriceLabel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.PictureBox auctionPictureBox;
        private System.Windows.Forms.TextBox nowPriceTextBox;
        private System.Windows.Forms.Button increaseByLevelBtn;
        private System.Windows.Forms.Button prevsBtn;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.Button resetBtn;
        private System.Windows.Forms.Label rateLabel;
        private System.Windows.Forms.Label rmbLabel;
        private System.Windows.Forms.Label usdLabel;
        private System.Windows.Forms.TextBox rmbTextBox;
        private System.Windows.Forms.TextBox usdTextBox;
        private System.Windows.Forms.ComboBox auctionComboBox;
        private System.Windows.Forms.Button resetAllBtn;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox hkTextBox;
        private System.Windows.Forms.Label hkLabel;
        private System.Windows.Forms.TextBox winBidderTextBox;
        private System.Windows.Forms.Label winBidderLabel;
        private System.Windows.Forms.Button confirmBidderButton;
        private System.Windows.Forms.Button clearBidderButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label sessionLabel;
        private System.Windows.Forms.ComboBox sessionComboBox;
    }
}

